using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAction {
    private static HandleAction _instance;
	private Interpreter interpreter;

	private HandleAction() {
		_instance = this;
		interpreter = Interpreter.GetInstance();
	}
	public static HandleAction GetInstance() {
		if (_instance == null) return new HandleAction();
		return _instance;
	}


	#region Handle action
	public void ReadAction( string action ) {
		string[] data = action.Split('(', ')');
		switch (data[0].ToLower()) {
			case "next":
			Command_Next();
			break;
			case "back":
			Command_Back();
			break;

			case "open"://show scene
			Command_ShowScene();
			break;
			case "close"://close scene
			Command_CloseScene();
			break;

			case "enter"://enter character(s)
			Command_Enter(data[1]);
			break;
			case "exit":// exit character(s)
			Command_Exit(data[1]);
			break;

			case "f"://flip character(s)
			Command_Flip(data[1]);
			break;
			case "fl"://face left character(s)
			Command_FaceLeft(data[1]);
			break;
			case "fr"://face rigth character(s)
			Command_FaceRight(data[1]);
			break;

			case "move"://move character(s)
			Command_MoveCharacter(data[1]);
			break;
			case "setpos"://setposition character(s)
			Command_SetPosition(data[1]);
			break;

			case "setback"://setBackground
								 //Command_SetLayerImage(data[1], BCFC.instance.background);
			break;
			case "setcin"://setCinematic
								//Command_SetLayerImage(data[1], BCFC.instance.cinematic);
			break;
			case "setfore"://setForeground
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
		interpreter.Next();
	}
	public void Command_Back() {
		//interpreter.Back();
	}

	public void Command_ShowScene() {

	}
	public void Command_CloseScene() {
		Command_OutAllCharacters();



		DialogueSystem._instance.Close();
	}

	/// <summary>
	/// Make Character fade in.
	/// </summary>
	/// <param name="data">"Raelin;Avira,15,true". Characters speed and smooth. Default "character, 3, false"</param>
	void Command_Enter( string data ) {
		string[] parameters={""}, charactersNames = {""};
		float speed = 3f;
		bool smooth = false;

		if (data.Contains(",")) {
			parameters = data.Split(',');
			charactersNames = parameters[0].Split(';');

			string isBool = parameters[1].ToLower();
			if (isBool == "true" || isBool == "false") {
				smooth = HelpFunctions.ConvertToBool(parameters[1]);
				if(parameters.Length==3) speed = HelpFunctions.ConvertToFloat(parameters[2]);
			} else {
				speed = HelpFunctions.ConvertToFloat(parameters[1]);
				if (parameters.Length == 3) smooth = HelpFunctions.ConvertToBool(parameters[2]);
			}
			
		} else {
			charactersNames = data.Split(';');
		}
		
		for (int i = 0; i < charactersNames.Length; i++) {
			string characterName = charactersNames[i];
			if (parameters.Length > 1) {
				Character character = CharacterManager._instance.GetCharacter(characterName, true);
				character.FadeIn(speed, smooth);
			} else {
				Character character = CharacterManager._instance.GetCharacter(characterName, true, false);
			}
		}
	}
	/// <summary>
	/// Make Character fade out.
	/// </summary>
	/// <param name="data">"Raelin;Avira,15,true". Characters speed and smooth. Default "character, 3, false"</param>
	void Command_Exit( string data ) {
		string[] parameters = { "" }, charactersNames = { "" };
		float speed = 3f;
		bool smooth = false;

		if (data.Contains(",")) {
			parameters = data.Split(',');
			charactersNames = parameters[0].Split(';');

			string isBool = parameters[1].ToLower();
			if (isBool == "true" || isBool == "false") {
				smooth = HelpFunctions.ConvertToBool(parameters[1]);
				if (parameters.Length == 3) speed = HelpFunctions.ConvertToFloat(parameters[2]);
			} else {
				speed = HelpFunctions.ConvertToFloat(parameters[1]);
				if (parameters.Length == 3) smooth = HelpFunctions.ConvertToBool(parameters[2]);
			}

		} else {
			charactersNames = data.Split(';');
		}

		for (int i = 0; i < charactersNames.Length; i++) {
			Character character = CharacterManager._instance.GetCharacter(charactersNames[i]);
			if (parameters.Length > 1) {
				character.FadeOut(speed, smooth);
			} else {
				character.FadeOut();
			}
		}
	}

	/// <summary>
	/// Flip Character.
	/// </summary>
	/// <param name="data"></param>
	void Command_Flip( string data ) {
		string[] characters = data.Split(';');

		for (int i = 0; i < characters.Length; i++) {
			Character c = CharacterManager._instance.GetCharacter(characters[i]);
			c.Flip();
		}
	}
	/// <summary>
	/// Flip Character left.
	/// </summary>
	/// <param name="data"></param>
	void Command_FaceLeft( string data ) {
		string[] characters = data.Split(';');

		for (int i = 0; i < characters.Length; i++) {
			Character c = CharacterManager._instance.GetCharacter(characters[i]);
			c.FaceLeft();
		}
	}
	/// <summary>
	/// Flip Character right
	/// </summary>
	/// <param name="data"></param>
	void Command_FaceRight( string data ) {
		string[] characters = data.Split(';');

		for (int i = 0; i < characters.Length; i++) {
			Character c = CharacterManager._instance.GetCharacter(characters[i]);
			c.FaceRight();
		}
	}

	void Command_MoveCharacter( string data ) {
		string[] parameters = { "" }, charactersNames = { "" };
		Vector2 location = new Vector2(0.5f, 0.5f);
		float speed = 7f;
		bool smooth = true;
		
		if (data.Contains(",")) {
			parameters = data.Split(',');//(имя;имя,0.5,1,true)
			charactersNames = parameters[0].Split(';');

			smooth = parameters.Length == 5 ? HelpFunctions.ConvertToBool(parameters[4]) : smooth;
			speed = parameters.Length >= 4 ? HelpFunctions.ConvertToFloat(parameters[3]) : speed;
			location = parameters.Length >= 3 ? new Vector2(HelpFunctions.ConvertToFloat(parameters[1]), HelpFunctions.ConvertToFloat(parameters[2])) : location;

			if (parameters.Length == 2) {
				location = CharacterManager.GetPositionByString(parameters[1].ToLower());
				if (location.x == -1) {
					float xy = HelpFunctions.ConvertToFloat(parameters[1]);
					location = new Vector2(xy, xy);
				}

			}

		} else {
			charactersNames = data.Split(';');
		}

		for (int i = 0; i < charactersNames.Length; i++) {
			string characterName = charactersNames[i];
			Character character = CharacterManager._instance.GetCharacter(charactersNames[i]);
			character.MoveTo(location,speed, smooth);
		}
	}
	/// <summary>
	/// Set Character(s) position
	/// </summary>
	/// <param name="data"></param>
	void Command_SetPosition( string data ) {
		string[] parameters = { "" }, charactersNames = { "" };
		Vector2 location = new Vector2(0.5f, 0.5f);

		if (data.Contains(",")) {
			parameters = data.Split(',');//(имя;имя,0.5)
			charactersNames = parameters[0].Split(';');

			if (parameters.Length == 2) {
				location = CharacterManager.GetPositionByString(parameters[1].ToLower());
				if(location.x == -1) {
					float xy = HelpFunctions.ConvertToFloat(parameters[1]);
					location = new Vector2(xy, xy);
				}
				
			} else if(parameters.Length == 3) {
				location = new Vector2(HelpFunctions.ConvertToFloat(parameters[1]), HelpFunctions.ConvertToFloat(parameters[2]));
			}
		} else {//только имена
			charactersNames = data.Split(';');
		}

		for (int i = 0; i < charactersNames.Length; i++) {
			string characterName = charactersNames[i];
			if (parameters.Length > 1) {
				Character character = CharacterManager._instance.GetCharacter(charactersNames[i]);
				character.SetPosition(location);
			} else {
				Character character = CharacterManager._instance.GetCharacter(charactersNames[i]);
				character.SetPosition(location);
			}
		}
	}


	public void Command_OpenUI() {
		DialogueSystem._instance.Open();
	}
	public void Command_CloseUI() {
		DialogueSystem._instance.Close();
	}

	#region System commands
	public static void Command_NextForce() {
		//_instance.interpreter.NextForce();
	}

	public static void Command_Interrupt(bool t) {
		if(t) _instance.interpreter.Interrupt();
		else _instance.interpreter.Continue();

	}

	public static void Command_Skip() {
		_instance.interpreter.StartSkiping();
	}

	public static void Command_StopSkip() {
		_instance.interpreter.StopSkiping();
	}

	public static void Command_OutCharacter( string name ) {
		List<Character> characters = CharacterManager._instance.characters;

		for (int i = 0; i < characters.Count; i++) {
			if (characters[i].characterName == name) {
				characters[i].FadeOut();
			}
		}
	}

	public static void Command_OutAllCharacters() {
		List<Character> characters = CharacterManager._instance.characters;
		int i = 0;
		while (characters.Count > i) {
			characters[i].FadeOut();
			i++;
		}
		CharacterManager._instance.DestroyCharacters();
	}



	public static void Command_PreferncesSetActive() {
		CanvasGroup prefernces = OptionSceneController._instace.GetComponent<CanvasGroup>();
		HelpFunctions.EnableCanvasGroup(prefernces, !prefernces.blocksRaycasts);
	}
	#endregion
	
	#endregion
}
