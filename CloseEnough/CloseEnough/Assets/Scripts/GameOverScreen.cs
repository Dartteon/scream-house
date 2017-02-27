using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {
	public void SetScore(int score, bool isNewHighscore) {
		transform.Find ("ScoreCanvas").Find ("Text").GetComponent<Text> ().text = score.ToString ();
//		Debug.Log ("IS NEW HIGHSCORE? " + isNewHighscore);

		if (isNewHighscore) SaveManager.AttemptToSavePlayer (score, "ABC");
	}
}
