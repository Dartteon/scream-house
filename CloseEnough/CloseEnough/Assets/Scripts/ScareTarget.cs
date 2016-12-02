using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareTarget : Human {
	public float fearCount { get; private set; }
	public float fearGainRate { get; private set; }
	public float minimumFearThreshold { get; private set; }
	public float maximumFearThreshold { get; private set; }
	public Transform sightObject { get; private set; }
	private float fearLossRate = 20f;
	public float currDistFromPlayer { get; private set; }
	private PlayerInRangeDetector playerInRangeDetector;
	private FearBar fearBar;

	private bool hasSightedPlayer;
	private Player player;
	//Maximum distance this needs to be from the player, to start accumulating fear
	public float maximumPlayerDistance { get; private set; }

	public void Initialize(Player player) {
		fearGainRate = 80f;
		maximumFearThreshold = 100f;
		maximumPlayerDistance = GameSettings.scareAttackRange;

		this.player = player;
		sightObject = transform.Find ("Sight");
		fearBar = transform.Find ("FearBar").GetComponent<FearBar> ();
		fearBar.Initialize (this);
		playerInRangeDetector = transform.Find ("PlayerInRangeDetector").GetComponent<PlayerInRangeDetector> ();
		playerInRangeDetector.Initialize (this);


		//TODO: Change to correct anim names and sprite
		ANIM_DOWNWARD_MOVING = "PlayerDownwardMoving";
		ANIM_DOWNWARD_IDLE = "PlayerDownwardIdle";
		ANIM_UPWARD_MOVING = "PlayerUpwardMoving";
		ANIM_UPWARD_IDLE = "PlayerUpwardIdle";
		ANIM_RIGHTWARD_MOVING = "PlayerRightwardMoving";
		ANIM_RIGHTWARD_IDLE = "PlayerRightwardIdle";
		ANIM_LEFTWARD_MOVING = "PlayerLeftwardMoving";
		ANIM_LEFTWARD_IDLE = "PlayerLeftwardIdle";
	}

	public void TriggerScare() {
		if (currState != HumanState.SCARED && gameObject.activeInHierarchy) {
			currState = HumanState.SCARED;
			Vector2 playerPos = player.transform.position;
			Vector2 currPos = transform.position;
			float dist = (playerPos - currPos).magnitude;
			float ratio = 1f - (dist / maximumPlayerDistance);
			GameManager.TriggerScare (fearCount, ratio);
			gameObject.SetActive (false);
		}
	}

	public void TriggerPlayerSighted() {
		if (!hasSightedPlayer) {
			hasSightedPlayer = true;
			Debug.LogError ("PLAYER SIGHTED!");
		}
	}

	public bool IsPlayerInFearRange() {
		return playerInRangeDetector.isPlayerInFearRange;
	}

	protected override void CallExtraUpdates() {
		if (player == null) {
			Debug.LogError ("Player not set!");
			return;
		}
		SetSightDirection ();
		CheckPlayerAndAddFear ();
	}
	private void CheckPlayerAndAddFear() {
		if (playerInRangeDetector.isPlayerInFearRange) {
			Vector2 playerPos = player.transform.position;
			Vector2 currPos = transform.position;
			float dist = (playerPos - currPos).magnitude;

			//Player close enough.. start gaining fear
			float ratio = 1f - (dist / maximumPlayerDistance);
			float fearToAdd = ratio * fearGainRate * Time.deltaTime;
			if (fearCount + fearToAdd >= maximumFearThreshold) {
				fearCount = maximumFearThreshold;
			} else {
				fearCount += fearToAdd;
			}
			fearBar.gameObject.SetActive (true);
		} else {
			//Slowly lose fear?
			float fearToMinus = fearLossRate * Time.deltaTime;
			if (fearCount - fearToMinus <= 0) {
				fearCount = 0f;
			} else {
				fearCount -= fearToMinus;
			}
		}
	}

	private void SetSightDirection() {
		switch (currDirection) {
		case Direction.UP:
			sightObject.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, 180f));
			break;
		case Direction.DOWN:
			sightObject.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, 0));
			break;
		case Direction.LEFT:
			sightObject.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, 270f));
			break;
		case Direction.RIGHT:
			sightObject.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, 90f));
			break;
		}
	}
}
