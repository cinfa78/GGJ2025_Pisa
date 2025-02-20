using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UiCardManager : MonoBehaviour{
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private DemonListSo.DemonData demonData;
    [SerializeField] private SpriteAnimator _chainsAnimator;
    [SerializeField] private SpriteAnimator _lockAnimator;
    [SerializeField] private GameObject _lockOpenObject;
    [Header("Sfx")] [SerializeField] private AudioClip _lockOpenSfx;
    [SerializeField] private AudioClip _chainsLockedSfx;
    [SerializeField] private AudioClip _chainsSfx;
    [SerializeField] private AudioClip _cardFlipSfx;

    private bool _isShowing;
    private bool _locked = true;
    private bool _toUnlock = true;
    private float _defaultZRotation;
    private bool _busy;
    private Coroutine _shakingOnUnlocked;
    private int _sinOffset;
    
    public DemonListSo.DemonData DemonData{
        get => demonData;
        set{
            demonData = value;
            UpdateCard();
        }
    }

    private void OnValidate(){
        UpdateCard();
    }

    private void Awake(){
        transform.localRotation = Quaternion.Euler(0, 180, 0);
        _defaultZRotation = transform.localRotation.z;
        _sinOffset = Mathf.Abs(System.Guid.NewGuid().GetHashCode()/1000000);
    }

    private void OnEnable(){
        if (!_locked){
            _toUnlock = false;
            _chainsAnimator.gameObject.SetActive(false);
            _lockAnimator.gameObject.SetActive(false);
            Flip(false);
        }
        else{
            _toUnlock = true;
            _chainsAnimator.OnLoopOver += OnChainAnimationOver;
            _lockAnimator.OnLoopOver += OnLockOpen;
        }
    }

    private void OnLockOpen(){
        _lockAnimator.OnLoopOver -= OnLockOpen;
        _lockAnimator.gameObject.SetActive(false);
        _lockOpenObject.SetActive(true);
        _lockOpenObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 30);
        Destroy(_lockOpenObject, 3f);
        _chainsAnimator.Toggle();
    }

    private void OnChainAnimationOver(){
        _chainsAnimator.OnLoopOver -= OnChainAnimationOver;

        _toUnlock = false;
        _busy = false;
        Flip();
    }

    [Button("Unlock")]
    public void UnlockCard(){
        _locked = false;
        _shakingOnUnlocked = StartCoroutine(ShakingOnUnlocked());
    }

    private void UpdateCard(){
        _nameLabel.text = demonData.name;
        _spriteRenderer.sprite = demonData.sprite;
    }

    private void OnMouseDown(){
        if (!_busy){
            if (!_locked){
                if (_toUnlock){
                    _busy = true;
                    StopCoroutine(_shakingOnUnlocked);
                    transform.localRotation = Quaternion.Euler(0f, 180, 0);
                    _lockAnimator.Toggle();
                    AudioManager.Instance.PlaySfx(_lockOpenSfx);
                    AudioManager.Instance.PlaySfx(_chainsSfx);
                }
                else{
                    Flip();
                }
            }
            else{
                StartCoroutine(Shaking());
            }
        }
    }

    [Button("Flip")]
    private void Flip(bool playSound = true){
        _busy = true;
        if (playSound)
            AudioManager.Instance.PlaySfx(_cardFlipSfx);
        _isShowing = !_isShowing;
        transform.DOKill();
        if (_isShowing){
            transform.DORotate(Vector3.up * 0, Random.Range(.5f, 1f)).OnComplete(() => _busy = false);
        }
        else{
            transform.DORotate(Vector3.up * 180, Random.Range(.5f, 1f)).OnComplete(() => _busy = false);
        }
    }

    private IEnumerator Shaking(){
        AudioManager.Instance.PlaySfx(_chainsLockedSfx);
        float timer = 0f;
        float startY = transform.localRotation.y;
        while (timer < .5f){
            transform.localRotation = Quaternion.Euler(0f, 180, Mathf.Sin((_sinOffset+Time.time) * 30f) * 5);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0f, 180, _defaultZRotation);
    }

    private IEnumerator ShakingOnUnlocked(){
        while (true){
            transform.localRotation = Quaternion.Euler(0f, 180, Mathf.Sin((_sinOffset+Time.time) * 30f) * 3);
            yield return null;
        }
    }
}