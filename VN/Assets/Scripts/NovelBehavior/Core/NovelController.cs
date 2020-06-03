using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NovelController : MonoBehaviour {
    public static NovelController _instance;

	string activeChapterFile = "";

	/// <summary> The lines of data loaded directly from a chapter file.	/// </summary>
	List<string> data = new List<string>();

	/*[HideInInspector] */public int chapterProgress = 0;//line
	[SerializeField]int lineProgress = 0;

	/// <summary> Used as a fallback when no speaker is given.</summary>
	[HideInInspector] public string cachedLastSpeaker = "";

	void Awake() {
		_instance = this;
		GameObject[] nexts = GameObject.FindGameObjectsWithTag("NEXT");
		for(int i = 0; i < nexts.Length; i++) {
			nexts[i].GetComponent<Button>().onClick.AddListener(() => { Next(); });
		}

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

		cachedLastSpeaker = "";

		if (handlingChapterFile != null)
			StopCoroutine(handlingChapterFile);
		handlingChapterFile = StartCoroutine(HandlingChapterFile());

		//auto start the chapter.
		Next();
	}
	#region Handling chapter file
	public bool isHandlingChapterFile { get { return handlingChapterFile != null; } }
	Coroutine handlingChapterFile = null;
	IEnumerator HandlingChapterFile() {
		chapterProgress = 0;

		while (chapterProgress < data.Count) {
			if (next) {//click next
				string line = data[chapterProgress];

				TagEvents.Inject(ref line);//inject data into the line where it may be needed.

				if (line.StartsWith("choice")) {//choice
					yield return HandlingChoiceLine(line);
					chapterProgress++;
				}
				
				else if (line.StartsWith("input")) {//user input
					//yield return HandlingInputLine(line);
					//chapterProgress++;
				}else {//normal line of dialogue and actions.
					HandleLine(line);
					chapterProgress++;
					while (isHandlingLine) {
						yield return new WaitForEndOfFrame();
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		handlingChapterFile = null;
	}
	#region Handling line
	void HandleLine( string rawLine ) {
		LineArchitect.LINE line = LineArchitect.Interpret(rawLine);//прочтёная разсегментированная линия

		//now we need to handle the line.
		StopHandlingLine();
		handlingLine = StartCoroutine(HandlingLine(line));
	}
	public bool isHandlingLine { get { return handlingLine != null; } }
	Coroutine handlingLine = null;
	IEnumerator HandlingLine( LineArchitect.LINE line ) {
		next = false;
		lineProgress = 0;

		while (lineProgress < line.segments.Count) {
			next = false;//reset at the start of each loop.
			LineArchitect.LINE.SEGMENT segment = line.segments[lineProgress];

			if (lineProgress > 0) {
				if (segment.trigger == LineArchitect.LINE.SEGMENT.TRIGGER.autoDelay) {
					for (float timer = segment.autoDelay; timer >= 0; timer -= Time.deltaTime) {
						yield return new WaitForEndOfFrame();
						if (next)
							break;//allow the termination of a delay when "next" is triggered. Prevents unskippable wait timers.
					}
				} else {
					while (!next)
						yield return new WaitForEndOfFrame();//wait until the player says move to the next segment.
				}
			}
			next = false;

			segment.Run();//the segment now needs to build and run.

			while (segment.isRunning) {
				yield return new WaitForEndOfFrame();
				if (next) {
					if (!segment.architect.skip)//rapidly complete the text on first advance, force it to finish on the second.
						segment.architect.skip = true;
					else
						segment.ForceFinish();
					next = false;
				}
			}
			lineProgress++;
			yield return new WaitForEndOfFrame();
		}

		
		for (int i = 0; i < line.actions.Count; i++) {//Handle all the actions set at the end of the line.
			HandleAction(line.actions[i]);
		}

		handlingLine = null;
		//Line is finished. 
	}
	void StopHandlingLine() {
		if (isHandlingLine)
			StopCoroutine(handlingLine);
		handlingLine = null;
	}
	#endregion
	#region Handling choice
	IEnumerator HandlingChoiceLine( string line ) {
		string title = line.Split('"')[1];
		List<string> choices = new List<string>();
		List<string> actions = new List<string>();

		bool gatheringChoices = true;
		while (gatheringChoices) {
			chapterProgress++;
			line = data[chapterProgress];

			if (line == "{")
				continue;

			line = line.Replace("    ", "");//remove the tabs that have become quad spaces.

			if (line != "}") {
				choices.Add(line.Split('"')[1]);
				actions.Add(data[chapterProgress + 1].Replace("    ", ""));
				chapterProgress++;
			} else {
				gatheringChoices = false;
			}
		}

		//display choices
		if (choices.Count > 0) {
			ChoiceScreen.Show(title, choices.ToArray()); yield return new WaitForEndOfFrame();
			while (ChoiceScreen.isWaitingForChoiceToBeMade)
				yield return new WaitForEndOfFrame();

			//choice is made. execute the paired action.
			string actionLine = actions[ChoiceScreen.lastChoiceMade.index];
			HandleLine(actionLine);

			while (isHandlingLine)
				yield return new WaitForEndOfFrame();
		} else {
			Debug.LogError("Invalid choice operation. No choices were found.");
		}
	}
	#endregion
	#region Handle action
	public void HandleAction( string action ) {
		print("execute command - " + action);
	}
	#endregion

	#endregion
}
