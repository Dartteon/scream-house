using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	private static GameManager instance;

	[SerializeField]
	private Transform[] stagePrefabs;
	[SerializeField]
	private Transform playerPrefab;
	[SerializeField]
	private Transform scareTargetPrefab;

	private Player player;
	private List<ScareTarget> scareTargets = new List<ScareTarget>();
	private Transform currentStage;
	private Controller controller;
	private CameraManager cameraManager;

	private int currentScore = 0;
	private int currentCombo = 0;
	private float multiplier = 1f;

	private void Initialize() {
		instance = this;
		currentStage = Instantiate (stagePrefabs [0], null);
		SpawnPlayer ();
		SpawnScareTargets ();
		SetController ();
		SetCamera ();
		SetUI ();
	}

	private void SetUI() {
		transform.Find ("UserInterface").GetComponent<UserInterface> ().Initialize ();
	}

	private void SpawnPlayer() {
		Transform playerTransform = Instantiate (playerPrefab, currentStage);
		playerTransform.transform.localPosition = new Vector3 (0f, 0f, -.1f);
		player = playerTransform.GetComponent<Player> ();
		player.Initialize ();
	}
	private void SetController() {
		controller = transform.GetComponent<Controller> ();
		controller.Initialize (player);
	}
	private void SetCamera() {
		cameraManager = transform.GetComponent<CameraManager> ();
		cameraManager.Initialize ();
		cameraManager.Follow (player.transform);
	}

	private void SpawnScareTargets() {
		int numScareTargets = 5;
		float spawnRadius = 15f;
		for (int i = 0; i < numScareTargets; i++) {
			Transform scareTargetTransform = Instantiate (scareTargetPrefab, currentStage);
			scareTargetTransform.localPosition = new Vector3 (
				Random.Range (-spawnRadius, spawnRadius), Random.Range (-spawnRadius, spawnRadius), -.1f);
			ScareTarget scareTarget = scareTargetTransform.GetComponent<ScareTarget> ();
			scareTarget.Initialize (player);
			scareTargets.Add (scareTarget);
		}
	}


	void Start () {
		Initialize ();
	}

	public static void TriggerScare(float fearCount, float distanceRatio) {
		float points = fearCount * distanceRatio * instance.multiplier;
		instance.currentScore += (int)points;
		Debug.Log ("Gained Points [" + points + "]");
		Debug.Log ("Total Points: " + instance.currentScore);

		UserInterface.SetScore (instance.currentScore);
		instance.currentCombo++;
	}
}
