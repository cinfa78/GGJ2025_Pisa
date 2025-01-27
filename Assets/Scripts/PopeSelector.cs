using System;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PopeSelector : MonoBehaviour{
    [SerializeField] private float _idleSpinSpeed = 3;
    [SerializeField] private float _spinTime = 3;
    [SerializeField] private float _choiseMadeTime = 1;
    [SerializeField] private AnimationCurve _spinCurve;
    [SerializeField] private SpriteRenderer[] _popesSprites;
    [SerializeField] private Color _darkerColor;
    [Header("Sfx")] [SerializeField] private AudioClip _spinSfx;
    [SerializeField] private AudioClip _choiceMadeSfx;
    [SerializeField] private UnityEvent _onChoiceMade;
    private bool _isSpinning;
    public static event Action OnSelectionStart;
    public static event Action OnSelectionDone;

    private IEnumerator Start(){
        transform.localRotation = Quaternion.Euler(0, 45 * Random.Range(0, 10), 0);
        ColorSprites(Color.clear, 0);
        yield return new WaitForSeconds(.1f);
        ColorSprites(Color.white, _choiseMadeTime);
    }

    [Button("Spin Election")]
    public void Spin(){
        OnSelectionStart?.Invoke();
        StopAllCoroutines();
        StartCoroutine(Spinning());
    }

    private void Update(){
        if (!_isSpinning){
            transform.Rotate(Vector3.up, _idleSpinSpeed * Time.deltaTime);
        }
    }

    private IEnumerator Spinning(){
        _isSpinning = true;
        int rounds = Random.Range(8, 14);
        float startRotationY = transform.localRotation.eulerAngles.y;
        float beep = 0;
        float nextBeep = 90;
        float timer = 0;
        while (timer < _spinTime){
            float t = timer / _spinTime;
            //t*t * (3f - 2f*t)
            float y = Mathf.LerpUnclamped(startRotationY, rounds * 360, _spinCurve.Evaluate(t));
            beep = y;
            if (beep >= nextBeep){
                nextBeep += 45;
                AudioManager.Instance.PlaySfx(_spinSfx);
            }

            transform.localRotation = Quaternion.AngleAxis(y, Vector3.up);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.identity;
        AudioManager.Instance.PlaySfx(_spinSfx);
        for (var i = 1; i < _popesSprites.Length; i++){
            _popesSprites[i].DOColor(_darkerColor, _choiseMadeTime);
        }

        _popesSprites[0].transform.DOScale(Vector3.one * 1.3f, _choiseMadeTime).OnComplete(() => {
            AudioManager.Instance.PlaySfx(_choiceMadeSfx);
            OnSelectionDone?.Invoke();
            _onChoiceMade.Invoke();
        });
    }

    private void ColorSprites(Color targetColor, float duration){
        foreach (var ps in _popesSprites){
            ps.DOKill();
            ps.DOColor(targetColor, duration);
        }
    }
}