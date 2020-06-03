using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceButton : MonoBehaviour {
	public TextMeshProUGUI tmpro;
	public string text { get { return tmpro.text; } set { tmpro.text = value; } }

	[HideInInspector]
	public int choiceIndex = -1;
}
