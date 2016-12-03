using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FearBar : MonoBehaviour {
	private ScareTarget scareTarget;
	private Image barImage;
	private Color unripeColor = new Color (0f, 0f, 1f);
	private Color ripeColor = new Color (0f, 1f, 0f);
	private Color overripeColor = new Color (1f, 0f, 0f);
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
		float minFear = scareTarget.minimumFearThreshold;
		float ratio = currFear / maxFear;
		barImage.fillAmount = ratio;
		if (currFear >= maxFear) {
			barImage.color = overripeColor;
		} else {
			barImage.color = (currFear > minFear) ? ripeColor : unripeColor;
		}
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
