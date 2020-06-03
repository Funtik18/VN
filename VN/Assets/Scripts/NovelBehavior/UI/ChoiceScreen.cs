using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceScreen : MonoBehaviour {
	public static ChoiceScreen _instance;

	public GameObject root;

	public TitleHeader header;


	public ChoiceButton choicePrefab;
	static List<ChoiceButton> choices = new List<ChoiceButton>();

	public CHOICE choice = new CHOICE();
	public static CHOICE lastChoiceMade { get { return _instance.choice; } }

	public static bool isWaitingForChoiceToBeMade { get { return isShowingChoices && !lastChoiceMade.hasBeenMade; } }

	public VerticalLayoutGroup layoutGroup;


	void Awake() {
		_instance = this;
		Hide();
	}

	public static void Show( string title, params string[] choices ) {
		_instance.root.SetActive(true);

		if (title != "")
			_instance.header.Show(title);
		else
			_instance.header.Hide();

		if (isShowingChoices)
			_instance.StopCoroutine(showingChoices);

		ClearAllCurrentChoices();

		showingChoices = _instance.StartCoroutine(ShowingChoices(choices));
	}

	public static void Hide() {
		if (isShowingChoices)
			_instance.StopCoroutine(showingChoices);
		showingChoices = null;

		_instance.header.Hide();

		ClearAllCurrentChoices();

		_instance.root.SetActive(false);
	}

	public static bool isShowingChoices { get { return showingChoices != null; } }
	static Coroutine showingChoices = null;
	public static IEnumerator ShowingChoices( string[] choices ) {
		yield return new WaitForEndOfFrame();//allow the header to begin appearing if it will be present.
		lastChoiceMade.Reset();

		while (_instance.header.isRevealing)
			yield return new WaitForEndOfFrame();

		for (int i = 0; i < choices.Length; i++) {
			CreateChoice(choices[i]);
		}

		SetLayoutSpacing();

		while (isWaitingForChoiceToBeMade)
			yield return new WaitForEndOfFrame();

		Hide();
	}
	static void ClearAllCurrentChoices() {
		foreach (ChoiceButton b in choices) {
			HelpFunctions.DestroyObject(b.gameObject);
		}
		choices.Clear();
	}


	[System.Serializable]
	public class CHOICE {
		public bool hasBeenMade { get { return title != "" && index != -1; } }

		public string title = "";
		public int index = -1;

		public void Reset() {
			title = "";
			index = -1;
		}
	}
	
	

	public void MakeChoice( ChoiceButton button ) {
		choice.index = button.choiceIndex;
		choice.title = button.text;
	}

	public void MakeChoice( string choiceTitle ) {
		foreach (ChoiceButton b in choices) {
			if (b.text.ToLower() == choiceTitle.ToLower()) {
				MakeChoice(b);
				return;
			}
		}
	}

	public void MakeChoice( int choiceIndex ) {
		if (choices.Count > choiceIndex)
			MakeChoice(choices[choiceIndex]);
	}

	static void CreateChoice( string choice ) {
		GameObject ob = Instantiate(_instance.choicePrefab.gameObject, _instance.choicePrefab.transform.parent);
		ob.SetActive(true);
		ChoiceButton b = ob.GetComponent<ChoiceButton>();

		b.text = choice;
		b.choiceIndex = choices.Count;

		choices.Add(b);
	}

	static void SetLayoutSpacing() {
		int i = choices.Count;
		if (i <= 3)
			_instance.layoutGroup.spacing = 20;
		else if (i >= 7)
			_instance.layoutGroup.spacing = 1;
		else {
			switch (i) {
				case 4:
				_instance.layoutGroup.spacing = 15;
				break;
				case 5:
				_instance.layoutGroup.spacing = 10;
				break;
				case 6:
				_instance.layoutGroup.spacing = 5;
				break;
			}
		}
	}
}
