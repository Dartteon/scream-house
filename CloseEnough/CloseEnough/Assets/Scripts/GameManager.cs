using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance { get; private set; }

	[SerializeField]
	private Transform[] stagePrefabs;
	[SerializeField]
	private Transform playerPrefab;
	[SerializeField]
	private Transform scareTargetPrefab;
	[SerializeField]
	private AudioClip coinSound;

	private AudioSource audio;

	private Player player;
	private List<ScareTarget> scareTargets = new List<ScareTarget>();
	private Transform currentStage;
	private Controller controller;
	private CameraManager cameraManager;

	private int currentScore = 0;
	private int currentCombo = 0;
	private float multiplierPerCombo = 0.2f;

	private int successCount = 0;

	private float elapsedTime = 0f;
	private float maxTime;

	private enum GameState { PreGame, DuringGame, PostGame };
	private GameState currState = GameState.PreGame;

	void Update () {
		if (currState != GameState.DuringGame) return;
		elapsedTime += Time.deltaTime;
		if (maxTime - elapsedTime <= 0) {
			EndGame ();
		}
		UserInterface.SetTimeLeft (GetTimeLeft ());
	}

	public static void SpacePressed() {
		if (instance.currState == GameState.DuringGame) {
			//During the game
			instance.player.ScareAllScareTargetsInRange ();
		} else if (instance.currState == GameState.PreGame) {
			//Start the game
			Restart ();
			instance.currState = GameState.DuringGame;
			instance.transform.Find ("TitleScreen").gameObject.SetActive (false);
			instance.transform.Find ("GameOverScreen").gameObject.SetActive (false);
			instance.transform.Find ("UserInterface").gameObject.SetActive (true);
		} else {
			//At end game screen
			instance.currState = GameState.PreGame;
			instance.transform.Find ("TitleScreen").gameObject.SetActive (true);
			instance.transform.Find ("TitleScreen").GetComponent<TitleScreen>().SetHighscoreBoard ();
			instance.transform.Find ("GameOverScreen").gameObject.SetActive (false);
			instance.transform.Find ("UserInterface").gameObject.SetActive (false);
			instance.StartGame ();
		}
	}

	private void EndGame() {
		currState = GameState.PostGame;
		Transform gameOverScreen = instance.transform.Find ("GameOverScreen");
		gameOverScreen.gameObject.SetActive (true);
		bool isNewHighScore = SaveManager.IsNewHighScore (currentScore);
		gameOverScreen.GetComponent<GameOverScreen> ().SetScore (currentScore, isNewHighScore);
		instance.currentStage.gameObject.SetActive (false);
		instance.transform.Find ("TitleScreen").gameObject.SetActive (false);
		instance.transform.Find ("UserInterface").gameObject.SetActive (false);
		instance.transform.Find ("TitleScreen").GetComponent<TitleScreen>().SetHighscoreBoard ();

	}

	private void ExtendTime (float duration, string description) {
		if (duration == 0) return;
		maxTime += duration;
		UserInterface.TriggerTimeExtension (duration, description);
	}

	public static void TogglePause() {
		if (Time.timeScale > 0) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
	}

	public static float GetTimeLeft() {
		return instance.maxTime - instance.elapsedTime;
	}

	public static void NotifyDeath(ScareTarget sT) {
		if (instance.scareTargets.Contains (sT)) {
			instance.scareTargets.Remove (sT);
		}
	}


	public static void Restart() {
		if (instance.currentStage != null)
			Destroy (instance.currentStage.gameObject);
		instance.Initialize ();
		instance.StartGame ();
	}

	private void Initialize() {
		currState = GameState.PreGame;
		instance = this;
		audio = transform.GetComponent<AudioSource> ();
	}

	private void StartGame() {
//		Debug.Log ("Game Started");
		currentStage = Instantiate (stagePrefabs [0], null);
		SpawnPlayer ();
		SpawnScareTargets ();
		SetController ();
		SetCamera ();
		SetUI ();
		currentCombo = 0;
		currentScore = 0;
		elapsedTime = 0;
		maxTime = GameSettings.gameTime;
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
		int numScareTargets = 1;
		float spawnRadius = 10f;
		for (int i = 0; i < GameSettings.startNumScareTargets; i++) {
			/*
			float xPos = Random.Range (GameSettings.xMinSpawnDist, GameSettings.boundaryDist);
			float yPos = Random.Range (GameSettings.yMinSpawnDist, GameSettings.boundaryDist);
			if (Random.Range (0, 2) == 1) xPos = -xPos;
			if (Random.Range (0, 2) == 1) yPos = -yPos;
			*/
			float xPos = (((i % 4) - 2) * 10f) + 5f;
			float yPos = (((i / 4) - 2) * 10f) + 5f;
			SpawnScareTarget (new Vector2(xPos, yPos));
		}
	}

	private void SpawnScareTarget (Vector2 pos) {
		Transform scareTargetTransform = Instantiate (scareTargetPrefab, currentStage);
		scareTargetTransform.localPosition = new Vector3 (pos.x, pos.y, -.1f);
		ScareTarget scareTarget = scareTargetTransform.GetComponent<ScareTarget> ();
		scareTarget.Initialize (player);
		scareTargets.Add (scareTarget);
	}


	void Start () {
		SaveManager.Load ();
		instance = this;
		Initialize ();
		SetController ();
		instance.transform.Find ("TitleScreen").GetComponent<TitleScreen>().SetHighscoreBoard ();
	}

	public static void TriggerScare(float fearCount, float distanceRatio, bool isOverMinimumRequiredThreshold, ScareTarget scareTarget) {
		
	}
	public void TriggerScare(List<ScareTarget> scareTargets) {
		
		int numSuccessfulScares = 0;
		int numUnderwhelmingScares = 0;
		int numHeartAttacks = 0;
		int numGreatScares = 0;
		int comboChange = 0;
		float pointReward = 0;
		float biggestDistanceRatio = 0f;
		foreach (ScareTarget scareTarget in scareTargets) {
			if (scareTarget.hasBeenScared || scareTarget.hasSightedPlayer) continue;
			Vector2 playerPos = player.transform.position;
			Vector2 scareTargetPos = scareTarget.transform.position;
			float dist = (playerPos - scareTargetPos).magnitude;
			instance.scareTargets.Remove (scareTarget);
			float distanceRatio = (dist / scareTarget.maximumPlayerDistance);
//			Debug.Log ("FOUND DISTANCE RATIO : " + distanceRatio);
			float inverseDistanceRatio = 1f - distanceRatio;
			if (distanceRatio > biggestDistanceRatio && distanceRatio < 1) {
				biggestDistanceRatio = distanceRatio;
			}
			bool isOverMinimumRequiredThreshold = (scareTarget.fearCount > scareTarget.minimumFearThreshold);
			bool isGreatScare = (scareTarget.fearCount > (scareTarget.maximumFearThreshold * .9f));
			float points = scareTarget.fearCount * inverseDistanceRatio;
			int scareType = 0;
			if (!isOverMinimumRequiredThreshold) {
//				Debug.LogError ("Underwhelming [" + scareTarget.fearCount + "/" + scareTarget.maximumFearThreshold + "]");
				scareType = 1;
			} else if (scareTarget.fearCount >= scareTarget.maximumFearThreshold) {
//				Debug.LogError ("Heart attack [" + scareTarget.fearCount + "/" + scareTarget.maximumFearThreshold + "]");
				scareType = 2;
			} else if (isGreatScare) {
				scareType = 3;
				successCount += 2;
				comboChange += 2;
				numGreatScares++;
			} else {
				scareType = 0;
				successCount++;
				comboChange++;
			}

			scareTarget.TriggerScare (scareType);
			pointReward += points;

			switch (scareType) {
			case 0:	//Normal scare
				numSuccessfulScares++;
				break;
			case 1:
				numUnderwhelmingScares++;
				if (!isOverMinimumRequiredThreshold) {
					instance.BadScare ();
				}
				break;
			case 2:
				numHeartAttacks++;
				break;
			case 3:
				numSuccessfulScares++;
				break;
			}

		};

		//Set points
		pointReward *= numSuccessfulScares*(1 + (multiplierPerCombo * currentCombo));
		instance.currentScore += (int)pointReward;
		int extraCombo = (numUnderwhelmingScares > 0 || numHeartAttacks > 0) ? 0 : numSuccessfulScares;
		UserInterface.SetScore (instance.currentScore);
		if ((int)pointReward > 0) {
			audio.PlayOneShot (coinSound);
			UserInterface.ShowScoreGain ((int)pointReward, extraCombo, (1 + (multiplierPerCombo * currentCombo)));
		}
		//Now set combos
		if (numSuccessfulScares > 1) {
			Debug.LogError ("TODO: Spawn MULTI call");
		}
		if (numUnderwhelmingScares > 0 || numHeartAttacks > 0) {
			currentCombo = 0;
			comboChange = 0;
		} else {
			if (numSuccessfulScares > 1) {
				currentCombo += extraCombo;
			}
			currentCombo += comboChange;
		}
		UserInterface.SetCombo (1 + (multiplierPerCombo * currentCombo));

		string timeExtendDescription = "";
		float bestRatio = biggestDistanceRatio;
//		Debug.Log ("BEST RATIO" + bestRatio);
		int timeExtentionMultiplier = 1;
//		Debug.Log (bestRatio);
		if (bestRatio > 0.7f) {
			timeExtentionMultiplier = 3;
			timeExtendDescription = "★★★";
		} else if (bestRatio > 0.5f) {
			timeExtentionMultiplier = 2;
			timeExtendDescription = "★★";
		} else {
			timeExtentionMultiplier = 1;
			timeExtendDescription = "★";
		}
		float timeToExtend = (numSuccessfulScares) * GameSettings.maxTimeExtentionPerSuccess * timeExtentionMultiplier;
		ExtendTime (timeToExtend, timeExtendDescription);

//		Debug.Log ("SUCCESS: " + numSuccessfulScares + " UNDERWHELMING: " + numUnderwhelmingScares + " HEARTATTACK: " + numHeartAttacks);
		instance.SpawnNewScareTargets ();
	}
	private void BadScare() {
		instance.SpawnNewScareTargets ();
		UserInterface.ShowTip (2);
	}
	public static void PlayerSighted() {
		instance.player.StunPlayer (1f);
		instance.SpawnNewScareTargets ();
		instance.currentCombo = 0;
		UserInterface.SetCombo (1 + (instance.multiplierPerCombo * instance.currentCombo));
		UserInterface.ShowTip (3);
	}

	public static void TriggerHeartAttack (ScareTarget scareTarget) {
		instance.currentCombo = 0;
		scareTarget.TriggerScare (2);
	}

	private void SpawnNewScareTargets() {

		int numToSpawn = 1 + (successCount / 5);
		if (numToSpawn > 3) numToSpawn = 3;

		Vector2 playerPos = player.transform.position;
		float displacement = 10;
		float xPos = 0, yPos = 0;
		float randomer = 3f;

		if (playerPos.x > 0) {
			//Player on RIGHT side
			xPos = Random.Range ((playerPos.x - GameSettings.xMinSpawnDist), -GameSettings.boundaryDist);
		} else {
			xPos = Random.Range ((playerPos.x + GameSettings.xMinSpawnDist), GameSettings.boundaryDist);
		}


		if (playerPos.y > 0) {
			//Player on TOP side
			yPos = Random.Range ((playerPos.y - GameSettings.yMinSpawnDist), -GameSettings.boundaryDist);
		} else {
			yPos = Random.Range ((playerPos.y + GameSettings.yMinSpawnDist), GameSettings.boundaryDist);
		}

		Transform scareTargetTransform = Instantiate (scareTargetPrefab, currentStage);
		scareTargetTransform.localPosition = new Vector3 (xPos, yPos, -.1f);
		ScareTarget scareTarget = scareTargetTransform.GetComponent<ScareTarget> ();
		scareTarget.Initialize (player);
		scareTargets.Add (scareTarget);
	}
}
