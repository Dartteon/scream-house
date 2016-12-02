using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareTargetInRangeDetector : MonoBehaviour {
	public List<ScareTarget> targetsInRange { get; private set; }
	private Transform fearRadius;
	private Player player;

	public void Initialize(Player player) {
		this.player = player;
		targetsInRange = new List<ScareTarget> ();
		fearRadius = transform.Find ("FearRadius");
		transform.GetComponent<CircleCollider2D> ().radius = player.scareAttackRange;
		fearRadius.transform.localScale = new Vector3 (player.scareAttackRange / 2.55f, player.scareAttackRange / 2.55f, 1f);
	}

	void OnTriggerEnter2D(Collider2D col) {
		ScareTarget p = col.GetComponent<ScareTarget> ();
		if (p != null) {
			targetsInRange.Add (p);
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		ScareTarget p = col.GetComponent<ScareTarget> ();
		if (p != null && targetsInRange.Contains(p)) {
			targetsInRange.Remove (p);
		}
	}
}
