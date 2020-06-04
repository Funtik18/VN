using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NovelController : MonoBehaviour {
    public static NovelController _instance;

	string activeChapterFile = "";

	/// <summary> The lines of data loaded directly from a chapter file.</summary>
	List<string> data = new List<string>();

	/*[HideInInspector] */public int chapterProgress = 0;//line
	[SerializeField]int lineProgress = 0;

	/// <summary> Used as a fallback when no speaker is given.</summary>
	[HideInInspector] public string cachedLastSpeaker = "";

	void Awake() {
		_instance = this;
		GameObject[] nexts = GameObject.FindGameObjectsWithTag("NEXT");
		for(int i = 0; i < nexts.Length; i++) {
			nexts[i].GetComponent<Button>().onClick.AddListener(() => { Command_Next(); });
		}

		LoadChapterFile("story_2");
	}
	/// <summary>
	/// Trigger that advances the progress through a chapter file.
	/// </summary>
	bool next = false;

	public void LoadChapterFile( string _fileName ) {
		activeChapterFile = _fileName;
		data = FileManager.ReadTextAsset(FileManager.GetFileTXT(_fileName));

		cachedLastSpeaker = "";

		if (handlingChapterFile != null)
			StopCoroutine(handlingChapterFile);
		handlingChapterFile = StartCoroutine(HandlingChapterFile());

		//auto start the chapter.
		Command_Next();
	}
	#region Handling chapter file
	public bool isHandlingChapterFile { get { return handlingChapterFile != null; } }
	Coroutine handlingChapterFile = null;
	IEnumerator HandlingChapterFile() {
		chapterProgress = 0;

		while (chapterProgress < data.Count) {
			if (next) {//click next
				string line = data[chapterProgress];

				TagEvents.Inject(ref line);//inject data into the line where it may be needed.

				if (line.ToLower().StartsWith("choice")) {//choice
					yield return HandlingChoiceLine(line);
					chapterProgress++;
				}
				
				else if (line.StartsWith("input")) {//user input
					//yield return HandlingInputLine(line);
					//chapterProgress++;
				}else {//normal line of dialogue and actions.
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
		handlingLine = StartCoroutine(HandlingLine(line));
	}
	public bool isHandlingLine { get { return handlingLine != null; } }
	Coroutine handlingLine = null;
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
				if (next) {
					if (!segment.architect.skip)//rapidly complete the text on first advance, force it to finish on the second.
						segment.architect.skip = true;
					else
						segment.ForceFinish();
					next = false;
				}
			}
			lineProgress++;
			yield return new WaitForEndOfFrame();
		}

		
		for (int i = 0; i < line.actions.Count; i++) {//Handle all the actions set at the end of the line.
			HandleAction(line.actions[i]);
		}

		handlingLine = null;
		//Line is finished. 
	}
	void StopHandlingLine() {
		if (isHandlingLine)
			StopCoroutine(handlingLine);
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
				while (temp[0] == ' '|| temp[0] == '\t') {
					temp = temp.Remove(0,1);
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
	#region Handle action
	public void HandleAction( string action ) {
		Debug.Log("execute command - " + action);
		string[] data = action.Split('(', ')');
		switch (data[0].ToLower()) {
			case "next":
			Command_Next();
			break;

			case "enter":
			Command_Enter(data[1]);
			break;
			case "exit":
			Command_Exit(data[1]);
			break;

			case "f"://flip
			Command_Flip(data[1]);
			break;
			case "fl"://face left
			Command_FaceLeft(data[1]);
			break;
			case "fr"://face rigth
			Command_FaceRight(data[1]);
			break;

			case "move"://move
			Command_MoveCharacter(data[1]);
			break;
			case "setpos"://setposition
			Command_SetPosition(data[1]);
			break;

			/*case "savePlayerName":
			Command_SavePlayerName(InputScreen.currentInput);//saves the player name as what was last input by the player
			break;

			case "setBackground":
			Command_SetLayerImage(data[1], BCFC.instance.background);
			break;

			case "setCinematic":
			Command_SetLayerImage(data[1], BCFC.instance.cinematic);
			break;

			case "setForeground":
			Command_SetLayerImage(data[1], BCFC.instance.foreground);
			break;

			case "playSound":
			Command_PlaySound(data[1]);
			break;

			case "playMusic":
			Command_PlayMusic(data[1]);
			break;

			case "playAmbiance":
			Command_PlayAmbiance(data[1]);
			break;

			case "stopAmbiance":
			if (data[1] != "")
				Command_StopAmbiance(data[1]);
			else
				Command_StopAllAmbiance();
			break;

			

			case "setFace":
			Command_SetFace(data[1]);
			break;

			case "setBody":
			Command_SetBody(data[1]);
			break;

			

			case "transBackground":
			Command_TransLayer(BCFC.instance.background, data[1]);
			break;

			case "transCinematic":
			Command_TransLayer(BCFC.instance.cinematic, data[1]);
			break;

			case "transForeground":
			Command_TransLayer(BCFC.instance.foreground, data[1]);
			break;

			case "showScene":
			Command_ShowScene(data[1]);
			break;

			case "Load":
			Command_Load(data[1]);
			break;

			

			case "saveTempVal":
			Command_SaveTemporaryValue(data[1]);
			break;

			case "saveTempInput"://this takes only the index in the cache to save the value to.
			Command_SaveTemporaryValue(data[1] + "," + InputScreen.currentInput);
			break; */
		}
	}
	/// <summary>
	/// Procede to the next line of a chapter or finish the line right now.
	/// </summary>
	public void Command_Next() {
		next = true;
	}

	void Command_Enter( string data ) {
		string[] parameters = data.Split(',');
		string[] characters = parameters[0].Split(';');
		float speed = 3;
		bool smooth = false;
		for (int i = 1; i < parameters.Length; i++) {
			float fVal = 0; bool bVal = false;
			if (float.TryParse(parameters[i], out fVal)) { speed = fVal; continue; }
			if (bool.TryParse(parameters[i], out bVal)) { smooth = bVal; continue; }
		}

		for (int i = 0; i < characters.Length;i++) {
			Character character = CharacterManager._instance.GetCharacter(characters[i], true, false);
			character.enabled = true;
			character.FadeIn(speed, smooth);
		}
	}
	void Command_Exit( string data ) {
		string[] parameters = data.Split(',');
		string[] characters = parameters[0].Split(';');
		float speed = 3;
		bool smooth = false;
		for (int i = 1; i < parameters.Length; i++) {
			float fVal = 0; bool bVal = false;
			if (float.TryParse(parameters[i], out fVal)) { speed = fVal; continue; }
			if (bool.TryParse(parameters[i], out bVal)) { smooth = bVal; continue; }
		}

		for (int i = 0; i < characters.Length; i++) {
			Character c = CharacterManager._instance.GetCharacter(characters[i]);
			c.FadeOut(speed, smooth);
		}
	}

	void Command_Flip( string data ) {
		string[] characters = data.Split(';');

		for (int i = 0; i < characters.Length; i++) {
			Character c = CharacterManager._instance.GetCharacter(characters[i]);
			c.Flip();
		}
	}
	void Command_FaceLeft( string data ) {
		string[] characters = data.Split(';');

		for (int i = 0; i < characters.Length; i++) {
			Character c = CharacterManager._instance.GetCharacter(characters[i]);
			c.FaceLeft();
		}
	}
	void Command_FaceRight( string data ) {
		string[] characters = data.Split(';');

		for (int i = 0; i < characters.Length; i++) {
			Character c = CharacterManager._instance.GetCharacter(characters[i]);
			c.FaceRight();
		}
	}

	void Command_MoveCharacter( string data ) {
		string[] parameters = data.Split(',');
		string character = parameters[0];
		float locationX = float.Parse(parameters[1]);
		float locationY = parameters.Length >= 3 ? float.Parse(parameters[2]) : 0;
		float speed = parameters.Length >= 4 ? float.Parse(parameters[3]) : 7f;
		bool smooth = parameters.Length == 5 ? bool.Parse(parameters[4]) : true;

		Character c = CharacterManager._instance.GetCharacter(character);
		c.MoveTo(new Vector2(locationX, locationY), speed, smooth);
	}
	void Command_SetPosition( string data ) {
		string[] parameters = data.Split(',');
		string character = parameters[0];
		float locationX = float.Parse(parameters[1]);
		float locationY = parameters.Length == 3 ? float.Parse(parameters[2]) : 0;

		Character c = CharacterManager._instance.GetCharacter(character);
		c.SetPosition(new Vector2(locationX, locationY));

		//print("set " + c.characterName + " position to " + locationX + "," + locationY);
	}
	#endregion

	#endregion
}
