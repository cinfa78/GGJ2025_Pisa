using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ShotController : MonoBehaviour{
    [FormerlySerializedAs("_direction")] public Vector3 direction = Vector3.zero;
    [SerializeField] private float _shotSpeed;
    [SerializeField] private AudioClip _shotSfx;
    private Rigidbody _rigidbody;

    private void Awake(){
        _rigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, 5);
    }

    private void OnEnable(){
        AudioManager.Instance.PlaySfx(_shotSfx);
    }

    private void Update(){
        transform.position += direction * (_shotSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other){
        if (other.gameObject.layer == LayerMask.NameToLayer("Grenade")){
            _rigidbody.useGravity = true;
        }
    }
}