using UnityEngine;

public class GrenadeController : Bullet{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _splashVfx;
    [SerializeField] private Sprite[] _grenadeSprites;
    [Header("Sfx")] [SerializeField] private AudioClip _splashSfx;
    [SerializeField] private AudioClip _spikeHitSfx;


    private void Awake(){
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        _intitialDirection.Normalize();
        Destroy(gameObject, 3);
    }

    public void SetupSprite(int id){
        int i = id % _grenadeSprites.Length;
        _spriteRenderer.sprite = _grenadeSprites[i];
        _collider.radius -= (i + 1) * 0.1f;
    }


    protected override void OnCollisionEnter(Collision other){
        var otherLayer = other.gameObject.layer;
        if (otherLayer == _layerDevil || otherLayer == _layerSpikes){
            _splashVfx.transform.SetParent(null);
            _splashVfx.SetActive(true);
            AudioManager.Instance.PlaySfx(_splashSfx);
            if (otherLayer == _layerSpikes)
                AudioManager.Instance.PlaySfx(_spikeHitSfx);
            Destroy(gameObject);
        }
    }
}