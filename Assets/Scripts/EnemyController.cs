using System;
using NaughtyAttributes;
using UnityEngine;

public interface IKillable{
    public void InstantKill();
}

public class EnemyController : MonoBehaviour, IKillable{
    [SerializeField] protected GameObject _spriteContainer;
    [Space] [SerializeField] protected float _movementSpeed;
    [SerializeField] protected Collider[] _colliders;
    [SerializeField] protected Collider _spikeCollider;

    [Header("Sfx")] [SerializeField] protected AudioClip _deathSfx;
    [Space] protected Rigidbody _rigidbody;
    protected Transform _playerTransform;
    protected bool _isAlive;
    protected int _grenadeLayer;
    [SerializeField, ReadOnly] protected bool _hasTarget;
    public static event Action<EnemyController> EnemySpawned;
    public static event Action<EnemyController> EnemyKilled;

    protected virtual void Awake(){
        _grenadeLayer = LayerMask.NameToLayer("Grenade");
    }

    protected virtual void OnEnable(){
        _isAlive = true;
        EnemySpawned?.Invoke(this);
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void OnDisable(){
        EnemyKilled?.Invoke(this);
    }

    protected virtual void Update(){
        if (_isAlive && _hasTarget){
            _rigidbody.velocity = (_playerTransform.position - transform.position).normalized * (_movementSpeed * Time.deltaTime);
            _spriteContainer.transform.localScale = !(_playerTransform.position.x > transform.position.x) ? Vector3.one : new Vector3(-1, 1, 1);
            Shoot();
        }
    }

    protected virtual void Shoot(){
    }

    protected void DeathPhysics(){
        _rigidbody.useGravity = true;
        foreach (var c in _colliders){
            c.enabled = false;
        }
    }

    public virtual void InstantKill(){
        if (_isAlive){
            _isAlive = false;
        }
    }
}