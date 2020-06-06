using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Carousel : MonoBehaviour {
    public TextMeshProUGUI carouselText;

    public Button left;
    public Button right;
	[SerializeField]
	private int Index = 0;
	public int value {
		get {
			return Index;
		} set {
			if (value < 0) value = strings.Count - 1;
			Index = value % ( strings.Count );

			UpdateChanges();
		}
	}

	private List<string> strings = new List<string>();

	private void Awake() {
		left.onClick.AddListener(() => { Previos();});
		right.onClick.AddListener(() => { Next();});
	}
	public void AddOptions( List<string> options) {
		strings = new List<string>(options);
	}

	private void UpdateChanges() {
		carouselText.text = strings[value];
	}

	private void Next() {
		value++;
	}
	private void Previos() {
		value--;
	}
}
