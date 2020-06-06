using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionSceneController : MonoBehaviour {

	public TMP_Dropdown quality;
	public TMP_Dropdown resolution;

	public Toggle fullscreen;

	public Button btnComfirm;
	public Button btnReject;


	List<Resolution> filteredResolutions = new List<Resolution>();

	private void Awake() {
		fullscreen.onValueChanged.AddListener(delegate { });

		btnComfirm.onClick.AddListener(() => { ApplySettings(); });
		btnReject.onClick.AddListener(() => { });
	}

	IEnumerator Start() {
		//Wait settings
		while (!Settings.getInstance().IsReady) {
			yield return null;//return one frame
		}

		Settings setting = Settings.getInstance();
		GameOptions options = setting.gameOptions;

		//ui
		//quality
		quality.ClearOptions();
		quality.AddOptions(setting.QualityNames);
		quality.value = options.quality;

		//resolution
		int index = 0;
		int currentIndex = -1;
		int lw = -1, lh = -1;
		resolution.ClearOptions();
		List<string> resolutions = new List<string>();
		for(int i = 0; i < setting.ResolutionSettings.Count; i++) {
			Resolution temp = setting.ResolutionSettings[i];
			if (lw != temp.width && lh != temp.height) {
				resolutions.Add($"{temp.width}X{temp.height}");//add string 
				lw = temp.width;
				lh = temp.height;

				if(lw == options.width && lh == options.height) {
					currentIndex = index;
				}
				filteredResolutions.Add(temp);
			}
			index++;
		}
		resolution.AddOptions(resolutions);
		resolution.value = currentIndex;

		//fullscreen
		fullscreen.isOn = options.fullscreen;
	}

	private bool FullScreen() {
		return false;
	}

	private void ApplySettings() {
		Settings settings = Settings.getInstance();

		int currentQuality = quality.value;
		Resolution currentResolution = filteredResolutions[resolution.value];

		settings.SaveOptions(currentQuality, currentResolution.width, currentResolution.height, fullscreen.isOn);
		GameOptions options = settings.LoadOptions();
		settings.ApplySettings(options);
	}
}
