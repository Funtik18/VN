using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Character {

	public Renderers renderers = new Renderers();

	public string characterName;
	public string displayName = "";

	public bool enabled {
		get {
			return root.gameObject.activeInHierarchy;
		}
		set {
			root.gameObject.SetActive(value);
		}
	}

	[HideInInspector] public RectTransform root;

	DialogueSystem dialogue;

	/// <summary>
	/// The space between the anchors of this character. Defines how much space a character takes up on the canvas.
	/// </summary>
	/// <value>The anchor padding.</value>
	public Vector2 anchorPadding { get { return root.anchorMax - root.anchorMin; } }

	public CanvasGroup canvasGroup;

	/// <summary>
	/// Create a new character.
	/// </summary>
	/// <param name="_name">Name.</param>
	public Character( string _name, bool enableOnStart = true ) {
		CharacterManager cm = CharacterManager._instance;

		GameObject prefab = FileManager.GetCharacter(_name);
		GameObject ob = GameObject.Instantiate(prefab, cm.characterPanel);

		root = ob.GetComponent<RectTransform>();
		canvasGroup = ob.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;

		characterName = _name;
		displayName = characterName;

		//get the renderer(s)
		renderers.bodyRenderer = ob.transform.Find("BodyLayer").GetComponentInChildren<Image>();
		renderers.expressionRenderer = ob.transform.Find("ExpressionLayer").GetComponentInChildren<Image>();
		renderers.allBodyRenderers.Add(renderers.bodyRenderer);
		renderers.allExpressionRenderers.Add(renderers.expressionRenderer);

		dialogue = DialogueSystem._instance;

		enabled = enableOnStart;
	}

	/// <summary>
	/// Make this character say something.
	/// </summary>
	/// <param name="speech">Speech.</param>
	public void Say( string speech, bool add = false ) {
		if (!enabled)
			enabled = true;

		//if (!isInScene)
			//FadeIn();

		//get the details for the dialogue system related to this character. details are saved on file.
		//CharacterDialogueDetails.CDD c = CharacterDialogueDetails.instance.GetDetailsForCharacter(characterName);
		Debug.Log(characterName);

		dialogue.Say(speech, displayName, add/*, c*/);
	}


	[System.Serializable]
	public class Renderers {
		/// <summary>
		/// The body renderer for a multi layer character.
		/// </summary>
		public Image bodyRenderer;
		/// <summary>
		/// The expression renderer for a multi layer character.
		/// </summary>
		public Image expressionRenderer;

		public List<Image> allBodyRenderers = new List<Image>();
		public List<Image> allExpressionRenderers = new List<Image>();
	}
}
