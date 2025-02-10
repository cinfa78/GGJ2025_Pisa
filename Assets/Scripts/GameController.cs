using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public enum GameState{
    IDLE = 0,
    INGAME = 1,
    GAMEOVER = 2,
    VICTORY = 3,
}

public class GameController : MonoBehaviour{
    public static GameController Instance;
    public GameState gameState = GameState.IDLE;
    public AudioClip music;
    public AudioClip _victoryMusic;
    private BubbleController _bubbleController;
    private EnemySpawnerController _spawnerController;
    [SerializeField] private float _enemyFrequency = 1;
    [SerializeField] private float _enemyFrequencyIncrement;
    [SerializeField] private GameObject _environment;
    //public float victoryAnimationTime = 5f;
    public float totalGameTime;
    public static int devilsKilled;
    // public CanvasGroup victoryCanvasGroup;
    // [FormerlySerializedAs("deferatCanvasGroup")] public CanvasGroup defeatCanvasGroup;

    public static event Action OnGameStart;
    public static event Action OnGameOver;
    public static event Action OnVictory;

    private float _enemyTimer;
    private float _gameTimer;
    private List<IKillable> _devils = new List<IKillable>();
    private bool _bossOut;
    private IntroController _introController;
    private bool _isLoading;

    private void Awake(){
        if (Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }

        devilsKilled = 0;
    }

    private void Start(){
        _bubbleController = FindObjectOfType<BubbleController>();
        _spawnerController = FindObjectOfType<EnemySpawnerController>();

        IntroController.OnIntroOver += OnIntroOver;
        _introController = FindObjectOfType<IntroController>();

        EnemyController.EnemySpawned += OnEnemySpawned;
        EnemyController.EnemyKilled += OnEnemyKilled;
        Boss.OnBossDeath += Victory;

        BubbleController.OnPlayerdeath += OnPlayerDeath;

        Cursor.visible = false;
    }

    private void OnEnemyKilled(IKillable killable){
        _devils.Remove(killable);
        devilsKilled++;
    }

    private void OnEnemySpawned(IKillable killable){
        _devils.Add(killable);
    }

    private void OnDestroy(){
        IntroController.OnIntroOver -= OnIntroOver;

        EnemyController.EnemySpawned -= OnEnemySpawned;
        EnemyController.EnemyKilled -= OnEnemyKilled;
        Boss.OnBossDeath -= Victory;

        BubbleController.OnPlayerdeath -= OnPlayerDeath;
    }

    [Button("Lose Now")]
    private void OnPlayerDeath(){
        Cursor.visible = true;
        BubbleController.OnPlayerdeath -= OnPlayerDeath;
        Destroy(_spawnerController.gameObject);
        gameState = GameState.GAMEOVER;
        
        OnGameOver?.Invoke();
    }

    [Button("Win Now")]
    private void Victory(){
        Cursor.visible = true;
        OnVictory?.Invoke();
        gameState = GameState.VICTORY;
        BubbleController.OnPlayerdeath -= OnPlayerDeath;
        Destroy(_spawnerController.gameObject);
        _bubbleController._godMode = true;
        _bubbleController.SetMovement(false);
        for (int i = _devils.Count - 1; i >= 0; i--){
            IKillable k = _devils[i];
            k.InstantKill();
        }

        
        _environment.transform.DOMoveY(-15, 5).SetEase(Ease.InOutCubic).OnComplete(() => {});
        AudioManager.Instance.PlayMusic(_victoryMusic);
        
    }

    private void OnIntroOver(){
        IntroController.OnIntroOver -= OnIntroOver;
        if (_bubbleController.IsAlive){
            gameState = GameState.INGAME;
            OnGameStart?.Invoke();
            _spawnerController.SpawnEnemy();
            AudioManager.Instance.PlayMusic(music);
        }
    }

    public Transform GetPlayerTransform(){
        return _bubbleController.transform;
    }

    private void Update(){
        switch (gameState){
            case GameState.IDLE:
                if (Input.GetButtonDown("Cancel"))
                    _introController.StopIntro();
                break;
            case GameState.INGAME:
                _enemyTimer += Time.deltaTime;
                if (_enemyTimer > _enemyFrequency && !_bossOut){
                    _enemyTimer = 0;
                    _spawnerController.SpawnEnemy();
                    _enemyFrequency -= _enemyFrequencyIncrement;
                    if (_enemyFrequency < 1f) _enemyFrequency = 1f;
                }

                _gameTimer += Time.deltaTime;
                if (_gameTimer > totalGameTime && !_bossOut){
                    _spawnerController.SpawnBoss();
                    _bossOut = true;
                }

                break;
            case GameState.GAMEOVER:
                if (Input.GetButtonDown("Cancel")){
                    ElectPope();
                }

                break;
            case GameState.VICTORY:
                if (Input.GetButtonDown("Cancel")){
                    RestartGame();
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void RestartGame(){
        if (!_isLoading){
            _isLoading = true;
            SceneManager.LoadScene(0);
        }
    }

    public void ElectPope(){
        if (!_isLoading){
            _isLoading = true;
            SceneManager.LoadScene(2);
        }
    }

    public void QuitGame(){
        Application.Quit();
    }
}