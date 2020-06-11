using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="keybindings", menuName ="keys")]
public class KeyBindings : ScriptableObject{
	public KeyCode Next = KeyCode.Return;
	public KeyCode Back = KeyCode.Backspace;
	public KeyCode ESC = KeyCode.Escape;
	public KeyCode SkipLineSegment = KeyCode.Space;
}
