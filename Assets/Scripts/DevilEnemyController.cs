using System;
using UnityEngine;

public interface IKillable{
    public void InstantKill();
}

public class DevilEnemyController : MonoBehaviour, IKillable{
    [SerializeField] protected float _movementSpeed;
    [SerializeField] protected Collider[] _colliders;

    [Header("Sfx")] [SerializeField] protected AudioClip _deathSfx;
    [Space] protected Rigidbody _rigidbody;
    protected Transform _playerTransform;
    protected bool _isAlive;
    protected int _grenadeLayer;
    public static event Action<DevilEnemyController> DevilSpawned;
    public static event Action<DevilEnemyController> DevilKilled;

    protected virtual void Awake(){
        _grenadeLayer = LayerMask.NameToLayer("Grenade");
    }

    protected virtual void OnEnable(){
        _isAlive = true;
        DevilSpawned?.Invoke(this);
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void OnDisable(){
        DevilKilled?.Invoke(this);
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