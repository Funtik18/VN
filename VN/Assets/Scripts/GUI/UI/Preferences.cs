using System.Collections;
using System.Collections.Generic;
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

        FileManager.CreateFileSettings();

        btnComfirm.onClick.AddListener(() => { ComfirmResolution(); });
        btnReject.onClick.AddListener(() => { });
        btnNext.onClick.AddListener(() => { NextResolution(); });
        btnPrev.onClick.AddListener(() => { PreviosResolution(); });

        resolutions = Screen.resolutions;

        SetResolution(resolutions[currentResolutionIndex]);

    }

    public void ComfirmResolution() {
        Resolution resolution = resolutions[currentResolutionIndex];

        SetResolution(resolution);

        Screen.SetResolution(resolution.width, resolution.height, window.isOn?FullScreenMode.Windowed:FullScreenMode.FullScreenWindow);
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


    private void SetResolution( Resolution resolution ) {
        textResolution.text = resolution.width + "X" + resolution.height;
    }
}
