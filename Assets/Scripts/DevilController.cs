using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class DevilController : DevilEnemyController{
    private SpriteRenderer _baseSpriteRenderer;
    [SerializeField] private Sprite _deadDevilSprite;
    [Header("Shots")] public bool _canShoot;
    [SerializeField] private float _shotInterval;
    [SerializeField] private GameObject _bubbleShotPrefab;
    [SerializeField] private Sprite _shootingDevilSprite;

    [SerializeField, ReadOnly] private bool _hasTarget;
    private float _shotTimer;

    protected override void Awake(){
        base.Awake();
        _baseSpriteRenderer = _spriteContainer.GetComponent<SpriteRenderer>();
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

    private void Update(){
        if (_isAlive && _hasTarget){
            _rigidbody.velocity = (_playerTransform.position - transform.position).normalized * (_movementSpeed * Time.deltaTime);
            _spriteContainer.transform.localScale = !(_playerTransform.position.x > transform.position.x) ? Vector3.one : new Vector3(-1, 1, 1);
            if (_canShoot){
                _shotTimer += Time.deltaTime;
                if (_shotTimer >= _shotInterval){
                    _shotTimer = 0;
                    Shoot();
                }
            }
        }
    }

    private void Shoot(){
        var newShot = Instantiate(_bubbleShotPrefab, transform.position, Quaternion.identity);
        newShot.GetComponent<ShotController>().direction =
            (_playerTransform.position.x > transform.position.x) ? Vector3.right : Vector3.left;
    }

    private void OnCollisionEnter(Collision other){
        if (_isAlive){
            if (other.gameObject.layer == _grenadeLayer){
                _isAlive = false;
                DoDeath();
            }
        }
    }

    private void DoDeath(){
        _baseSpriteRenderer.sprite = _deadDevilSprite;
        DeathPhysics();
        AudioManager.Instance.PlaySfx(_deathSfx);
        Destroy(gameObject, 3f);
    }

    public override void InstantKill(){
        base.InstantKill();
        DoDeath();
    }
}