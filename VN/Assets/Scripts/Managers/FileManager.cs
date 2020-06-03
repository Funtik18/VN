using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour{
    public static string fileExtension = ".txt";


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
