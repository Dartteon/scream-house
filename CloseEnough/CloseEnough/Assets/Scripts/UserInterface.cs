using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
	private static UserInterface instance;
	private Text scoreText;

	public void Initialize() {
		instance = this;
		scoreText = transform.Find ("ScoreCanvas").Find ("Text").GetComponent<Text> ();
		scoreText.text = "0";
	}


	public static void SetScore (int score) {
		if (instance.scoreText == null) return;
		instance.scoreText.text = score.ToString();
	}
}
