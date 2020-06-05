using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonCallNovelUI : BasicButton {//Test

	public string currentFile = "story_2";

	private void Start() {
		main = GetComponent<Button>();

		main.onClick.AddListener(() => { CallUI();});
	}

	private void CallUI() {
		DialogueSystem._instance.Open();
		LayoutOrders._instance.MakeMainGUI();
		NovelController._instance.Prepare();
		NovelController._instance.LoadChapterFile(currentFile);
	}
}
