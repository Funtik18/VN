using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverSystem : MonoBehaviour {
	public static CoverSystem _instance;

	/// <summary>The graphic layer in the background behind everything.</summary>
	public LAYER background = new LAYER();
	/// <summary>The graphic layer in front of the characters but behind the dialogue.</summary>
	public LAYER cinematic = new LAYER();
	/// <summary>the graphic layer above everything.</summary>
	public LAYER foreground = new LAYER();

	public static float transitionSpeed = 3f;

	void Awake() {
		_instance = this;
	}
}
[System.Serializable]
public class LAYER {
	public GameObject root;
}
