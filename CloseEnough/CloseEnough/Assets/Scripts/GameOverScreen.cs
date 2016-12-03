using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {
	public void SetScore(int score) {
		transform.Find ("ScoreCanvas").Find ("Text").GetComponent<Text> ().text = score.ToString ();
	}
}
