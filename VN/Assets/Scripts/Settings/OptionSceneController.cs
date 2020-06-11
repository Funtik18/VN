using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionSceneController : MonoBehaviour {

	public static OptionSceneController _instace;

	//public TMP_Dropdown quality;//delete
	public Carousel carousel;

	public Toggle fullscreen;

	public Button btnComfirm;
	public Button btnReject;

	public Button btnClose;

	List<Resolution> filteredResolutions = new List<Resolution>();
	int currentIndex = -1;

	private void Awake() {
		_instace = this;

		fullscreen.onValueChanged.AddListener(delegate { FullScreen(); });

		btnComfirm.onClick.AddListener(() => { ApplySettings(); });
		btnReject.onClick.AddListener(() => { RejectSettings(); });
	}

	IEnumerator Start() {
		//Wait settings
		Settings setting = Settings.getInstance();
		while (!setting.IsReady) {
			yield return null;//return one frame
		}

		GameOptions options = setting.gameOptions;

		//ui
		//quality
		//quality.ClearOptions();
		//quality.AddOptions(setting.QualityNames);
		//quality.value = options.quality;

		//resolution
		int index = 0;
		int lw = -1, lh = -1;
		List<string> resolutions = new List<string>();
		for(int i = 0; i < setting.ResolutionSettings.Count; i++) {
			Resolution temp = setting.ResolutionSettings[i];
			if (lw != temp.width && lh != temp.height) {
				lw = temp.width;
				lh = temp.height;

				if(lw == options.width && lh == options.height) {
					currentIndex = index;
				}
				resolutions.Add($"{temp.width}X{temp.height}");//add string 
				filteredResolutions.Add(temp);
				index++;
			}
		}
		carousel.AddOptions(resolutions);
		carousel.value = currentIndex;

		//fullscreen
		fullscreen.isOn = options.fullscreen;
	}

	private void FullScreen() {
		
	}

	/// <summary>
	/// Отмена действий
	/// </summary>
	public void RejectSettings() {
		Settings setting = Settings.getInstance();
		GameOptions options = setting.gameOptions;

		//quality.value = options.quality;
		carousel.value = currentIndex;

		fullscreen.isOn = options.fullscreen;
	}

	/// <summary>
	/// Принятие действий
	/// </summary>
	private void ApplySettings() {
		Settings settings = Settings.getInstance();

		currentIndex = carousel.value;

		//int currentQuality = quality.value;
		Resolution currentResolution = filteredResolutions[currentIndex];

		settings.SaveOptions(QualitySettings.GetQualityLevel() /*currentQuality*/, currentResolution.width, currentResolution.height, fullscreen.isOn);
		settings.ApplySettings();
	}
}
