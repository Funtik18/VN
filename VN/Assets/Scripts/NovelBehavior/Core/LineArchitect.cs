﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineArchitect {
	public static LINE Interpret( string rawLine ) {
		return new LINE(rawLine);
	}


	public class LINE {
		/// <summary>Who is speaking on this line. </summary>
		public string speaker = "";
		/// <summary>
		/// the speaker may not be displaying their actual name, instead showing another one temporarily. Good for introducing new characters.
		/// </summary>
		public string speakerDisplayName = "";

		/// <summary>The segments on this line that make up each piece of dialogue. </summary>
		public List<SEGMENT> segments = new List<SEGMENT>();

		/// <summary> All the actions to be called during or at the end of this line. </summary>
		public List<string> actions = new List<string>();

		public string lastSegmentsWholeDialogue = "";

		public LINE( string rawLine ) {
			string[] dialogueAndActions = rawLine.Split('"');
			char actionSplitter = ' ';
			string[] actionsArr = dialogueAndActions.Length == 3 ? dialogueAndActions[2].Split(actionSplitter) : dialogueAndActions[0].Split(actionSplitter);

			if (dialogueAndActions.Length == 3) {//contains dialogue

				speaker = dialogueAndActions[0] == "" ? NovelController._instance.cachedLastSpeaker : dialogueAndActions[0];
				if (speaker.Length > 0 && speaker[speaker.Length - 1] == ' ')
					speaker = speaker.Remove(speaker.Length - 1);

				////////////////////////////
				if (speaker.Contains(" as ")) {//the speaker is speaking as someone else. Using a false display name such as ??? or Old Man
					string[] speakerAndDisplay = speaker.Split(new string[1] { " as " }, System.StringSplitOptions.None);
					speaker = speakerAndDisplay[0];
					speakerDisplayName = speakerAndDisplay[1];
				} else
					speakerDisplayName = "";//only change the display name if one is provided. Allows us to keep the last display name of a character without having to type it on every dialogue line.

				//the speaker may be a nickname. We must be sure we have the correct name for this character.
				string fetchedSpeakerName = "";
				//if (CharacterDialogueDetails.instance.characterNicknameDictionary.TryGetValue(speaker, out fetchedSpeakerName))
				//speaker = fetchedSpeakerName;

				//self is a shortcut for the actual name of the character.
				if (speakerDisplayName == "self")//only called when a character is speaked as someone else. but it must come after the nickname lookup.
					speakerDisplayName = speaker;

				NovelController._instance.cachedLastSpeaker = speakerDisplayName;

				SegmentDialogue(dialogueAndActions[1]);
			}

			
			for (int i = 0; i < actionsArr.Length; i++) {//handle actions.
				actions.Add(actionsArr[i]);
			}

			//the line is now segmented, the actions are loaded, and it is ready to be used.
		}

		/// <summary>
		/// Segments a line of dialogue into a list of individual parts separated by input delays.
		/// </summary>
		/// <param name="dialogue">Dialogue.</param>
		void SegmentDialogue( string dialogue ) {
			segments.Clear();
			string[] parts = dialogue.Split('{', '}');

			for (int i = 0; i < parts.Length; i++) {
				SEGMENT segment = new SEGMENT();
				bool isOdd = i % 2 != 0;

				//commands are odd indexed. Dialogue is always even.
				if (isOdd) {
					string[] commandData = parts[i].Split(' ');
					switch (commandData[0]) {
						case "c"://wait for input and clear.
						segment.trigger = SEGMENT.TRIGGER.waitClickClear;
						break;
						case "a"://wait for input and append.
						segment.trigger = SEGMENT.TRIGGER.waitClick;
						//appending requires fetching the text of the previous segment to be the pre text
						segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
						break;
						case "w"://wait for set time and clear.
						segment.trigger = SEGMENT.TRIGGER.autoDelay;
						segment.autoDelay = float.Parse(commandData[1]);
						break;
						case "wa"://wait for set time and append.
						segment.trigger = SEGMENT.TRIGGER.autoDelay;
						segment.autoDelay = float.Parse(commandData[1]);

						segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
						break;
					}
					i++;//increment so we move past the command and to the next bit of dialogue.
				}

				segment.dialogue = parts[i];
				segment.line = this;

				segments.Add(segment);
			}
		}


		public class SEGMENT {
			public LINE line;
			public string dialogue = "";
			public string pretext = "";
			public enum TRIGGER { waitClick, waitClickClear, autoDelay }
			public TRIGGER trigger = TRIGGER.waitClickClear;

			public float autoDelay = 0;

			public TextArchitect architect = null;

			List<string> allCurrentlyExecutedEvents = new List<string>();

			/// <summary>
			/// Run the segment and watch the dialogue build and display on the screen.
			/// </summary>
			public void Run() {
				if (running != null)
					NovelController._instance.StopCoroutine(running);
				running = NovelController._instance.StartCoroutine(Running());
			}
			public bool isRunning { get { return running != null; } }
			Coroutine running = null;
			IEnumerator Running() {
				allCurrentlyExecutedEvents.Clear();
				//take care of any tags that must be injected into the dialogue before we worry about events.
				TagEvents.Inject(ref dialogue);

				//split the dialogue by the event characters.
				string[] parts = dialogue.Split('[', ']');

				for (int i = 0; i < parts.Length; i++) {
					//events will always be odd indexed. Execute an event.
					bool isOdd = i % 2 != 0;
					if (isOdd) {
						LineEvents.HandleEvent(parts[i], this);
						allCurrentlyExecutedEvents.Add(parts[i]);
						i++;
					}

					string targDialogue = parts[i];

					if (line.speaker != "narrator" && !line.speaker.Contains("*")) {
						string s = line.speaker;

						Character character = CharacterManager._instance.GetCharacter(line.speaker);//Get the character and make them speak.
						if (character != null) {
							if (line.speakerDisplayName != "")//if the character is using a disguise name, we need to set it.
								character.displayName = line.speakerDisplayName;

							character.Say(targDialogue, i > 0 ? true : pretext != "");
						}else {//This is a character that has no images to display. Only show the name and the dialogue.
							string n = line.speakerDisplayName == "" ? line.speaker : line.speakerDisplayName;
							DialogueSystem._instance.Say(targDialogue, n, i > 0 ? true : pretext != "");
						}
					} else {
						DialogueSystem._instance.Say(targDialogue, line.speaker, i > 0 ? true : pretext != "");
					}

					architect = DialogueSystem._instance.currentArchitect;//yield while the dialogue system's architect is constructing the dialogue.

					while (architect.isConstructing)
						yield return new WaitForEndOfFrame();
				}
				running = null;
			}
			public void ForceFinish() {
				if (running != null)
					NovelController._instance.StopCoroutine(running);
				running = null;

				if (architect != null) {
					architect.ForceFinish();

					//time to complete the entire dialogue string since an interrupted segment with events will only autocomplete to the next event.
					if (pretext == "")
						line.lastSegmentsWholeDialogue = "";

					//execute all actions that have not been made yet.
					string[] parts = dialogue.Split('[', ']');
					for (int i = 0; i < parts.Length; i++) {
						bool isOdd = i % 2 != 0;
						if (isOdd) {
							string e = parts[i];
							if (allCurrentlyExecutedEvents.Contains(e)) {
								allCurrentlyExecutedEvents.Remove(e);
							} else {
								LineEvents.HandleEvent(e, this);
							}
							i++;
						}
						//append only the dialogue to the whole dialogue and make the architect show it.
						line.lastSegmentsWholeDialogue += parts[i];
					}

					//show the whole dialogue on the architect.
					architect.ShowText(line.lastSegmentsWholeDialogue);
				}
			}
		}
	}
}
