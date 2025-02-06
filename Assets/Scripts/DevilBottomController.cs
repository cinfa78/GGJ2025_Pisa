using System;
using UnityEngine;


public class DevilBottomController : DevilEnemyController{
    //[SerializeField] private float _movementSpeed = 10;
    [SerializeField] private GameObject _spikePrefab;
    [SerializeField] private float _shotFrequency;
    [Header("Graphics")] [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _deadDevilSprite;

    private bool _hasPlayerTransform;
    private float _shotTimer;
    private float _defaultYPosition;

    protected override void Awake(){
        base.Awake();
        _defaultYPosition = transform.position.y;
    }

    private void Update(){
        if (_isAlive){
            if (!_hasPlayerTransform){
                _playerTransform = GameController.Instance.GetPlayerTransform();
                _hasPlayerTransform = true;
            }

            _shotTimer += Time.deltaTime;
            if (_shotTimer > _shotFrequency){
                _shotTimer = 0;
                Shoot();
            }

            if (transform.position.x < _playerTransform.position.x){
                transform.position += Vector3.right * (_movementSpeed * Time.deltaTime);
            }
            else if (transform.position.x > _playerTransform.position.x){
                transform.position += Vector3.left * (_movementSpeed * Time.deltaTime);
            }

            if (transform.position.y < _defaultYPosition){
                transform.position += Vector3.up * (_movementSpeed * Time.deltaTime);
            }
            else{
                transform.position += Vector3.down * (_movementSpeed * Time.deltaTime);
            }
        }
    }

    private void Shoot(){
        Instantiate(_spikePrefab, transform.position, Quaternion.identity);
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
        _spriteRenderer.sprite = _deadDevilSprite;
        DeathPhysics();
        AudioManager.Instance.PlaySfx(_deathSfx);
        Destroy(gameObject, 3f);
    }

    public override void InstantKill(){
        base.InstantKill();
        DoDeath();
    }
}