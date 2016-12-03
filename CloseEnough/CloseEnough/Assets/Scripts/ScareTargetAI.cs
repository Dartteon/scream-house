using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction = Human.Direction;

public class ScareTargetAI : MonoBehaviour {
	private ScareTarget scareTarget;


	private float baseDecisionTimeRandomizer = 1f;
	private float baseDecisionTimeSpacing = 1f;
	private float nextDecisionTiming;


	public void Initialize(ScareTarget scareTarget) {
		this.scareTarget = scareTarget;

	}

	void Update() {
		if (scareTarget.currState == Human.HumanState.SCARED) {
			return;
		}
		if (Time.time >= nextDecisionTiming) {
			float fearCount = scareTarget.fearCount;
			float maxFear = scareTarget.maximumFearThreshold;
			float ratio = 1f - (fearCount / maxFear);
			float difference = (ratio * baseDecisionTimeRandomizer) + baseDecisionTimeSpacing;
			float newNextDecisionTiming = Time.time + difference;
			nextDecisionTiming = newNextDecisionTiming;
			MakeDecision ();
		}
	}

	private void MakeDecision() {
		bool isStopped = (Random.Range (0, 2) == 1);
		if (isStopped) {
			scareTarget.SetMoveDirection(new Vector2(0, 0));
			return;
		}

		float xDir = 0, yDir = 0;
		bool isGoingSameDir = (Random.Range (0, 2) == 1);
		bool isGoingCWDir = (Random.Range (0, 2) == 1);
		switch (scareTarget.currDirection) {
		case Direction.UP:
			yDir = (isGoingSameDir ? 1 : 0);
			xDir = (isGoingCWDir ? 1 : -1);
			break;
		case Direction.DOWN:
			yDir = (isGoingSameDir ? -1 : 0);
			xDir = (isGoingCWDir ? -1 : 1);
			break;
		case Direction.LEFT:
			xDir = (isGoingSameDir ? -1 : 0);
			yDir = (isGoingCWDir ? 1 : -1);
			break;
		case Direction.RIGHT:
			xDir = (isGoingSameDir ? 1 : 0);
			yDir = (isGoingCWDir ? -1 : 1);
			break;
		}
		scareTarget.SetMoveDirection(new Vector2(xDir, yDir));
	}

}
