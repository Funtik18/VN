using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour {

	public static DialogueSystem _instance;

	

	void Awake() {
		_instance = this;
	}

	[System.Serializable]
	public class ELEMENTS {
		public GameObject speakerNamePanel;
		public TextMeshProUGUI speakerNameText;

		public GameObject speechPanel;
		public TextMeshProUGUI speechText;
	}
	public GameObject speechBox;

	public ELEMENTS elements;

	public GameObject[] SpeechPanelRequirements;


	public GameObject speakerNamePanel { get { return elements.speakerNamePanel; } }
	public TextMeshProUGUI speakerNameText { get { return elements.speakerNameText; } }
	
	public GameObject speechPanel { get { return elements.speechPanel; } }
	public TextMeshProUGUI speechText { get { return elements.speechText; } }
}
