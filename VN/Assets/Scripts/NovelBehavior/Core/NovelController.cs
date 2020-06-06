using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NovelController : MonoBehaviour {
    public static NovelController _instance;

	Interpreter interpreter;

	string activeChapterFile = "";

	/// <summary> The lines of data loaded directly from a chapter file.</summary>
	List<string> data = new List<string>();

	/// <summary> Used as a fallback when no speaker is given.</summary>
	public string cachedLastSpeaker = "";

	void Awake() {
		_instance = this;
	}
	

	public void LoadChapterFile( string _fileName ) {
		activeChapterFile = _fileName;
		data = FileManager.ReadTextAsset(FileManager.GetFileTXT(_fileName));

		//cachedLastSpeaker = "";

		interpreter = Interpreter.GetInstance();

		interpreter.StartReading(data);


		if (waitingFile != null)
			StopCoroutine(waitingFile);
		waitingFile = StartCoroutine(WaitingFile());


		//auto start the chapter.
		Command_Next();

		
	}
	public void Prepare() {
		GameObject[] nexts = GameObject.FindGameObjectsWithTag("NEXT");
		for (int i = 0; i < nexts.Length; i++) {
			nexts[i].GetComponent<Button>().onClick.AddListener(() => { Command_Next();});
		}
	}

	public bool isWaitingFile { get { return waitingFile != null; } }
	Coroutine waitingFile = null;
	IEnumerator WaitingFile() {
		Debug.Log("Start - " + activeChapterFile);

		//wait end file
		while (interpreter.isHandlingChapterFile) yield return null;
		yield return new WaitForSeconds(1f);

		DialogueSystem._instance.Close();
		Debug.Log("End - " + activeChapterFile);



	}

	
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

			case "setBackground":
			//Command_SetLayerImage(data[1], BCFC.instance.background);
			break;
			case "setCinematic":
			//Command_SetLayerImage(data[1], BCFC.instance.cinematic);
			break;
			case "setForeground":
			//Command_SetLayerImage(data[1], BCFC.instance.foreground);
			break;

			/*case "savePlayerName":
			Command_SavePlayerName(InputScreen.currentInput);//saves the player name as what was last input by the player
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
		interpreter.next = true;
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
		float locationX = HelpFunctions.ConvertToFloat(parameters[1]);
		float locationY = parameters.Length >= 3 ? HelpFunctions.ConvertToFloat(parameters[2]) : 0;
		float speed = parameters.Length >= 4 ? HelpFunctions.ConvertToFloat(parameters[3]) : 7f;
		bool smooth = parameters.Length == 5 ? HelpFunctions.ConvertToBool(parameters[4]) : true;

		Character c = CharacterManager._instance.GetCharacter(character);
		c.MoveTo(new Vector2(locationX, locationY), speed, smooth);
	}
	void Command_SetPosition( string data ) {
		string[] parameters = data.Split(',');
		string character = parameters[0];
		float locationX = HelpFunctions.ConvertToFloat(parameters[1]);
		float locationY = parameters.Length == 3 ? HelpFunctions.ConvertToFloat(parameters[2]): 0;

		Character c = CharacterManager._instance.GetCharacter(character);
		c.SetPosition(new Vector2(locationX, locationY));

		Debug.Log("set " + c.characterName + " position to " + locationX + "," + locationY);
	}


	#endregion
	
}
