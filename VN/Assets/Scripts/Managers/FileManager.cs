using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour{
    public static string fileExtension = ".txt";

    public static string pathStory = "Story";
    public static string pathCharacters = "Prefabs/Characters";
    public static string pathSpritesCharacters = "Images/Characters";

    public static TextAsset GetFileTXT(string _path) {
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
