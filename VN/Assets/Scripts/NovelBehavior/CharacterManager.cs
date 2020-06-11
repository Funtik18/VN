using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {
	public static CharacterManager _instance;

	/// <summary>
	/// All characters must be attached to the character panel.
	/// </summary>
	public RectTransform characterPanel;

	/// <summary>
	/// A list of all characters currently in our scene.
	/// </summary>
	public List<Character> characters = new List<Character>();
	/// <summary>
	/// Easy lookup for our characters.
	/// </summary>
	public Dictionary<string, int> characterDictionary = new Dictionary<string, int>();

	void Awake() {
		_instance = this;

		characterPanel.gameObject.SetActive(true);
	}

	/// <summary>
	/// Try to get a character by the name provided from the character list.
	/// </summary>
	/// <returns>The character.</returns>
	/// <param name="characterName">Character name.</param>
	/// <param name="createCharacterIfDoesNotExist">If set to <c>true</c> create character if does not exist.</param>
	/// <param name="enableCreatedCharacterOnStart">If set to <c>true</c> enable created character on start.</param>
	public Character GetCharacter( string characterName, bool createCharacterIfDoesNotExist = true, bool enableCreatedCharacterOnStart = true ) {
		int index = -1;
		if (characterDictionary.TryGetValue(characterName, out index)) {
			return characters[index];
		} else if (createCharacterIfDoesNotExist){
			if (FileManager.GetCharacter(characterName) != null)
				return CreateCharacter(characterName, enableCreatedCharacterOnStart);
			return null;
		}
		return null;
	}
	

	/// <summary>
	/// Creates the character.
	/// </summary>
	/// <returns>The character.</returns>
	/// <param name="characterName">Character name.</param>
	public Character CreateCharacter( string characterName, bool enableOnStart = true ) {
		Character newCharacter = new Character(characterName, enableOnStart);

		characterDictionary.Add(characterName, characters.Count);
		characters.Add(newCharacter);

		return newCharacter;
	}

	/// <summary>
	/// Destroys a character in the scene.
	/// </summary>
	/// <param name="character"></param>
	public void DestroyCharacter( Character character ) {
		if (characters.Contains(character))
			characters.Remove(character);

		characterDictionary.Remove(character.characterName);

		Destroy(character.root.gameObject);
	}
	/// <summary>
	/// Destroys a character in the scene by this name.
	/// </summary>
	/// <param name="characterName"></param>
	public void DestroyCharacter( string characterName ) {
		Character character = GetCharacter(characterName, false, false);
		if (character != null) {
			DestroyCharacter(character);
		}
	}
	public void DestroyCharacters() {
		while (characters.Count > 0) {
			DestroyCharacter(characters[0]);
		}
	}


	public class CHARACTERPOSITIONS {

		public Vector2 left = new Vector2(0f, 0.5f);
		public Vector2 topLeft = new Vector2(0, 1f);
		public Vector2 bottomLeft = new Vector2(0, 0);

		public Vector2 top = new Vector2(0.5f, 1f);
		public Vector2 center = new Vector2(0.5f, 0.5f);
		public Vector2 bottom = new Vector2(0.5f, 0);

		public Vector2 topRight = new Vector2(1f, 1f);
		public Vector2 bottomRight = new Vector2(1f, 0);
		public Vector2 right = new Vector2(1, 0.5f);
	}
	public static CHARACTERPOSITIONS characterPositions = new CHARACTERPOSITIONS();

	public static Vector2 GetPositionByString(string name) {
		Vector2 pos = Vector2.zero;
		switch (name) {
			case "l": {
				pos = characterPositions.left;
			}
			break;
			case "lt": {
				pos = characterPositions.topLeft;
			}
			break;
			case "lb": {
				pos = characterPositions.bottomLeft;
			}
			break;
			case "c": {
				pos = characterPositions.center;
			}
			break;
			case "ct": {
				pos = characterPositions.top;
			}
			break;
			case "cb": {
				pos = characterPositions.bottom;
			}
			break;

			case "r": {
				pos = characterPositions.right;
			}
			break;
			case "rt": {
				pos = characterPositions.topRight;
			}
			break;
			case "rb": {
				pos = characterPositions.bottomRight;
			}
			break;
			default: {
				pos = new Vector2(-1,-1);
			}
			break;
		}
		return pos;
	}
}
