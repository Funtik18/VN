using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour {//print(Application.persistentDataPath);

    private readonly static string SETTINGS_FILE = "/settings.json";
    
    public static string fileExtension = ".txt";

    public static string settingsPath { get { return Path.Combine(Application.persistentDataPath + SETTINGS_FILE); }  private set { } }

    public static string pathStory = "Story";
    public static string pathCharacters = "Prefabs/Characters";
    public static string pathSpritesCharacters = "Images/Characters";


    public static void SaveFile( string path, string data, bool append = false ) {
        StreamWriter sw = new StreamWriter(path, append);
        sw.WriteLine(data);
        sw.Close();
    }
    public static string LoadFile(string path) {
        string data = "";
        StreamReader sr = new StreamReader(path);
        data = sr.ReadToEnd();
        sr.Close();

        return data;
    }
    public static bool IsFileExist( string path ) {
        if (File.Exists(path))
            return true;
        return false;
    }
    public static void DeleteFile( string path ) {
        File.Delete(path);
	}
    
        

    public static TextAsset GetFileTXT(string _path) {////////////
        TextAsset txt = Resources.Load<TextAsset>($"{pathStory}/{_path}");
        return txt;
    }
    public static GameObject GetCharacter( string _name ) {
        GameObject go = Resources.Load($"{pathCharacters}/Character[{_name}]") as GameObject;
        return go;
    }

    public static Sprite[] GetSprites( string _path ) {
        Sprite[] sprites = Resources.LoadAll<Sprite>($"{pathSpritesCharacters}/{_path}");
        return sprites;
    }


    /// <summary>
    /// Read a text asset and return a list of lines
    /// </summary>
    /// <returns>The text asset.</returns>
    /// <param name="txt">Text.</param>
    public static List<string> ReadTextAsset( TextAsset txt ) {
        string[] lines = txt.text.Split('\n', '\r');

        return HelpFunctions.ArrayToList(lines);
    }

	
}
