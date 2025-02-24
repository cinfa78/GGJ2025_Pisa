using UnityEngine;

public class ShotController : MonoBehaviour{
    [SerializeField] private Collider _collider;
    public Vector3 direction = Vector3.zero;
    [SerializeField] private float _shotSpeed;
    [SerializeField] private AudioClip _shotSfx;
    private Rigidbody _rigidbody;
    private int _grenadeLayer;
    private int _popeLayer;


    private void Awake(){
        _rigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, 5);
        _grenadeLayer = LayerMask.NameToLayer("Grenade");
        _popeLayer = LayerMask.NameToLayer("Pope");
    }

    private void OnEnable(){
        AudioManager.Instance.PlaySfx(_shotSfx);
    }

    private void Update(){
        transform.position += direction * (_shotSpeed * Time.deltaTime);
    }

    public void DeactivateShot(){
        _rigidbody.useGravity = true;
        _collider.enabled = false;
    }

    private void OnCollisionEnter(Collision other){
        if (other.gameObject.layer == _grenadeLayer || other.gameObject.layer == _popeLayer){
            DeactivateShot();
        }
    }
}