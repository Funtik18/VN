using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleHeader : MonoBehaviour {
	public Image banner;
	public TextMeshProUGUI titleText;
	public string title { get { return titleText.text; } set { titleText.text = value; } }

	public enum DISPLAY_METHOD {
		instant,
		slowFade,
		typeWriter,
		floatingSlowFade
	}
	public DISPLAY_METHOD displayMethod = DISPLAY_METHOD.instant;
	public float fadeSpeed = 1;

	public void Show( string displayTitle ) {
		title = displayTitle;

		if (isRevealing)
			StopCoroutine(revealing);

		if (!cachedBannerPos)
			cachedBannerOriginalPosition = banner.transform.position;

		revealing = StartCoroutine(Revealing());
	}

	public void Hide() {
		if (isRevealing)
			StopCoroutine(revealing);
		revealing = null;

		banner.enabled = false;
		titleText.enabled = false;

		if (cachedBannerPos)
			banner.transform.position = cachedBannerOriginalPosition;
	}


	public bool isRevealing { get { return revealing != null; } }
	Coroutine revealing = null;
	IEnumerator Revealing() {
		banner.enabled = true;
		titleText.enabled = true;
		//yield for the current display method.
		switch (displayMethod) {
			case DISPLAY_METHOD.instant:
			banner.color = HelpFunctions.SetAlpha(banner.color, 1);
			titleText.color = HelpFunctions.SetAlpha(titleText.color, 1);
			break;
			case DISPLAY_METHOD.slowFade:
			yield return SlowFade();
			break;
			case DISPLAY_METHOD.floatingSlowFade:
			yield return FloatingSlowFade();
			break;
			case DISPLAY_METHOD.typeWriter:
			yield return TypeWriter();
			break;
		}

		//title is displayed now.
		revealing = null;
	}

	IEnumerator SlowFade() {
		banner.color = HelpFunctions.SetAlpha(banner.color, 0);
		titleText.color = HelpFunctions.SetAlpha(titleText.color, 0);
		while (banner.color.a < 1) {
			banner.color = HelpFunctions.SetAlpha(banner.color, Mathf.MoveTowards(banner.color.a, 1, fadeSpeed * Time.unscaledDeltaTime));
			titleText.color = HelpFunctions.SetAlpha(titleText.color, banner.color.a);
			yield return new WaitForEndOfFrame();
		}
	}

	bool cachedBannerPos = false;
	Vector3 cachedBannerOriginalPosition = Vector3.zero;
	IEnumerator FloatingSlowFade() {
		banner.color = HelpFunctions.SetAlpha(banner.color, 0);
		titleText.color = HelpFunctions.SetAlpha(titleText.color, 0);

		float amount = 25f * ( (float)Screen.height / 720f );
		Vector3 downPos = new Vector3(0, amount, 0);
		banner.transform.position = cachedBannerOriginalPosition - downPos;

		while (banner.color.a < 1 || banner.transform.position != cachedBannerOriginalPosition) {
			banner.color = HelpFunctions.SetAlpha(banner.color, Mathf.MoveTowards(banner.color.a, 1, fadeSpeed * Time.unscaledDeltaTime));
			titleText.color = HelpFunctions.SetAlpha(titleText.color, banner.color.a);

			banner.transform.position = Vector3.MoveTowards(banner.transform.position, cachedBannerOriginalPosition, 11 * fadeSpeed * Time.unscaledDeltaTime);
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator TypeWriter() {
		banner.color = HelpFunctions.SetAlpha(banner.color, 1);
		titleText.color = HelpFunctions.SetAlpha(titleText.color, 1);
		TextArchitect architect = new TextArchitect(titleText, title);
		while (architect.isConstructing)
			yield return new WaitForEndOfFrame();
	}
}
