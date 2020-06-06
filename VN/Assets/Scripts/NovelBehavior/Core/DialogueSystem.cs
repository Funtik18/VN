using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour {

	public static DialogueSystem _instance;

	[HideInInspector]public string targetSpeech = "";

	
	TextArchitect textArchitect = null;
	[HideInInspector] public TextArchitect currentArchitect { 
		get { 
			return textArchitect;
		} 
	}

	public bool isClosed {
		get { return !speechBox.activeInHierarchy; }
	}

	void Awake() {
		_instance = this;

		interpreter = Interpreter.GetInstance();

		elements.speechButtons[0].onClick.AddListener(() => {
			
		});//back;
		elements.speechButtons[1].onClick.AddListener(() => { });
		elements.speechButtons[2].onClick.AddListener(() => { });
		elements.speechButtons[3].onClick.AddListener(() => { });
		elements.speechButtons[4].onClick.AddListener(() => { });
		elements.speechButtons[5].onClick.AddListener(() => { });
		elements.speechButtons[6].onClick.AddListener(() => { });
		elements.speechButtons[7].onClick.AddListener(() => { });
		elements.speechButtons[8].onClick.AddListener(() => { });

		//Close();
	}
	Interpreter interpreter;

	/// <summary>
	/// Say something and show it on the speech box.
	/// </summary>
	public void Say( string speech, string speaker = "", bool additive = false/*, CharacterDialogueDetails.CDD dialogueDetails = null*/ ) {
		StopSpeaking();

		if (additive)
			speechText.text = targetSpeech;

		speaking = StartCoroutine(Speaking(speech, additive, speaker/*, dialogueDetails*/));
	}
	Coroutine speaking = null;
	[HideInInspector] public bool isSpeaking { get { return speaking != null; } }
	[HideInInspector] public bool isWaitingForUserInput = false;
	IEnumerator Speaking( string speech, bool additive, string speaker = ""/*, CharacterDialogueDetails.CDD dialogueDetails = null*/ ) {
		speechPanel.SetActive(true);

		string additiveSpeech = additive ? speechText.text : "";
		targetSpeech = additiveSpeech + speech;

		//create a new architect the very first time. Any time other than that and we renew the architect.
		if (textArchitect == null)
			textArchitect = new TextArchitect(speechText, speech, additiveSpeech);
		else
			textArchitect.Renew(speech, additiveSpeech);

		speakerNameText.text = DetermineSpeaker(speaker);//
		speakerNamePanel.SetActive(speakerNameText.text != "");//

		//get or create a fresh set of dialogue details to make the dialogue system look a certain way for certain characters.
		/*string speakerValue = speakerNameText.text;
		if (speakerValue.Contains("<color="))
			speakerValue = speakerValue.Split('>')[1];
		if (dialogueDetails == null) dialogueDetails = new CharacterDialogueDetails.CDD(speakerValue);
		SetDialogueDetails(dialogueDetails);*/

		isWaitingForUserInput = false;

		if (isClosed) {
			VisibilityRequirements(true);
			VisibilitySpeech(true);
		}

		while (textArchitect.isConstructing) {
			if (Input.GetKey(KeyCode.Space))//////////////////
				textArchitect.skip = true;

			yield return new WaitForEndOfFrame();
		}

		//text finished
		isWaitingForUserInput = true;
		while (isWaitingForUserInput)
			yield return new WaitForEndOfFrame();

		StopSpeaking();
	}
	public void StopSpeaking() {
		if (isSpeaking) {
			StopCoroutine(speaking);
		}
		if (textArchitect != null && textArchitect.isConstructing) {
			textArchitect.Stop();
		}
		speaking = null;
	}



	/*void SetDialogueDetails( CharacterDialogueDetails.CDD cdd ) {
		//set the color of the name field.
		speakerNameText.text = cdd.nameColor + speakerNameText.text;

		//Set the font for this character.
		speechText.font = cdd.speechFont;
	}*/

	string DetermineSpeaker( string s ) {
		string retVal = speakerNameText.text;//default return is the current name
		if (s != speakerNameText.text && s != "")
			retVal = ( s.ToLower().Contains("narrator") ) ? "" : s;

		if (retVal.Contains("*"))
			retVal = retVal.Remove(0, 1);

		return retVal;
	}

	/// <summary>
	/// Close the entire speech panel. Stop all dialogue.
	/// </summary>
	public void Close() {
		StopSpeaking();

		VisibilityRequirements(false);
		VisibilitySpeech(false);
	}
	public void VisibilityButtons( bool trigger ) {
		for (int i = 0; i < elements.speechButtons.Length; i++) {
			elements.speechButtons[i].gameObject.SetActive(trigger);
		}
	}
	public void VisibilityRequirements( bool trigger ) {
		for (int i = 0; i < elements.requirementsObjects.Length; i++) {
			elements.requirementsObjects[i].SetActive(trigger);
		}
	}
	public void VisibilitySpeech( bool trigger ) {
		speechBox.SetActive(trigger);
		//speakerNamePanel.SetActive(trigger);
		//speechPanel.SetActive(trigger);
	}
	public void Open( string speakerName = "", string speech = "" ) {

		VisibilityRequirements(true);
		VisibilityButtons(true);
		VisibilitySpeech(true);
		/*if (speakerName == "" && speech == "") {
			VisibilitySpeech(false);
			VisibilityRequirements(false);
			return;
		}
		VisibilityRequirements(true);
		VisibilitySpeech(true);

		speakerNameText.text = speakerName;

		speakerNamePanel.SetActive(speakerName != "");

		speechText.text = speech;*/
	}


	[System.Serializable]
	public class ELEMENTS {
		public GameObject speakerNamePanel;
		public TextMeshProUGUI speakerNameText;

		public GameObject speechPanel;
		public TextMeshProUGUI speechText;

		public Button[] speechButtons;

		public GameObject[] requirementsObjects;
	}
	public GameObject speechBox;

	public ELEMENTS elements;

	public GameObject speakerNamePanel { get { return elements.speakerNamePanel; } }
	public TextMeshProUGUI speakerNameText { get { return elements.speakerNameText; } }
	
	public GameObject speechPanel { get { return elements.speechPanel; } }
	public TextMeshProUGUI speechText { get { return elements.speechText; } }
}
