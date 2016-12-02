using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FearBar : MonoBehaviour {
	private ScareTarget scareTarget;
	private Image barImage;
	private Color startColor = new Color (0f, 1f, 0f);
	private Color endColor = new Color (1f, 0f, 0f);
	private Text text;
	private GameObject playerInRangeIndicator;

	public void Initialize(ScareTarget scareTarget) {
		this.scareTarget = scareTarget;
		playerInRangeIndicator = transform.Find ("PlayerInRangeIndicator").gameObject;
		this.text = transform.Find ("Canvas").Find ("Text").GetComponent<Text> ();
		barImage = transform.Find ("Canvas").Find ("Image").GetComponent<Image> ();
	}

	void Update() {
		float currFear = scareTarget.fearCount;
		float maxFear = scareTarget.maximumFearThreshold;
		float ratio = currFear / maxFear;
		barImage.fillAmount = ratio;
		barImage.color = Color.Lerp (startColor, endColor, ratio);
		text.text = scareTarget.currDistFromPlayer.ToString ();
		if (ratio <= 0) {
			gameObject.SetActive (false);
		}
		if (scareTarget.IsPlayerInFearRange ()) {
			playerInRangeIndicator.gameObject.SetActive (true);
		} else {
			playerInRangeIndicator.gameObject.SetActive (false);
		}
	}

}
