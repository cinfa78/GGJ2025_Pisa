using UnityEngine;

public class SinMovement : MonoBehaviour{
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _amplitude;
    [SerializeField] private float _frequency;
    private float _offset;
    private Vector3 _defaultPosition;

    void Start(){
        _direction.Normalize();
        _defaultPosition = transform.localPosition;
        _offset = Random.Range(-1.0f, 1.0f);
    }

    private void Update(){
        transform.localPosition = _defaultPosition + _direction * (_amplitude * Mathf.Sin((Time.time + _offset)* _frequency) );
    }
}