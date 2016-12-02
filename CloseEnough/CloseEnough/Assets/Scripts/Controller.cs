using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	private Player player;

	public void Initialize(Player player) {
		this.player = player;
	}

	void Update() {
		if (player == null) return;
		float yDir = 0f;
		float xDir = 0f;
		if (Input.GetKey (KeyCode.W)) yDir += 1;
		if (Input.GetKey (KeyCode.S)) yDir -= 1;
		if (Input.GetKey (KeyCode.A)) xDir -= 1;
		if (Input.GetKey (KeyCode.D)) xDir += 1;
		Vector2 dir = new Vector2 (xDir, yDir);
		player.SetMoveDirection (dir);

		if (Input.GetKeyDown (KeyCode.Space)) {
			player.ScareAllScareTargetsInRange ();
		}
	}
}
