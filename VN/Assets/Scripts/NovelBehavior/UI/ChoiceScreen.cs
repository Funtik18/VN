using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceScreen : MonoBehaviour {
	public static ChoiceScreen _instance;

	public GameObject root;

	public TitleHeader header;

	public Transform choicePanel;
	public ChoiceButton choicePrefab;
	static List<ChoiceButton> choices = new List<ChoiceButton>();

	public CHOICE choice = new CHOICE();
	public static CHOICE lastChoiceMade { get { return _instance.choice; } }

	public static bool isWaitingForChoiceToBeMade { get { return isShowingChoices && !lastChoiceMade.hasBeenMade; } }

	public GridLayoutGroup layoutGroup;


	void Awake() {
		_instance = this;
		Hide();
	}

	public static void Show( string title, params string[] choices ) {
		_instance.root.SetActive(true);

		if (title == "") _instance.header.Hide();
		else _instance.header.Show(title);

		if (isShowingChoices)
			_instance.StopCoroutine(_instance.showingChoices);

		_instance.ClearAllCurrentChoices();

		_instance.showingChoices = _instance.StartCoroutine(_instance.ShowingChoices(choices));
	}
	public static void Hide() {
		if (isShowingChoices)
			_instance.StopCoroutine(_instance.showingChoices);
		_instance.showingChoices = null;

		_instance.header.Hide();

		_instance.ClearAllCurrentChoices();

		_instance.root.SetActive(false);
	}


	public static bool isShowingChoices { get { return _instance.showingChoices != null; } }
	Coroutine showingChoices = null;
	public IEnumerator ShowingChoices( string[] choices ) {
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
	void ClearAllCurrentChoices() {
		foreach (ChoiceButton b in choices) {
			HelpFunctions.DestroyObject(b.gameObject);
		}
		choices.Clear();
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

	void CreateChoice( string choice ) {
		ChoiceButton ob = Instantiate<ChoiceButton>(choicePrefab, choicePanel);
		ob.gameObject.SetActive(true);
		ChoiceButton choicebutton = ob.GetComponent<ChoiceButton>();


		choicebutton.text = choice;
		choicebutton.choiceIndex = choices.Count;

		choicebutton.main.onClick.AddListener(() => { _instance.MakeChoice(choicebutton); });

		choices.Add(choicebutton);
	}

	void SetLayoutSpacing() {
		int count = choices.Count;

		float space = 20f;//spacing beetwen
		float offset = 150f;//width-offset

		float width = choicePanel.GetComponent<RectTransform>().rect.width;
		float height = choicePanel.GetComponent<RectTransform>().rect.height;

		if(( height / count )-(count*space) < 100) {
			layoutGroup.cellSize = new Vector2(width - offset, ( height / count ) - (  space ));
			layoutGroup.spacing = new Vector2(0, space);
		} else {
			layoutGroup.cellSize = new Vector2(width - offset, 100);
			layoutGroup.spacing = new Vector2(0, space);
		}
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
}
