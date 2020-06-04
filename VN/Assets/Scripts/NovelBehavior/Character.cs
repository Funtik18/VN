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
	public bool isInScene = false;

	[HideInInspector] public RectTransform root;

	DialogueSystem dialogue;


	public Vector2 _targetPosition {
		get { return targetPosition; }
	}
	Vector2 targetPosition;
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

		if (!isInScene)
			FadeIn();

		//get the details for the dialogue system related to this character. details are saved on file.
		//CharacterDialogueDetails.CDD c = CharacterDialogueDetails.instance.GetDetailsForCharacter(characterName);
		//Debug.Log(characterName);

		dialogue.Say(speech, displayName, add/*, c*/);
	}

	#region Moving
	/// <summary>
	/// Immediately set the position of this character to the intended target.
	/// </summary>
	/// <param name="target">Target.</param>
	public void SetPosition( Vector2 target ) {
		targetPosition = target;

		Vector2 padding = anchorPadding;
		float maxX = 1f - padding.x;
		float maxY = 1f - padding.y;

		Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);

		root.anchorMin = minAnchorTarget;
		root.anchorMax = root.anchorMin + padding;
	}

	/// <summary>
	/// Move to a specific point relative to the canvas space. (1,1) = far top right, (0,0) = far bottom left, (0.5,0.5) = Middle.
	/// </summary>
	/// <param name="Target">Target.</param>
	/// <param name="speed">Speed.</param>
	/// <param name="smooth">If set to <c>true</c> smooth.</param>
	public void MoveTo( Vector2 Target, float speed, bool smooth = true ) {
		Debug.Log("move " + characterName + " to " + Target.ToString());
		StopMoving();
		//start moving coroutine.
		moving = CharacterManager._instance.StartCoroutine(Moving(Target, speed, smooth));
	}
	bool isMoving { get { return moving != null; } }
	Coroutine moving;
	/// <summary>
	/// The coroutine that runs to gradually move the character towards a position.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="speed">Speed.</param>
	/// <param name="smooth">If set to <c>true</c> smooth.</param>
	IEnumerator Moving( Vector2 target, float speed, bool smooth ) {
		targetPosition = target;

		//now we want to get the padding between the anchors of this character so we know what their min and max positions are.
		Vector2 padding = anchorPadding;

		//now get the limitations for 0 to 100% movement. The farthest a character can move to the right before reaching 100% should be the 1 value - the padding (thickness of the character) so that 100% to the right/up does not place them outside of the canvas.
		float maxX = 1f - padding.x;
		float maxY = 1f - padding.y;

		//now get the actual position target for the minimum anchors (left/bottom bounds) of the character. because maxX and maxY is just a percent reference.
		Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);
		speed *= Time.deltaTime;

		//move until we reach the target position.
		while (root.anchorMin != minAnchorTarget) {
			root.anchorMin = ( !smooth ) ? Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed) : Vector2.Lerp(root.anchorMin, minAnchorTarget, speed);
			root.anchorMax = root.anchorMin + padding;
			yield return new WaitForEndOfFrame();
		}

		StopMoving();
	}
	/// <summary>
	/// Stops the character in its tracks, either setting it immediately at the target position or not.
	/// </summary>
	/// <param name="arriveAtTargetPositionImmediately">If set to <c>true</c> arrive at target position immediately.</param>
	public void StopMoving( bool arriveAtTargetPositionImmediately = false ) {
		if (isMoving) {
			CharacterManager._instance.StopCoroutine(moving);
			if (arriveAtTargetPositionImmediately)
				SetPosition(targetPosition);
		}
		moving = null;
	}
	#endregion
	#region Transitioning images
	#region Transition body
	bool isTransitioningBody { get { return transitioningBody != null; } }
	Coroutine transitioningBody = null;
	public void TransitionBody( Sprite sprite, float speed, bool smooth ) {
		StopTransitioningBody();
		transitioningBody = CharacterManager._instance.StartCoroutine(TransitioningBody(sprite, speed, smooth));
	}
	public IEnumerator TransitioningBody( Sprite sprite, float speed, bool smooth ) {
		for (int i = 0; i < renderers.allBodyRenderers.Count; i++) {
			Image image = renderers.allBodyRenderers[i];
			if (image.sprite == sprite) {
				renderers.bodyRenderer = image;
				break;
			}
		}

		if (renderers.bodyRenderer.sprite != sprite) {
			Image image = GameObject.Instantiate(renderers.bodyRenderer.gameObject, renderers.bodyRenderer.transform.parent).GetComponent<Image>();
			renderers.allBodyRenderers.Add(image);
			renderers.bodyRenderer = image;
			image.color = HelpFunctions.SetAlpha(image.color, 0f);
			image.sprite = sprite;
		}

		while (HelpFunctions.TransitionImages(ref renderers.bodyRenderer, ref renderers.allBodyRenderers, speed, smooth, true))
			yield return new WaitForEndOfFrame();

		StopTransitioningBody();
	}
	void StopTransitioningBody() {
		if (isTransitioningBody)
			CharacterManager._instance.StopCoroutine(transitioningBody);
		transitioningBody = null;
	}
	#endregion
	#region Transition expression
	public void TransitionExpression( Sprite sprite, float speed, bool smooth ) {
		StopTransitioningExpression();
		transitioningExpression = CharacterManager._instance.StartCoroutine(TransitioningExpression(sprite, speed, smooth));
	}
	bool isTransitioningExpression { get { return transitioningExpression != null; } }
	Coroutine transitioningExpression = null;
	public IEnumerator TransitioningExpression( Sprite sprite, float speed, bool smooth ) {
		for (int i = 0; i < renderers.allExpressionRenderers.Count; i++) {
			Image image = renderers.allExpressionRenderers[i];
			if (image.sprite == sprite) {
				renderers.expressionRenderer = image;
				break;
			}
		}

		if (renderers.expressionRenderer.sprite != sprite) {
			Image image = GameObject.Instantiate(renderers.expressionRenderer.gameObject, renderers.expressionRenderer.transform.parent).GetComponent<Image>();
			renderers.allExpressionRenderers.Add(image);
			renderers.expressionRenderer = image;
			image.color = HelpFunctions.SetAlpha(image.color, 0f);
			image.sprite = sprite;
		}

		while (HelpFunctions.TransitionImages(ref renderers.expressionRenderer, ref renderers.allExpressionRenderers, speed, smooth, true))
			yield return new WaitForEndOfFrame();

		StopTransitioningExpression();
	}
	void StopTransitioningExpression() {
		if (isTransitioningExpression)
			CharacterManager._instance.StopCoroutine(transitioningExpression);
		transitioningExpression = null;
	}
	#endregion
	#region Transition fade
	public void FadeOut( float speed = 3, bool smooth = false ) {
		if (isEnteringOrExitingScene)
			CharacterManager._instance.StopCoroutine(enteringExiting);

		enteringExiting = CharacterManager._instance.StartCoroutine(ExitingScene(speed, smooth));
	}
	Coroutine enteringExiting = null;
	public bool isEnteringOrExitingScene { get { return enteringExiting != null; } }

	IEnumerator EnteringScene( float speed = 3, bool smooth = false ) {
		isInScene = true;

		while (canvasGroup.alpha < 1) {
			canvasGroup.alpha = smooth ? Mathf.Lerp(canvasGroup.alpha, 1, speed * Time.deltaTime) : Mathf.MoveTowards(canvasGroup.alpha, 1, speed * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}

		enteringExiting = null;
	}

	IEnumerator ExitingScene( float speed = 3, bool smooth = false ) {
		isInScene = false;

		while (canvasGroup.alpha > 0) {
			canvasGroup.alpha = smooth ? Mathf.Lerp(canvasGroup.alpha, 0, speed * Time.deltaTime) : Mathf.MoveTowards(canvasGroup.alpha, 0, speed * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}

		enteringExiting = null;

		//character is completely faded out and exited the scene. Destroy it so it is no longer saved to file until recalled.
		CharacterManager._instance.DestroyCharacter(this);
	}

	Sprite lastBodySprite, lastFacialSprite = null;
	public void FadeIn( float speed = 3, bool smooth = false ) {
		if (isEnteringOrExitingScene)
			CharacterManager._instance.StopCoroutine(enteringExiting);

		enteringExiting = CharacterManager._instance.StartCoroutine(EnteringScene(speed, smooth));
	}
	#endregion
	#region Transition flip
	public bool isFacingLeft { get { return root.localScale.x == 1; } }
	public bool isFacingRight { get { return root.localScale.y == -1; } }
	public void Flip() {
		root.localScale = new Vector3(root.localScale.x * -1, 1, 1);
	}
	public void FaceLeft() {
		root.localScale = Vector3.one;
	}
	public void FaceRight() {
		root.localScale = new Vector3(-1, 1, 1);
	}
	#endregion
	public Sprite GetSprite( int index = 0 ) {
		Sprite[] sprites = FileManager.GetSprites(characterName);
		return sprites[index];
	}
	public Sprite GetSprite( string spriteName = "" ) {
		Sprite[] sprites = FileManager.GetSprites(characterName);
		for (int i = 0; i < sprites.Length; i++) {
			if (sprites[i].name == spriteName)
				return sprites[i];
		}
		return sprites.Length > 0 ? sprites[0] : null;
	}

	public void SetBody( int index ) {
		renderers.bodyRenderer.sprite = GetSprite(index);
	}
	public void SetBody( Sprite sprite ) {
		renderers.bodyRenderer.sprite = sprite;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="spriteName"></param>
	public void SetBody( string spriteName ) {
		if (spriteName == "AlphaOnly")
			SetBody(Resources.Load<Sprite>("Images/AlphaOnly"));
		else
			renderers.bodyRenderer.sprite = GetSprite(spriteName);
	}


	public void SetExpression( int index ) {
		renderers.expressionRenderer.sprite = GetSprite(index);
	}
	public void SetExpression( Sprite sprite ) {
		renderers.expressionRenderer.sprite = sprite;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="spriteName"></param>
	public void SetExpression( string spriteName ) {
		if (spriteName == "AlphaOnly")
			SetExpression(Resources.Load<Sprite>("Images/AlphaOnly"));
		else
			renderers.expressionRenderer.sprite = GetSprite(spriteName);
	}



	#endregion

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
