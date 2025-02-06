using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class DevilBoss : DevilEnemyController{
    [SerializeField] private Transform[] _spikeNozzles;
    [SerializeField] private GameObject _spikeBulletPrefab;
    private int _spikesToShoot;
    [SerializeField] private float _shotInterval = 1;
    private float _shotTimer;

    [SerializeField] private Transform[] _bubbleNozzle;
    [SerializeField] private GameObject _bubbleBulletPrefab;
    private int _bubblesToShoot;
    [SerializeField] private int _health;

    [SerializeField] private AudioClip _bossHurtSfx;
    public static event Action OnBossDeath;

    [SerializeField] private float _targetX;
    private bool _hasPlayerTransform;

    private void Update(){
        if (_isAlive){
            if (!_hasPlayerTransform){
                _playerTransform = GameController.Instance.GetPlayerTransform();
                if (_playerTransform != null)
                    _hasPlayerTransform = true;
            }

            _shotTimer += Time.deltaTime;
            if (_shotTimer >= _shotInterval){
                Shoot();
                _shotTimer = 0;
            }

            if (transform.position.x > _targetX) transform.position += Vector3.left * (_movementSpeed * Time.deltaTime);
            if (transform.position.y < _playerTransform.position.y) transform.position += Vector3.up * (_movementSpeed * Time.deltaTime);
            if (transform.position.y > _playerTransform.position.y) transform.position += Vector3.down * (_movementSpeed * Time.deltaTime);
        }
    }

    private void Shoot(){
        var rnd = Random.Range(0, 1f);
        if (rnd < 0.5f){
            for (int i = 0; i < _spikesToShoot; i++){
                var newSpike = Instantiate(_spikeBulletPrefab, _spikeNozzles[i].position, _spikeNozzles[i].rotation);
            }

            _spikesToShoot++;
            if (_spikesToShoot > _spikeNozzles.Length)
                _spikesToShoot = _spikeNozzles.Length;
        }
        else{
            for (int i = 0; i < _bubblesToShoot; i++){
                var newBubble = Instantiate(_bubbleBulletPrefab, _bubbleNozzle[i].position, _bubbleNozzle[i].rotation);
                newBubble.name = "Bubble" + i;
            }

            _bubblesToShoot++;
            if (_bubblesToShoot > _bubbleNozzle.Length)
                _bubblesToShoot = _bubbleNozzle.Length;
        }
    }

    private void OnCollisionEnter(Collision other){
        if (_isAlive){
            if (other.gameObject.layer == _grenadeLayer){
                _health--;
                if (_health <= 0){
                    _isAlive = false;
                    OnBossDeath?.Invoke();
                    DoDeath();
                }
                else{
                    AudioManager.Instance.PlaySfx(_bossHurtSfx);
                }
            }
        }
    }

    private void DoDeath(){
        DeathPhysics();
        _rigidbody.AddForce(Vector3.up * 3f, ForceMode.Impulse);
        AudioManager.Instance.PlaySfx(_deathSfx);
    }

    [Button("Kill")]
    public void Kill(){
        _isAlive = false;
        DoDeath();
    }

    public override void InstantKill(){
        base.InstantKill();
        DoDeath();
    }
}