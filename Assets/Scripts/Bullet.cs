using System;
using UnityEngine;

public class Bullet : MonoBehaviour{
    [SerializeField] protected float _initialVelocity;
    [SerializeField] protected Vector3 _intitialDirection = Vector3.right;
    [SerializeField] protected float _damage = 1f;
    //

    [SerializeField] protected SphereCollider _collider;
    [SerializeField] protected AudioClip _launchSfx;

    public float GetDamage => _damage;
    protected Rigidbody _rigidbody;
    protected int _layerDevil;
    protected int _layerSpikes;

    private void Awake(){
        _rigidbody = GetComponent<Rigidbody>();
        _layerDevil = LayerMask.NameToLayer("Devil");
        _layerSpikes = LayerMask.NameToLayer("Spikes");
    }

    public void ApplyDirection(Vector3 startDirection){
        _intitialDirection = startDirection.normalized;
        _rigidbody.velocity = _intitialDirection * _initialVelocity;
        AudioManager.Instance.PlaySfx(_launchSfx);
    }

    protected virtual void OnCollisionEnter(Collision other){
        var otherLayer = other.gameObject.layer;
        if (otherLayer == _layerDevil || otherLayer == _layerSpikes){
            Destroy(gameObject);
        }
    }
}