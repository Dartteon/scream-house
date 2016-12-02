using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour {
	protected Vector2 moveDirection;
	[SerializeField]
	protected float moveSpeed;

	protected Animator anim;

	protected string currAnimation;

	protected string ANIM_DOWNWARD_MOVING = "";
	protected string ANIM_DOWNWARD_IDLE = "";
	protected string ANIM_UPWARD_MOVING = "";
	protected string ANIM_UPWARD_IDLE = "";
	protected string ANIM_RIGHTWARD_MOVING = "";
	protected string ANIM_RIGHTWARD_IDLE = "";
	protected string ANIM_LEFTWARD_MOVING = "";
	protected string ANIM_LEFTWARD_IDLE = "";

	public enum Direction { UP, DOWN, LEFT, RIGHT }
	protected Direction currDirection = Direction.DOWN;

	public enum HumanState { IDLE, MOVING, SCARING, SCARED }
	protected HumanState currState;

	public virtual void Initialize() {

	}

	void Update() {
		SetState ();
		SetMovement ();
		SetAnimations ();
		CallExtraUpdates ();
	}
	protected virtual void CallExtraUpdates() {

	}
	protected void SetState() {
		if (moveDirection.magnitude > 0) {
			currState = HumanState.MOVING;
		} else {
			currState = HumanState.IDLE;
		}
	}

	protected void SetMovement() {
		Vector2 displacement2 = Time.deltaTime * moveSpeed * (moveDirection.normalized);
		Vector3 displacement3 = new Vector3 (displacement2.x, displacement2.y, 0f);
		transform.localPosition = transform.localPosition + displacement3;

		switch (currDirection) {
		case Direction.UP:
			if (moveDirection.y == 0) {
				if (moveDirection.x > 0)
					currDirection = Direction.RIGHT;
				else if (moveDirection.x < 0)
					currDirection = Direction.LEFT;
			} else if (moveDirection.y < 0) {
				currDirection = Direction.DOWN;
			}
			break;
		case Direction.DOWN:
			if (moveDirection.y == 0) {
				if (moveDirection.x > 0)
					currDirection = Direction.RIGHT;
				else if (moveDirection.x < 0)
					currDirection = Direction.LEFT;
			} else if (moveDirection.y > 0) {
				currDirection = Direction.UP;
			}
			break;
		case Direction.RIGHT:
			if (moveDirection.x == 0) {
				if (moveDirection.y > 0)
					currDirection = Direction.UP;
				else if (moveDirection.y < 0)
					currDirection = Direction.DOWN;
			} else if (moveDirection.x < 0) {
				currDirection = Direction.LEFT;
			}
			break;
		case Direction.LEFT:
			if (moveDirection.x == 0) {
				if (moveDirection.y > 0)
					currDirection = Direction.UP;
				else if (moveDirection.y < 0)
					currDirection = Direction.DOWN;
			} else if (moveDirection.x > 0) {
				currDirection = Direction.RIGHT;
			}
			break;
		}
	}

	public void SetMoveDirection (Vector2 dir) {
		moveDirection = dir;
	}

	protected void SetAnimations() {
		if (anim == null) anim = transform.Find ("Sprite").GetComponent<Animator> ();
		if (anim == null) return;

		if (currState == HumanState.IDLE || currState == HumanState.MOVING) {
			CheckMovingOrIdleAnimation ();
		}
	}
	protected void CheckMovingOrIdleAnimation() {
		if (moveDirection.magnitude > 0) {
			//Moving
			switch (currDirection) {
			case Direction.UP:
				if (currAnimation != ANIM_UPWARD_MOVING) {
					currAnimation = ANIM_UPWARD_MOVING;
					anim.Play (ANIM_UPWARD_MOVING);
				}
				break;
			case Direction.DOWN:
				if (currAnimation != ANIM_DOWNWARD_MOVING) {
					currAnimation = ANIM_DOWNWARD_MOVING;
					anim.Play (ANIM_DOWNWARD_MOVING);
				}
				break;
			case Direction.LEFT:
				if (currAnimation != ANIM_LEFTWARD_MOVING) {
					currAnimation = ANIM_LEFTWARD_MOVING;
					anim.Play (ANIM_LEFTWARD_MOVING);
				}
				break;
			case Direction.RIGHT:
				if (currAnimation != ANIM_RIGHTWARD_MOVING) {
					currAnimation = ANIM_RIGHTWARD_MOVING;
					anim.Play (ANIM_RIGHTWARD_MOVING);
				}
				break;
			}

		} else {
			//Idle
			switch (currDirection) {
			case Direction.UP:
				if (currAnimation != ANIM_UPWARD_IDLE) {
					currAnimation = ANIM_UPWARD_IDLE;
					anim.Play (ANIM_UPWARD_IDLE);
				}
				break;
			case Direction.DOWN:
				if (currAnimation != ANIM_DOWNWARD_IDLE) {
					currAnimation = ANIM_DOWNWARD_IDLE;
					anim.Play (ANIM_DOWNWARD_IDLE);
				}
				break;
			case Direction.LEFT:
				if (currAnimation != ANIM_LEFTWARD_IDLE) {
					currAnimation = ANIM_LEFTWARD_IDLE;
					anim.Play (ANIM_LEFTWARD_IDLE);
				}
				break;
			case Direction.RIGHT:
				if (currAnimation != ANIM_RIGHTWARD_IDLE) {
					currAnimation = ANIM_RIGHTWARD_IDLE;
					anim.Play (ANIM_RIGHTWARD_IDLE);
				}
				break;
			}

		}
	}
}
