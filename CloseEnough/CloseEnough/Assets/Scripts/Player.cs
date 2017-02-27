using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Human {
	[SerializeField]
	private AudioClip screamSound;

	private AudioSource audio;

	public float scareAttackRange { get; private set; }
	private ScareTargetInRangeDetector scareTargetInRangeDetector;
	private string ANIM_SCARING = "PlayerScaring";

	public override void Initialize () {
		audio = transform.GetComponent<AudioSource> ();
		this.scareAttackRange = GameSettings.scareAttackRange;
		scareTargetInRangeDetector = transform.Find ("ScareTargetInRangeDetector").GetComponent<ScareTargetInRangeDetector> ();
		scareTargetInRangeDetector.Initialize (this);
		currDirection = Direction.DOWN;

		ANIM_DOWNWARD_MOVING = "PlayerDownwardMoving";
		ANIM_DOWNWARD_IDLE = "PlayerDownwardIdle";
		ANIM_UPWARD_MOVING = "PlayerUpwardMoving";
		ANIM_UPWARD_IDLE = "PlayerUpwardIdle";
		ANIM_RIGHTWARD_MOVING = "PlayerRightwardMoving";
		ANIM_RIGHTWARD_IDLE = "PlayerRightwardIdle";
		ANIM_LEFTWARD_MOVING = "PlayerLeftwardMoving";
		ANIM_LEFTWARD_IDLE = "PlayerLeftwardIdle";

//		Debug.Log ("Player Init!");
	}

	public void ScareAllScareTargetsInRange() {
		if (currState == HumanState.SCARING) return;
		audio.PlayOneShot (screamSound);
		CameraManager.CameraShake (.2f, .5f);
		List<ScareTarget> scareTargetsInRange = scareTargetInRangeDetector.targetsInRange;
		GameManager.instance.TriggerScare (scareTargetsInRange);
		scareTargetsInRange.Clear ();
		SetNextState(HumanState.SCARING);
		SetMoveDirection (new Vector2(0f, 0f));
		anim.Play (ANIM_SCARING);
		Invoke ("UnstunPlayer", 1f);
	}

	public void StunPlayer(float duration) {
		SetMoveDirection (new Vector2(0f, 0f));
		SetNextState(HumanState.SCARING);
		Invoke ("UnstunPlayer", duration);
	}

	public void UnstunPlayer() {
		SetNextState(HumanState.IDLE);
		anim.Play (ANIM_DOWNWARD_IDLE);
	}
}