using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour {
	public void SetHighscoreBoard() {
		if (SaveManager.savedGame != null) {
			GameSave savedGame = SaveManager.savedGame;
			savedGame.Sort ();
			List<SavedPlayer> savedPlayers = savedGame.highscorePlayers;
//			Debug.Log ("Saved players count = " + savedPlayers.Count);
			Transform leaderboardTransform = transform.Find ("Leaderboard");
			for (int i = 0; i < 5; i++) {
				Transform t = leaderboardTransform.Find ("" + i + "Canvas");
				if (i > savedPlayers.Count - 1) {
					t.gameObject.SetActive (false);
					continue;
				}
				if (t != null) {
//					Debug.Log ("Setting text");
					t.Find ("Text").GetComponent<Text> ().text = "" + (i + 1) + ". " + " " + savedPlayers [i].score + " POINTS";

					t.gameObject.SetActive (true);
				} else {
					Debug.Log ("NULL");
				}
			}
			leaderboardTransform.gameObject.SetActive (true);
		}
	}
}
