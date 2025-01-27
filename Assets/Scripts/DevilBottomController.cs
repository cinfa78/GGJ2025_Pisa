using System;
using UnityEngine;

public interface IKillable{
    public void Kill();
}

public class DevilBottomController : MonoBehaviour, IKillable{
    [SerializeField] private float _movementSpeed = 10;
    [SerializeField] private GameObject _spikePrefab;
    [SerializeField] private float _shotFrequency;
    [Header("Graphics")] [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _deadDevilSprite;
    [Header("Sfx")] [SerializeField] private AudioClip _deathSfx;
    private Rigidbody _rigidbody;
    private Transform _playerTransform;
    private bool _hasPlayerTransform;
    private bool _isAlive;
    private float _shotTimer;

    public static event Action<IKillable> DevilSpawned;
    public static event Action<IKillable> DevilKilled;

    private void Awake(){
        _rigidbody = GetComponent<Rigidbody>();
        _isAlive = true;
    }

    private void OnEnable(){
        DevilSpawned?.Invoke(this);
    }

    private void OnDestroy(){
        DevilKilled?.Invoke(this);
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
        }
    }

    private void Shoot(){
        Instantiate(_spikePrefab, transform.position, Quaternion.identity);
    }

    private void OnCollisionEnter(Collision other){
        if (_isAlive){
            if (other.gameObject.layer == LayerMask.NameToLayer("Grenade")){
                DoDeath();
            }
        }
    }

    private void DoDeath(){
        _isAlive = false;
        AudioManager.Instance.PlaySfx(_deathSfx);
        _spriteRenderer.sprite = _deadDevilSprite;
        _rigidbody.useGravity = true;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 3f);
    }

    public void Kill(){
        DoDeath();
    }
}