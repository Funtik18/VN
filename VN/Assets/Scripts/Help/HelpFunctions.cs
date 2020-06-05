using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class HelpFunctions : MonoBehaviour {

	public static void EnableCanvasGroup(CanvasGroup element, bool triger) {
		element.alpha = ConvertToInteger<bool>(triger);
		element.blocksRaycasts = triger;
		element.interactable = triger;
	}
	public static void EnableCanvasGroup( CanvasGroup []elements, bool triger ) {
		for(int i = 0; i < elements.Length; i++) {
			EnableCanvasGroup(elements[i], triger);
		}
	}







	/// <summary>
	/// Converts an array of strings into a list of the same values.
	/// </summary>
	/// <returns>The to list.</returns>
	/// <param name="array">Array.</param>
	/// <param name="removeBlankLines">If set to <c>true</c> remove blank lines.</param>
	public static List<string> ArrayToList( string[] array, bool removeBlankLines = true ) {
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++) {
			string s = array[i];
			if (s.Length > 0 || !removeBlankLines) {
				list.Add(s);
			}
		}
		return list;
	}

	public static List<T> GetCopy<T>( List<T> _original ) {
		List<T> copy = new List<T>();
		for (int i = 0; i < _original.Count; i++) {
			copy.Add(_original[i]);
		}
		return copy;
	}


	public static void DestroyChilds( Transform _transform ) {
		GameObject[] trashArray = new GameObject[_transform.childCount];

		for (int i = 0; i < trashArray.Length; i++) {
			trashArray[i] = _transform.GetChild(i).gameObject;
		}

		for (int i = 0; i < trashArray.Length; i++) {
			DestroyImmediate(trashArray[i]);
		}
	}
	public static void DestroyObject( GameObject _obj ) {
		DestroyImmediate(_obj);
	}

	public static Transform[] TakeAllChilds( Transform _transform ) {
		Transform[] childs = new Transform[_transform.childCount];
		for (int i = 0; i < _transform.childCount; i++) {
			childs[i] = _transform.GetChild(i);
		}
		return childs;
	}

	public static bool TransitionImages( ref Image activeImage, ref List<Image> allImages, float speed, bool smooth, bool fasterInTime = false ) {
		bool anyValueChanged = false;

		speed *= Time.deltaTime;
		for (int i = allImages.Count - 1; i >= 0; i--) {
			Image image = allImages[i];
			if (image == activeImage) {
				if (image.color.a < 1f) {
					float spd = fasterInTime ? speed * 2 : speed;
					image.color = SetAlpha(image.color, smooth ? Mathf.Lerp(image.color.a, 1f, spd) : Mathf.MoveTowards(image.color.a, 1f, spd));
					anyValueChanged = true;
				}
			} else {
				if (image.color.a > 0) {
					image.color = SetAlpha(image.color, smooth ? Mathf.Lerp(image.color.a, 0f, speed) : Mathf.MoveTowards(image.color.a, 0f, speed));
					anyValueChanged = true;
				} else {
					allImages.RemoveAt(i);
					DestroyImmediate(image.gameObject);
					continue;
				}
			}
		}

		return anyValueChanged;
	}

	public static bool TransitionRawImages( ref RawImage activeImage, ref List<RawImage> allImages, float speed, bool smooth ) {
		bool anyValueChanged = false;

		speed *= Time.deltaTime;
		for (int i = allImages.Count - 1; i >= 0; i--) {
			RawImage image = allImages[i];
			if (image == activeImage) {
				if (image.color.a < 1f) {
					image.color = SetAlpha(image.color, smooth ? Mathf.Lerp(image.color.a, 1f, speed) : Mathf.MoveTowards(image.color.a, 1f, speed));
					anyValueChanged = true;
				}
			} else {
				if (image.color.a > 0) {
					image.color = SetAlpha(image.color, smooth ? Mathf.Lerp(image.color.a, 0f, speed) : Mathf.MoveTowards(image.color.a, 0f, speed));
					anyValueChanged = true;
				} else {
					//MovieTexture mov = image.texture as MovieTexture;
					//if (mov != null)
						//mov.Stop();

					allImages.RemoveAt(i);
					DestroyImmediate(image.gameObject);
					continue;
				}
			}
		}

		return anyValueChanged;
	}

	public static string[] SplitString( string _str, string _separator ) {
		char[] separators = _separator.ToCharArray();
		string[] parts = _str.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
		return parts;
	}
	public static string[] SplitString( string _str, char _separator ) {
		string[] parts = _str.Split(_separator);
		return parts;
	}


	public static Color SetAlpha( Color color, float alpha ) {
		return new Color(color.r, color.g, color.b, alpha);
	}

	public static int ConvertToInteger<T>( T number ) {
		return Convert.ToInt32(number);
	}
	public static float ConvertToFloat(string number) {
		return float.Parse(number, CultureInfo.InvariantCulture.NumberFormat);
	}
	public static bool ConvertToBool( string number ) {
		return Convert.ToBoolean(number);
	}

	public static string DeleteAllSpaces(string str) {
		return Regex.Replace(str, @"\s+", "");
	}
}
