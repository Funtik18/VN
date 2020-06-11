using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NovelController : MonoBehaviour {
    public static NovelController _instance;

	private Interpreter interpreter;
	private HandleAction handleAction;

	public bool isEnd = true;
	public bool canUse = false;

	private string activeChapterFile = "";

	/// <summary> The lines of data loaded directly from a chapter file.</summary>
	private List<string> data = new List<string>();

	/// <summary> Used as a fallback when no speaker is given.</summary>
	public string cachedLastSpeaker = "";

	void Awake() {
		_instance = this;

		interpreter = Interpreter.GetInstance();
		handleAction = HandleAction.GetInstance();
	}
	public void LoadChapterFile( string _fileName ) {
		activeChapterFile = _fileName;
		data = FileManager.ReadTextAsset(FileManager.GetFileTXT(_fileName));

		cachedLastSpeaker = "";
		
		interpreter.StartReading(data);

		//auto start the chapter.
		handleAction.Command_Next();

		waitingFile = StartCoroutine(WaitingFile());
	}

	public void ReadAction(string action) {
		handleAction.ReadAction(action);
	}

	public bool isWaitingFile { get { return waitingFile != null; } }
	Coroutine waitingFile = null;
	IEnumerator WaitingFile() {
		isEnd = false;
		canUse = true;
		//wait end file
		while (interpreter.isHandlingChapterFile) yield return null;

		canUse = false;
		isEnd = true;
		yield break;
	}
}
