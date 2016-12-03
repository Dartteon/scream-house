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
	}

	public static void Load() {
		if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			SaveManager.savedGame = (GameSave)bf.Deserialize(file);
			file.Close();
		}
	}

	public static void AttemptToSavePlayer () {
		if (savedGame != null) {
			savedGame = new GameSave ();
		} else {

		}
	}
}
