using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareTargetSight : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {
		Player player = col.GetComponent<Player> ();
		if (player != null) {
			transform.parent.GetComponent<ScareTarget> ().TriggerPlayerSighted ();
		}
	}
}
