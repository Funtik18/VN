using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter {

	private static Interpreter _instance;

	List<string> data = new List<string>();

	public int chapterProgress = 0;//read lines
	public int lineProgress = 0;

	public bool fast = false;

	private Interpreter() {
		_instance = this;
	}

	public static Interpreter GetInstance() {
		if (_instance == null) return new Interpreter();
		return _instance;
	}

	/// <summary>
	/// Trigger that advances the progress through a chapter file.
	/// </summary>
	public bool next = false;

	public void StartReading( List<string> _data ) {
		data = _data;
		if (handlingChapterFile != null)
			NovelController._instance.StopCoroutine(handlingChapterFile);
		chapterProgress = 0;
		handlingChapterFile = NovelController._instance.StartCoroutine(HandlingChapterFile());
	}
	public void ContinueReading( int index) {
		if (handlingChapterFile != null)
			NovelController._instance.StopCoroutine(handlingChapterFile);
		chapterProgress = index;
		handlingChapterFile = NovelController._instance.StartCoroutine(HandlingChapterFile());
	}

	#region Handling chapter file
	public bool isHandlingChapterFile { get { return handlingChapterFile != null; } }
	public Coroutine handlingChapterFile = null;
	IEnumerator HandlingChapterFile() {


		while (chapterProgress < data.Count) {
			if (next) {//click next
				string line = data[chapterProgress];

				TagEvents.Inject(ref line);//inject data into the line where it may be needed.

				if (line.ToLower().StartsWith("choice")) {//choice
					yield return HandlingChoiceLine(line);
					chapterProgress++;
				} else if (line.StartsWith("input")) {//user input
													  //yield return HandlingInputLine(line);
													  //chapterProgress++;
				} else {//normal line of dialogue and actions.
					HandleLine(line);
					chapterProgress++;
					while (isHandlingLine) {
						yield return new WaitForEndOfFrame();
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		handlingChapterFile = null;
	}
	#region Handling line
	void HandleLine( string rawLine ) {
		LineArchitect.LINE line = LineArchitect.Interpret(rawLine);//прочтёная разсегментированная линия

		//now we need to handle the line.
		StopHandlingLine();
		handlingLine = NovelController._instance.StartCoroutine(HandlingLine(line));
	}
	public bool isHandlingLine { get { return handlingLine != null; } }
	public Coroutine handlingLine = null;
	IEnumerator HandlingLine( LineArchitect.LINE line ) {
		next = false;
		lineProgress = 0;

		while (lineProgress < line.segments.Count) {
			next = false;//reset at the start of each loop.
			LineArchitect.LINE.SEGMENT segment = line.segments[lineProgress];

			if (lineProgress > 0) {
				if (segment.trigger == LineArchitect.LINE.SEGMENT.TRIGGER.autoDelay) {
					for (float timer = segment.autoDelay; timer >= 0; timer -= Time.deltaTime) {
						yield return new WaitForEndOfFrame();
						if (next)
							break;//allow the termination of a delay when "next" is triggered. Prevents unskippable wait timers.
					}
				} else {
					while (!next)
						yield return new WaitForEndOfFrame();//wait until the player says move to the next segment.
				}
			}
			next = false;

			segment.Run();//the segment now needs to build and run.

			while (segment.isRunning) {
				yield return new WaitForEndOfFrame();
				if (next || fast) {////////////////////////////////////////
					if (!segment.architect.skip)//rapidly complete the text on first advance, force it to finish on the second.
						segment.architect.skip = true;
					else {

						segment.ForceFinish();
						//yield break;
					}
					next = false;
				}
			}
			lineProgress++;
			yield return new WaitForEndOfFrame();
		}


		for (int i = 0; i < line.actions.Count; i++) {//Handle all the actions set at the end of the line.
			NovelController._instance.HandleAction(line.actions[i]);
		}

		handlingLine = null;
		//Line is finished. 
	}
	void StopHandlingLine() {
		if (isHandlingLine)
			NovelController._instance.StopCoroutine(handlingLine);
		handlingLine = null;
	}
	#endregion
	#region Handling choice
	IEnumerator HandlingChoiceLine( string line ) {
		string title = line.Split('"')[1];
		List<string> choices = new List<string>();
		List<string> actions = new List<string>();

		bool gatheringChoices = true;
		while (gatheringChoices) {
			chapterProgress++;
			line = data[chapterProgress];

			if (line.Contains("{"))
				continue;

			while (line[0] == ' ' || line[0] == '\t') {
				line = line.Remove(0, 1);//tittle
			}

			if (!line.Contains("}")) {
				choices.Add(line.Split('"')[1]);//choice
				string temp = data[chapterProgress + 1];
				while (temp[0] == ' ' || temp[0] == '\t') {
					temp = temp.Remove(0, 1);
				}
				actions.Add(temp);//dialogue
				chapterProgress++;
			} else {
				gatheringChoices = false;
			}
		}


		if (choices.Count > 0) {
			ChoiceScreen.Show(title, choices.ToArray()); yield return new WaitForEndOfFrame();//display choices
			while (ChoiceScreen.isWaitingForChoiceToBeMade)
				yield return new WaitForEndOfFrame();

			//choice is made. execute the paired action.
			string actionLine = actions[ChoiceScreen.lastChoiceMade.index];
			HandleLine(actionLine);

			while (isHandlingLine)
				yield return new WaitForEndOfFrame();
		} else {
			Debug.LogError("Invalid choice operation. No choices were found.");
		}
	}
	#endregion
	#endregion
}
