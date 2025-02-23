using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class DevilController : EnemyController{
    private SpriteRenderer _baseSpriteRenderer;
    [SerializeField] private Sprite _deadDevilSprite;
    [SerializeField] private GameObject _wingsContainer;
    [SerializeField] private SpriteRenderer _wingSpriteRenderer;
    [SerializeField] private SpriteRenderer[] _deadSprites;
    [Header("Shots")] public bool _canShoot;
    [SerializeField] private float _shotInterval;
    [SerializeField] private GameObject _bubbleShotPrefab;
    [SerializeField] private Sprite _shootingDevilSprite;


    private float _shotTimer;

    protected override void Awake(){
        base.Awake();
        _baseSpriteRenderer = _spriteContainer.GetComponent<SpriteRenderer>();
        for (int i = 0; i < _deadSprites.Length; i++){
            _deadSprites[i].color = _wingSpriteRenderer.color;
            _deadSprites[i].enabled = false;
        }
    }

    private IEnumerator Start(){
        if (_canShoot){
            _baseSpriteRenderer.sprite = _shootingDevilSprite;
            _spikeCollider.enabled = false;
        }

        yield return null;
        _playerTransform = GameController.Instance.GetPlayerTransform();
        _hasTarget = _playerTransform != null;
    }

    protected override void Update(){
        base.Update();
        _shotTimer += Time.deltaTime;
    }


    protected override void Shoot(){
        if (_canShoot && _shotTimer >= _shotInterval){
            _shotTimer = 0;
            var newShot = Instantiate(_bubbleShotPrefab, transform.position, Quaternion.identity);
            newShot.GetComponent<ShotController>().direction = (_playerTransform.position.x > transform.position.x) ? Vector3.right : Vector3.left;
        }
    }

    private void OnCollisionEnter(Collision other){
        if (_isAlive && other.gameObject.layer == _grenadeLayer){
            _isAlive = false;
            DoDeath();
        }
    }

    public override void InstantKill(){
        base.InstantKill();
        DoDeath();
    }

    private void DoDeath(){
        //_baseSpriteRenderer.sprite = _deadDevilSprite;
        _baseSpriteRenderer.gameObject.SetActive(false);
        _wingsContainer.SetActive(false);
        for (int i = 0; i < _deadSprites.Length; i++){
            _deadSprites[i].enabled = true;
        }

        DeathPhysics();
        AudioManager.Instance.PlaySfx(_deathSfx);
        Destroy(gameObject, 3f);
    }
}