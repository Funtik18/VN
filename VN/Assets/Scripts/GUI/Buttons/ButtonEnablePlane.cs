using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnablePlane : BasicButton {

	void Awake() {
		main = GetComponent<Button>();
		main.onClick.AddListener(() => { HandleAction.Command_PreferncesSetActive(); });
	}

	
}
