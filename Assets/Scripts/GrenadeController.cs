using System;
using UnityEngine;

public class GrenadeController : MonoBehaviour{
    [SerializeField] private Vector3 _intitialDirection = Vector3.right;
    [SerializeField] private float _initialVelocity;
    [SerializeField] private GameObject _splashVfx;
    [Header("Sfx")] [SerializeField] private AudioClip _launchSfx;
    [SerializeField] private AudioClip _splashSfx;
    [SerializeField] private AudioClip _spikeHitSfx;
    private Rigidbody _rigidbody;
    private int _layerDevil;
    private int _layerSpikes;

    private void Awake(){
        _layerDevil = LayerMask.NameToLayer("Devil");
        _layerSpikes = LayerMask.NameToLayer("Spikes");
        _rigidbody = GetComponent<Rigidbody>();
        _intitialDirection.Normalize();
        Destroy(gameObject, 3);
    }

    public void ApplyDirection(Vector3 startDirection){
        _intitialDirection = startDirection.normalized;
        _rigidbody.velocity = _intitialDirection * _initialVelocity;
        AudioManager.Instance.PlaySfx(_launchSfx);
    }

    private void OnCollisionEnter(Collision other){
        var otherLayer = other.gameObject.layer;
        if (other.gameObject.layer == _layerDevil || other.gameObject.layer == _layerSpikes){
            _splashVfx.transform.SetParent(null);
            _splashVfx.SetActive(true);
            AudioManager.Instance.PlaySfx(_splashSfx);
            if (otherLayer == _layerSpikes)
                AudioManager.Instance.PlaySfx(_spikeHitSfx);
            Destroy(gameObject);
        }
    }
}