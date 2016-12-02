using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRangeDetector : MonoBehaviour {
	public bool isPlayerInFearRange { get; private set; }
	private Transform fearRadius;
	private ScareTarget scareTarget;

	public void Initialize(ScareTarget scareTarget) {
		this.scareTarget = scareTarget;
		fearRadius = transform.Find ("FearRadius");
		transform.GetComponent<CircleCollider2D> ().radius = scareTarget.maximumPlayerDistance;
		fearRadius.transform.localScale = new Vector3 (scareTarget.maximumPlayerDistance / 2.55f, scareTarget.maximumPlayerDistance / 2.55f, 1f);
	}

	void OnTriggerEnter2D(Collider2D col) {
		Player p = col.GetComponent<Player> ();
		if (p != null) {
			isPlayerInFearRange = true;
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		Player p = col.GetComponent<Player> ();
		if (p != null) {
			isPlayerInFearRange = false;
		}
	}
}
