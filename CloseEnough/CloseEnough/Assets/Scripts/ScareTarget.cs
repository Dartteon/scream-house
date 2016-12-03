using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareTarget : Human {
	[SerializeField]
	private Transform emojiPopupPrefab;

	private AudioSource audio;
	[SerializeField]
	private AudioClip scaredSound;

	public float fearCount { get; private set; }
	public float fearGainRate { get; private set; }
	public float minimumFearThreshold { get; private set; }
	public float maximumFearThreshold { get; private set; }
	public Transform sightObject { get; private set; }
	private float fearLossRate = 20f;
	public float currDistFromPlayer { get; private set; }
	private PlayerInRangeDetector playerInRangeDetector;
	private FearBar fearBar;

	public bool hasSightedPlayer { get; private set; }
	public bool hasBeenScared { get; private set; }

	private string ANIM_SCARETARGET_SCARED = "ScareTarget1Scared";
	private string ANIM_SCARETARGET_HEART_ATTACK = "ScareTarget1HeartAttack";

	private Player player;
	//Maximum distance this needs to be from the player, to start accumulating fear
	public float maximumPlayerDistance { get; private set; }

	public void Initialize(Player player) {
		audio = transform.GetComponent<AudioSource> ();
		fearGainRate = GameSettings.fearGainRate;
		maximumFearThreshold = 100f;
		maximumPlayerDistance = GameSettings.scareAttackRange;
		minimumFearThreshold = 75f;

		this.player = player;
		sightObject = transform.Find ("Sight");
		fearBar = transform.Find ("FearBar").GetComponent<FearBar> ();
		fearBar.Initialize (this);
		playerInRangeDetector = transform.Find ("PlayerInRangeDetector").GetComponent<PlayerInRangeDetector> ();
		playerInRangeDetector.Initialize (this);
		transform.GetComponent<ScareTargetAI> ().Initialize (this);

		ANIM_DOWNWARD_MOVING = "ScareTarget1DownwardMoving";
		ANIM_DOWNWARD_IDLE = "ScareTarget1DownwardIdle";
		ANIM_UPWARD_MOVING = "ScareTarget1UpwardMoving";
		ANIM_UPWARD_IDLE = "ScareTarget1UpwardIdle";
		ANIM_RIGHTWARD_MOVING = "ScareTarget1RightwardMoving";
		ANIM_RIGHTWARD_IDLE = "ScareTarget1RightwardIdle";
		ANIM_LEFTWARD_MOVING = "ScareTarget1LeftwardMoving";
		ANIM_LEFTWARD_IDLE = "ScareTarget1LeftwardIdle";
	}

	public void TriggerScare(int scareType) {
		hasBeenScared = true;
		if (currState != HumanState.SCARED && gameObject.activeInHierarchy) {
			SetNextState(HumanState.SCARED);
			Transform emojiPopup = Instantiate (emojiPopupPrefab, transform);
			emojiPopup.transform.localPosition = new Vector3 (0f, 1.71f, -2f);
			switch (scareType) {
			case 0:
				emojiPopup.GetComponent<EmojiPopup> ().ShowRandomScaredEmoji ();
				audio.PlayOneShot (scaredSound);
				anim.Play (ANIM_SCARETARGET_SCARED);
				break;
			case 1:
				emojiPopup.GetComponent<EmojiPopup> ().ShowRandomNotScaredEmoji ();
				anim.Play (ANIM_DOWNWARD_IDLE);
				break;
			case 2:
				emojiPopup.GetComponent<EmojiPopup> ().ShowRandomHeartAttackEmoji ();
				audio.PlayOneShot (scaredSound);
				anim.Play (ANIM_SCARETARGET_HEART_ATTACK);
				break;
			case 3:
				//Gr8 scare
				emojiPopup.GetComponent<EmojiPopup> ().ShowRandomSuperScaredEmoji();
				audio.PlayOneShot (scaredSound);
				anim.Play (ANIM_SCARETARGET_SCARED);
				break;
			}

			Invoke ("Disappear", 2f);
			SetMoveDirection (Vector2.zero);
			GameManager.NotifyDeath (this);
		}
	}

	public void OnCollisionEnter2D(Collision2D col) {
		if (hasSightedPlayer || hasBeenScared) return;
		Player p = col.transform.GetComponent<Player> ();
		if (p != null) {
			GameManager.TriggerHeartAttack (this);
			fearCount = maximumFearThreshold;
			UserInterface.ShowTip (1);
			/*
			Vector2 playerPos = player.transform.position;
			Vector2 currPos = transform.position;
			Vector2 dir = currPos - playerPos;
			SetMoveDirection (dir);
			*/
		}
	}

	public void TriggerPlayerSighted() {
		if (!hasSightedPlayer && !hasBeenScared) {
			hasSightedPlayer = true;
			SetNextState(HumanState.HARDSTUNNED);
			GameManager.PlayerSighted ();
			Invoke ("Disappear", 2f);
			anim.Play (ANIM_DOWNWARD_IDLE);
			SetMoveDirection (Vector2.zero);
			Transform emojiPopup = Instantiate (emojiPopupPrefab, transform);
			emojiPopup.transform.localPosition = new Vector3 (0f, 1.71f, -2f);
			emojiPopup.GetComponent<EmojiPopup> ().ShowRandomDisappointedEmoji ();
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
		if (currState == HumanState.SCARED) {
			return;
		}
		SetSightDirection ();
		CheckPlayerAndAddFear ();
	}
	private void CheckPlayerAndAddFear() {
		if (hasSightedPlayer || hasBeenScared) {
			fearBar.gameObject.SetActive (false);
			return;
		}
		if (playerInRangeDetector.isPlayerInFearRange) {
			Vector2 playerPos = player.transform.position;
			Vector2 currPos = transform.position;
			float dist = (playerPos - currPos).magnitude;

			//Player close enough.. start gaining fear
			float ratio = 1f - (dist / maximumPlayerDistance);
			float fearToAdd = ratio * fearGainRate * Time.deltaTime;
			if (fearToAdd < 0) fearToAdd = 0;
			if (fearCount + fearToAdd >= maximumFearThreshold) {
				fearCount = maximumFearThreshold;
				GameManager.TriggerHeartAttack (this);
				UserInterface.ShowTip (4);
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

	private void Disappear() {
		gameObject.SetActive (false);
	}

}