using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Human {
	public float scareAttackRange { get; private set; }
	private ScareTargetInRangeDetector scareTargetInRangeDetector;

	public override void Initialize () {
		this.scareAttackRange = GameSettings.scareAttackRange;
		scareTargetInRangeDetector = transform.Find ("ScareTargetInRangeDetector").GetComponent<ScareTargetInRangeDetector> ();
		scareTargetInRangeDetector.Initialize (this);

		ANIM_DOWNWARD_MOVING = "PlayerDownwardMoving";
		ANIM_DOWNWARD_IDLE = "PlayerDownwardIdle";
		ANIM_UPWARD_MOVING = "PlayerUpwardMoving";
		ANIM_UPWARD_IDLE = "PlayerUpwardIdle";
		ANIM_RIGHTWARD_MOVING = "PlayerRightwardMoving";
		ANIM_RIGHTWARD_IDLE = "PlayerRightwardIdle";
		ANIM_LEFTWARD_MOVING = "PlayerLeftwardMoving";
		ANIM_LEFTWARD_IDLE = "PlayerLeftwardIdle";

		Debug.Log ("Player Init!");
	}

	public void ScareAllScareTargetsInRange() {
		CameraManager.CameraShake (.2f, .5f);
		List<ScareTarget> scareTargetsInRange = scareTargetInRangeDetector.targetsInRange;
		foreach (ScareTarget scareTarget in scareTargetsInRange) {
			scareTarget.TriggerScare ();
		}
	}
}
