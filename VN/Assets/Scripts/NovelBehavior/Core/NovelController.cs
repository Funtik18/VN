using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelController : MonoBehaviour {
    public static NovelController _instance;

	string activeChapterFile = "";

	void Awake() {
		_instance = this;

		LoadChapterFile("story_1");
	}

	public void LoadChapterFile( string fileName ) {
		activeChapterFile = fileName;
		List<string> data = FileManager.ReadTextAsset(Resources.Load<TextAsset>($"Story/{activeChapterFile}"));

		print(data[0]);


		/*data = FileManager.ReadTextAsset(Resources.Load<TextAsset>($"Story/{fileName}"));
		cachedLastSpeaker = "";

		if (handlingChapterFile != null)
			StopCoroutine(handlingChapterFile);
		handlingChapterFile = StartCoroutine(HandlingChapterFile());

		//auto start the chapter.
		Next();*/
	}
}
