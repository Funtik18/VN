using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Settings : MonoBehaviour {
    private static Settings _instance;

    public bool IsReady { get; private set; }

    public List<string> QualityNames {get; private set; }
    public List<Resolution> ResolutionSettings { get; private set; }

    public GameOptions gameOptions { get; private set; }

    public static Settings getInstance() {
        if (_instance == null) {
            _instance = GameObject.FindObjectOfType<Settings>();
            if(_instance == null) {
                GameObject go = new GameObject("GameSETTINGS");
                _instance = go.AddComponent<Settings>();

                DontDestroyOnLoad(go);
			}
		}
        return _instance;
    }

	private void Awake() {
        //Load strings
        QualityNames = new List<string>(QualitySettings.names);
        ResolutionSettings = new List<Resolution>(Screen.resolutions);
        //Load options
        gameOptions = LoadOptions();
        ApplySettings(gameOptions);

        IsReady = true;
    }

    public void ApplySettings() {
        ApplySettings(gameOptions);
    }
    private void ApplySettings( GameOptions options ) {
        QualitySettings.SetQualityLevel(options.quality);
        Screen.SetResolution(options.width, options.height, options.fullscreen);
    }

    public void SaveOptions( int _quality, int _width, int _height, bool _fullscreen ) {
        GameOptions options = new GameOptions() {
            quality = _quality,
            width = _width,
            height = _height,
            fullscreen = _fullscreen
        };

        string fullPath = FileManager.settingsPath;

        if (FileManager.IsFileExist(fullPath)) {
            FileManager.DeleteFile(fullPath);
		}

        string data = JsonUtility.ToJson(options, true);
        FileManager.SaveFile(fullPath, data);

        gameOptions = options;//remeber options
    }
    public GameOptions LoadOptions() {
        string fullPath = FileManager.settingsPath;

		if (FileManager.IsFileExist(fullPath)) {//если нет предустановленых настроек
            string json = FileManager.LoadFile(fullPath);
            return JsonUtility.FromJson<GameOptions>(json);
        }
        return new GameOptions() { // то дефолтные
            quality = QualitySettings.GetQualityLevel(), 
            width = Screen.width,
            height = Screen.height,
            fullscreen = Screen.fullScreen
        };
    }
}
