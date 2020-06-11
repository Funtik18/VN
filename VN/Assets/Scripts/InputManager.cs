using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {
	private static InputManager _instance;

	public KeyBindings keys;


	Action next, nextF, skip, stopskip, back, pref;
	Action openUI, closeUI;

	Interpreter interpreter;

	private InputManager() { }
	public static InputManager GetInstance() {
		if (_instance == null) return new InputManager();
		return _instance;
	}

	private void Start() {
		interpreter = Interpreter.GetInstance();


		DialogueSystem system = DialogueSystem._instance;
		DialogueSystem.ELEMENTS elements = system.elements;

		OptionSceneController optionController = OptionSceneController._instace;

		HandleAction action = HandleAction.GetInstance();

		next = action.Command_Next;
		nextF = HandleAction.Command_NextForce;
		skip = HandleAction.Command_Skip;
		stopskip = HandleAction.Command_StopSkip;
		back = action.Command_Back;
		pref = HandleAction.Command_PreferncesSetActive;

		openUI = action.Command_OpenUI;
		closeUI = action.Command_CloseUI;

		elements.speechButtons[0].GetComponent<Button>().onClick.AddListener(() => { back(); }); elements.speechButtons[0].SetActive(false);//back;
		elements.speechButtons[1].GetComponent<Button>().onClick.AddListener(() => { });//history
		elements.speechButtons[2].GetComponent<Toggle>().onValueChanged.AddListener(
			delegate{
				bool on = elements.speechButtons[2].GetComponent<Toggle>().isOn;
				if (on) {
					elements.speechButtons[2].GetComponent<Image>().color = Color.red;
					skip();
				} else {
					elements.speechButtons[2].GetComponent<Image>().color = Color.white;
					stopskip();
				}
			});//skip
		elements.speechButtons[3].GetComponent<Button>().onClick.AddListener(() => { });//auto
		elements.speechButtons[4].GetComponent<Button>().onClick.AddListener(() => { });//save
		elements.speechButtons[5].GetComponent<Button>().onClick.AddListener(() => { });//load
		elements.speechButtons[6].GetComponent<Button>().onClick.AddListener(() => { });//qsave
		elements.speechButtons[7].GetComponent<Button>().onClick.AddListener(() => { });//qload
		elements.speechButtons[8].GetComponent<Button>().onClick.AddListener(() => { pref(); });//preferences
		//nexts
		system.speechBox.GetComponent<Button>().onClick.AddListener(() => { next(); });
		elements.speechPanel.GetComponent<Button>().onClick.AddListener(() => { next(); });
		elements.speakerNamePanel.GetComponent<Button>().onClick.AddListener(() => { next(); });

		//options
		optionController.btnClose.onClick.AddListener(() => { pref(); });//close preferences
		optionController.btnComfirm.onClick.AddListener(() => { pref(); });
	}

	bool isMiddleMousePressed = false;
	bool showOnce = true;

	private void Update() {
		if (Input.GetKeyDown(keys.ESC)) {
			pref();
		}
		if (Input.GetKeyDown(keys.Next)) {
			if (!NovelController._instance.isEnd && NovelController._instance.canUse) {
				next();
			}
		}




		if(Input.GetAxis("Mouse ScrollWheel") == 0.1f) {
			//back();
		}


		if (Input.GetMouseButtonDown(1)) {
			MiddleMouse();
			
		}
		

		if (Input.GetAxis("Mouse ScrollWheel") == -0.1f) {
			next();
		}
	}

	void MiddleMouse() {
		isMiddleMousePressed = !isMiddleMousePressed;
		if (isMiddleMousePressed) {
			HandleAction.Command_Interrupt(true);
			closeUI();
		} else {
			HandleAction.Command_Interrupt(false);
			openUI();
		}
	}
}
