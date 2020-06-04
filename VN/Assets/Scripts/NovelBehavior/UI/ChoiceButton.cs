using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChoiceButton : MonoBehaviour {

	public Button main;
	public TextMeshProUGUI tmpro;
	
	public string text { get { return tmpro.text; } set { tmpro.text = value; } }

	[HideInInspector]
	public int choiceIndex = -1;
}
