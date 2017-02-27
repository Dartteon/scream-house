using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameSave {
	public List<SavedPlayer> highscorePlayers;
	private int maxNumberPlayers = 5;

	public GameSave () {
		highscorePlayers = new List<SavedPlayer> ();
	}

	public bool IsPlayerNewHighscore(int score) {
		if (highscorePlayers.Count <= maxNumberPlayers || highscorePlayers == null) return true;
		Sort ();
		SavedPlayer lowestPlayer = highscorePlayers [highscorePlayers.Count - 1];	//last
		if (lowestPlayer.score < score) {
			return true;
		}
		return false;
	}
	public bool AttemptAddPlayer (string playerName, int score) {
		if (highscorePlayers.Count < maxNumberPlayers) {
			highscorePlayers.Add (new SavedPlayer (score, playerName));
			return true;
		}
		Sort ();
		SavedPlayer lowestPlayer = highscorePlayers [highscorePlayers.Count - 1];	//last
		if (lowestPlayer.score < score) {
			highscorePlayers.Add (new SavedPlayer (score, playerName));
			highscorePlayers.Sort ((x, y) => y.score.CompareTo (x.score));
			highscorePlayers.RemoveAt (highscorePlayers.Count - 1);
			return true;
		}
		return false;
	}

	public void Sort() {
//		Debug.Log ("Sorting");
		if (highscorePlayers != null)
			highscorePlayers.Sort ((x, y) => y.score.CompareTo (x.score));
	}


	public void Test() {
		SavedPlayer p1 = new SavedPlayer (9, "Hi");
		SavedPlayer p2 = new SavedPlayer (3, "AAA");
		SavedPlayer p3 = new SavedPlayer (2, "BB");
		highscorePlayers = new List<SavedPlayer> ();
		highscorePlayers.Add (p1);
		highscorePlayers.Add (p2);
		highscorePlayers.Add (p3);
		highscorePlayers.Sort ((x, y) => y.score.CompareTo (x.score));
		for (int i = 0; i < highscorePlayers.Count; i++) {
			Debug.Log (highscorePlayers [i].name + " " + highscorePlayers [i].score);
		}
	}

}


[System.Serializable]
public class SavedPlayer {
	public int score;
	public string name;
	public SavedPlayer (int score, string name) {
		this.score = score;
		this.name = name;
	}
}