using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
	IDLE = 0,
	INGAME = 1,
	GAMEOVER = 2,
}

public class GameController : MonoBehaviour {
	public static GameController Instance;
	public GameState gameState = GameState.IDLE;
	public AudioClip music;
	private BubbleController _bubbleController;
	private EnemySpawnerController _spawnerController;
	[SerializeField] private float _enemyFrequency = 1;
	[SerializeField] private float _enemyFrequencyIncrement;
	private float _enemyTimer;
	[SerializeField] private float _introDelay = 4;
	[SerializeField] private float _deathPause = 2;
	public static event Action OnGameStart;
	public static event Action OnPlayerDeath;
	public float totalGameTime;
	private float _gameTimer;
	private List<DevilController> _devils = new List<DevilController>();

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	private void Start() {
		_bubbleController = FindObjectOfType<BubbleController>();
		_spawnerController = FindObjectOfType<EnemySpawnerController>();

		IntroController.OnIntroOver += OnIntroOver;
		DevilController.DevilSpawned += OnDevilSpawned;
		DevilController.DevilKilled += OnDevilKilled;
	}

	private void OnDevilKilled(DevilController oldDevilController) {
		_devils.Remove(oldDevilController);
	}

	private void OnDevilSpawned(DevilController newDevilContoller) {
		_devils.Add(newDevilContoller);
	}

	private void OnDestroy() {
		IntroController.OnIntroOver -= OnIntroOver;
		DevilController.DevilSpawned -= OnDevilSpawned;
		DevilController.DevilKilled -= OnDevilKilled;
	}

	private void OnIntroOver() {
		IntroController.OnIntroOver -= OnIntroOver;
		gameState = GameState.INGAME;
		_spawnerController.SpawnEnemy((int)EnemyType.Spike);
		AudioManager.Instance.PlayMusic(music);
	}

	public Transform GetPlayerTransform() {
		return _bubbleController.transform;
	}

	private void Update() {
		switch (gameState) {
			case GameState.IDLE:
				break;
			case GameState.INGAME:

				_enemyTimer += Time.deltaTime;
				if (_enemyTimer > _enemyFrequency) {
					_enemyTimer = 0;
					_spawnerController.SpawnEnemy();
					_enemyFrequency -= _enemyFrequencyIncrement;
					if (_enemyFrequency < .3) _enemyFrequency = .3f;
				}
				_gameTimer += Time.deltaTime;
				break;
			case GameState.GAMEOVER:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}