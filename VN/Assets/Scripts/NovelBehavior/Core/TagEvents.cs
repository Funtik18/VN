using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagEvents : MonoBehaviour {
    public static void Inject( ref string s ) {
        print(s);
        /*if (!s.Contains("["))
           return;

       //replace the mainCharName tag with the actual name of the main character.
       s = s.Replace("[playername]", GAMEFILE.activeFile != null ? GAMEFILE.activeFile.playerName : "No Game File");

       //another random tag just for example.
       s = s.Replace("[curHolyRelic]", "Divine Arc");

       //inject temporary values that are saved in cache, but not saved to any particular game file.
      if (s.Contains("[tempVal")) {
           s = s.Replace("[tempVal 1]", CACHE.tempVals[0]);
           s = s.Replace("[tempVal 2]", CACHE.tempVals[1]);
           s = s.Replace("[tempVal 3]", CACHE.tempVals[2]);
           s = s.Replace("[tempVal 4]", CACHE.tempVals[3]);
           s = s.Replace("[tempVal 5]", CACHE.tempVals[4]);
           s = s.Replace("[tempVal 6]", CACHE.tempVals[5]);
           s = s.Replace("[tempVal 7]", CACHE.tempVals[6]);
           s = s.Replace("[tempVal 8]", CACHE.tempVals[7]);
           s = s.Replace("[tempVal 9]", CACHE.tempVals[8]);
       }*/
    }

    public static string[] SplitByTags( string targetText ) {
        return targetText.Split(new char[2] { '<', '>' });
    }
}
