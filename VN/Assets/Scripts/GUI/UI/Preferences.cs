using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Preferences : MonoBehaviour {

    [SerializeField] public TextMeshProUGUI textResolution;
    
    [SerializeField] public Button btnNext;
    [SerializeField] public Button btnPrev;

    [SerializeField] public Toggle window;
    [SerializeField] public Toggle fullscreen;

    [SerializeField] public Button btnComfirm;
    [SerializeField] public Button btnReject;


    private Resolution[] resolutions;

    private int currentResolutionIndex = 0;


	private void Awake() {

       // FileManager.CreateFileSettings();

       /* btnComfirm.onClick.AddListener(() => { ComfirmResolution(); });
        btnReject.onClick.AddListener(() => { });
        btnNext.onClick.AddListener(() => { NextResolution(); });
        btnPrev.onClick.AddListener(() => { PreviosResolution(); });

        resolutions = Screen.resolutions;

        List<Resolution> temp = resolutions.ToList();
        int index = 0;
		for (int i = 0; i < temp.Count; i++) {
            if(temp[i].width == Screen.currentResolution.width && temp[i].height == Screen.currentResolution.height && temp[i].refreshRate == Screen.currentResolution.refreshRate) {
                index = i;
                break;
            }
		}
        currentResolutionIndex = index;

        if (Screen.fullScreen) fullscreen.isOn = true;
        else window.isOn = true;

        SetResolution(Screen.currentResolution);*/

    }

    public void ComfirmResolution() {
        Resolution resolution = resolutions[currentResolutionIndex];

        //SetResolution(resolution);

        Screen.SetResolution(resolution.width, resolution.height, window.isOn ? false:true);
        //FileManager.SaveFileSettings();
    }
    public void NextResolution() {
        if (currentResolutionIndex < resolutions.Length-1) {
            currentResolutionIndex++;

            SetResolution(resolutions[currentResolutionIndex]);
        }
    }
    public void PreviosResolution() {
        if (currentResolutionIndex > 0) {
            currentResolutionIndex--;

            SetResolution(resolutions[currentResolutionIndex]);
        }
    }


    public Resolution DisplayResolution;
    private void SetResolution( Resolution resolution ) {
        textResolution.text = resolution.ToString();
    }


}
