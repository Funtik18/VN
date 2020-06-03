using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelController : MonoBehaviour {
    public static NovelController _instance;

	string activeChapterFile = "";

	List<string> data;

	void Awake() {
		_instance = this;

		LoadChapterFile("story_1");
	}
	/// <summary>
	/// Trigger that advances the progress through a chapter file.
	/// </summary>
	bool next = false;
	/// <summary>
	/// Procede to the next line of a chapter or finish the line right now.
	/// </summary>
	public void Next() {
		next = true;
	}

	public void LoadChapterFile( string _fileName ) {
		activeChapterFile = _fileName;
		data = FileManager.ReadTextAsset(FileManager.GetFileTXT(_fileName));

		print(data[0]);


		/*
		cachedLastSpeaker = "";

		if (handlingChapterFile != null)
			StopCoroutine(handlingChapterFile);
		handlingChapterFile = StartCoroutine(HandlingChapterFile());*/

		//auto start the chapter.
		Next();
	}
}
