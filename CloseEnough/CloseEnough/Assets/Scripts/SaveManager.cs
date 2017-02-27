using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;



public class SaveManager : MonoBehaviour {
	public static GameSave savedGame { get; private set; }

	public static void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd");
		bf.Serialize(file, savedGame);
		file.Close();
//		Debug.Log ("File saved!");
	}

	public static void Load() {
		if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			SaveManager.savedGame = (GameSave)bf.Deserialize(file);
			SaveManager.savedGame.Sort ();
			file.Close();
		}
	}

	public static bool IsNewHighScore (int score) {
		if (savedGame == null) return true;
		else return savedGame.IsPlayerNewHighscore (score);
	}

	public static void AttemptToSavePlayer (int score, string name) {
		if (savedGame != null) {
			if (savedGame.IsPlayerNewHighscore (score)) {
				savedGame.AttemptAddPlayer (name, score);
				Save ();
			}
		} else {
			savedGame = new GameSave ();
			savedGame.AttemptAddPlayer (name, score);
			Save ();
		}
	}
}
