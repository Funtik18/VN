using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnablePlane : BasicButton {
	public CanvasGroup mainPanel;
	
	void Awake() {
		main = GetComponent<Button>();
		main.onClick.AddListener(() => { HelpFunctions.EnableCanvasGroup(mainPanel, !mainPanel.blocksRaycasts);print("+"); });
	}

	
}
