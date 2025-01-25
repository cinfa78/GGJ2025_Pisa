using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {
	IDLE = 0,
	INGAME = 1,
	GAMEOVER = 2,
	VICTORY = 3,
}

public class GameController : MonoBehaviour {
	public static GameController Instance;
	public GameState gameState = GameState.IDLE;
	public AudioClip music;
	public AudioClip _victoryMusic;
	private BubbleController _bubbleController;
	private EnemySpawnerController _spawnerController;
	[SerializeField] private float _enemyFrequency = 1;
	[SerializeField] private float _enemyFrequencyIncrement;
	private float _enemyTimer;
	[SerializeField] private float _introDelay = 4;
	[SerializeField] private float _deathPause = 2;
	[SerializeField] private GameObject _environment;
	public static event Action OnGameStart;
	public float totalGameTime;
	private float _gameTimer;
	private List<IKillable> _devils = new List<IKillable>();

	public static event Action OnVictory;
	public CanvasGroup victoryCanvasGroup;
	public CanvasGroup deferatCanvasGroup;
	public static event Action OnGameOver;
	private bool _bossOut;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
		victoryCanvasGroup.alpha = 0;
		victoryCanvasGroup.blocksRaycasts = false;
		victoryCanvasGroup.interactable = false;
		deferatCanvasGroup.alpha = 0;
		deferatCanvasGroup.blocksRaycasts = false;
		deferatCanvasGroup.interactable = false;
	}

	private void Start() {
		_bubbleController = FindObjectOfType<BubbleController>();
		_spawnerController = FindObjectOfType<EnemySpawnerController>();

		IntroController.OnIntroOver += OnIntroOver;
		DevilController.DevilSpawned += OnDevilSpawned;
		DevilController.DevilKilled += OnDevilKilled;

		DevilBottomController.DevilSpawned += OnDevilSpawned;
		DevilBottomController.DevilKilled += OnDevilKilled;


		DevilBoss.OnBossDeath += Victory;

		BubbleController.OnPlayerdeath += OnPlayerDeath;

		Cursor.visible = false;
	}

	private void OnDevilKilled(IKillable killable) {
		_devils.Remove(killable);
	}

	private void OnDevilSpawned(IKillable killable) {
		_devils.Add(killable);
	}

	private void OnDestroy() {
		IntroController.OnIntroOver -= OnIntroOver;
		DevilController.DevilSpawned -= OnDevilSpawned;
		DevilController.DevilKilled -= OnDevilKilled;


		DevilBoss.OnBossDeath -= Victory;
	}

	[Button("Lose Now")]
	private void OnPlayerDeath() {
		Cursor.visible = true;
		BubbleController.OnPlayerdeath -= OnPlayerDeath;
		Destroy(_spawnerController.gameObject);
		gameState = GameState.GAMEOVER;
		deferatCanvasGroup.DOFade(1, 5);
		deferatCanvasGroup.interactable = true;
		deferatCanvasGroup.blocksRaycasts = true;
		OnGameOver?.Invoke();
	}

	[Button("Win Now")]
	private void Victory() {
		Cursor.visible = true;
		gameState = GameState.VICTORY;
		BubbleController.OnPlayerdeath -= OnPlayerDeath;
		Destroy(_spawnerController.gameObject);
		_bubbleController._godMode = true;
		_bubbleController.SetMovement(false);
		victoryCanvasGroup.DOFade(1, 5);
		victoryCanvasGroup.interactable = true;
		victoryCanvasGroup.blocksRaycasts = true;
		_environment.transform.DOMoveY(-15, 5).SetEase(Ease.InOutCubic).OnComplete(() => { OnVictory?.Invoke(); });
		AudioManager.Instance.PlayMusic(_victoryMusic);
	}

	private void OnIntroOver() {
		IntroController.OnIntroOver -= OnIntroOver;
		if (_bubbleController.IsAlive) {
			gameState = GameState.INGAME;

			_spawnerController.SpawnEnemy((int)EnemyType.Spike);
			AudioManager.Instance.PlayMusic(music);
		}
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
				if (_enemyTimer > _enemyFrequency && !_bossOut) {
					_enemyTimer = 0;
					_spawnerController.SpawnEnemy();
					_enemyFrequency -= _enemyFrequencyIncrement;
					if (_enemyFrequency < 1f) _enemyFrequency = 1f;
				}
				_gameTimer += Time.deltaTime;
				if (_gameTimer > totalGameTime && !_bossOut) {
					_spawnerController.SpawnBoss();
					_bossOut = true;
				}
				break;
			case GameState.GAMEOVER:
				break;
			case GameState.VICTORY:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void RestartGame() {
		SceneManager.LoadScene(0);
	}
}