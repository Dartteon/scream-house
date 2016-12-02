using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
	public static CameraManager instance { get; private set; }

	[SerializeField]
	private float panSpeed;

	private Vector3 realPos;
	private Vector3 offset;
	private Transform followingObject;
	private Vector3 displacement;

	private bool isShaking;
	private float intensity = .5f;
	private float shakingStartTime;
	private float shakeDuration;

	public void Initialize() {
		instance = this;
	}

	void Update() {
		if (followingObject == null) return;

		Vector2 followingObjectPos = followingObject.transform.position;
		Vector2 targetPos = new Vector2 (followingObjectPos.x, followingObjectPos.y);
		float multiplier = Time.deltaTime * panSpeed;
		Vector2 newPos = Vector2.Lerp (realPos, targetPos, multiplier);
		realPos = new Vector3 (newPos.x, newPos.y, transform.position.z);

		if (isShaking) {
			if (Time.time - shakingStartTime >= shakeDuration) {
				isShaking = false;
				displacement = Vector3.zero;
			} else {
				displacement = new Vector3 (Random.Range (-intensity, intensity), Random.Range (-intensity, intensity), 0f);
			}
		} else {
			displacement = Vector3.zero;
		}
		transform.position = realPos + displacement;

		//		Vector2 currPos = transform.position;
//		Vector2 dir = targetPos - currPos;
//		float dist = dir.magnitude;
	}
	public void Follow(Transform target) {
		followingObject = target;
	}

	public static void CameraShake (float intensity, float duration) {
		instance.shakingStartTime = Time.time;
		instance.intensity = intensity;
		instance.shakeDuration = duration;
		instance.isShaking = true;
	}
}
