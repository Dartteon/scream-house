using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
	private static UserInterface instance;
	private Text scoreText;
	private Text timeLeftText;
	private Text comboText;
	private Animator comboInterfaceAnim;

	private Transform scorePopup;
	private Text scorePopupScoreText;
	private Text scorePopupMultiplierText;
	private Text scorePopupComboText;

	private Text tipsText;

	private int lastScore = 0;

	private static float showTipDuration = 3f;

	private Animator timePopupAnimator;
	private Text extraTimeText;

	private static readonly string pressSpaceTip = "Press SPACE to scare when the time is ripe!";
	private static readonly string dontTouchTip = "Not THAT close!\nTouching them causes a heart attack! RIP combo.";
	private static readonly string notImpressedTip = "Let their fear bar build before scaring them...";
	private static readonly string lineOfSightTip = "Don't let them see you, or you lose your combo!";
	private static readonly string fearBuildupTip = "Letting them build too much fear kills them!";


	private bool isShowingTip;
	private float showTipStartTime;

	public void Initialize() {
		instance = this;
		scoreText = transform.Find ("ScoreCanvas").Find ("Text").GetComponent<Text> ();
		comboText = transform.Find ("ComboInterface").Find ("ComboCanvas").Find ("Text").GetComponent<Text> ();
		timeLeftText = transform.Find ("TimeCanvas").Find ("Text").GetComponent<Text> ();
		tipsText = transform.Find ("TipsCanvas").Find ("Text").GetComponent<Text> ();
		extraTimeText = transform.Find ("TimeCanvas").Find ("TimeExtentionText").GetComponent<Text> ();
		scoreText.text = "0";
		comboText.text = "x1";
		timeLeftText.text = GameManager.GetTimeLeft ().ToString();
		comboInterfaceAnim = transform.Find ("ComboInterface").GetComponent<Animator> ();
		ShowTip (0);

		scorePopup = transform.Find ("PopupScore");
		scorePopupMultiplierText = scorePopup.Find ("MultiscoreCanvas").Find ("Text").GetComponent<Text> ();
		scorePopupComboText = scorePopup.Find ("ComboCanvas").Find ("Text").GetComponent<Text> ();
		scorePopupScoreText = scorePopup.Find ("ScoreCanvas").Find ("Text").GetComponent<Text> ();

		timePopupAnimator = transform.Find ("TimeCanvas").GetComponent<Animator> ();
	}
	public static void ShowTip (int tipType) {
//		Debug.Log ("Called to show tip");
		if (instance.isShowingTip) return;
		instance.isShowingTip = true;
		instance.showTipStartTime = Time.time;
		switch (tipType) {
		case 0:
			instance.tipsText.text = (pressSpaceTip);
			break;
		case 1:
			instance.tipsText.text = (dontTouchTip);
			break;
		case 2:
			instance.tipsText.text = (notImpressedTip);
			break;
		case 3:
			instance.tipsText.text = (lineOfSightTip);
			break;
		case 4:
			instance.tipsText.text = (fearBuildupTip);
			break;
		}
		instance.tipsText.transform.parent.gameObject.SetActive (true);

		instance.Invoke ("HideTipText", showTipDuration);
	}

	private void HideTipText() {
		isShowingTip = false;
		tipsText.transform.parent.gameObject.SetActive (false);
	}


	public static void SetScore (int score) {
		if (instance.scoreText == null) return;
		instance.scoreText.text = score.ToString();
	}

	public static void ShowScoreGain (int scoreGain, int multiScareMultiplier, float comboMultiplier) {
		if (comboMultiplier > 1) {
			instance.scorePopupComboText.text = "x" + comboMultiplier.ToString () + " COMBO";
			instance.scorePopupComboText.gameObject.SetActive (true);
		} else {
			instance.scorePopupComboText.gameObject.SetActive (false);
		}
		if (multiScareMultiplier > 1) {
			instance.scorePopupMultiplierText.text = "x" + multiScareMultiplier + " MULTISCARE";
			instance.scorePopupMultiplierText.gameObject.SetActive (true);
		} else {
			instance.scorePopupMultiplierText.gameObject.SetActive (false);
		}

		instance.scorePopupScoreText.text = scoreGain.ToString ();
		instance.scorePopup.gameObject.SetActive (true);
		instance.Invoke ("DisableScoreGainPopup", 1f);
	}

	private void DisableScoreGainPopup() {
		scorePopup.gameObject.SetActive (false);
	}

	public static void TriggerTimeExtension(float timeAmount, string description) {
		instance.extraTimeText.text = "+ " + timeAmount + "\n" + description;
		instance.timePopupAnimator.Play ("TimeCanvasExtendTime");
	}

	public static void SetTimeLeft(float t) {
		t = Mathf.Round(t);
		instance.timeLeftText.text = t.ToString ();
	}

	public static void SetCombo (float combo) {
		instance.comboText.text = "x" + combo.ToString();
		if (instance.comboInterfaceAnim.gameObject.activeInHierarchy) {
			instance.comboInterfaceAnim.Play ("ComboInterfaceChange");
		}
	}
}
