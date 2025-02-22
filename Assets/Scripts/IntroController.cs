using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class IntroController : MonoBehaviour{
    public CanvasGroup[] slides;
    public static event Action OnIntroOver;
    [SerializeField] private AudioClip _slideOverSfx;
    private Sequence _sequence;

    private void Awake(){
        for (int i = 0; i < slides.Length; i++){
            slides[i].alpha = 0;
        }
    }

    private void OnEnable(){
        slides[0].GetComponent<TMP_Text>().text += SaveManager.Instance.GetLastPopeName();
    }

    private void OnDestroy(){
        _sequence.Rewind();
    }

    private void Start(){
        _sequence = DOTween.Sequence();
        for (int i = 0; i < slides.Length; i++){
            _sequence.Append(slides[i].DOFade(1, .2f));
            _sequence.AppendInterval(2);
            _sequence.AppendCallback(() => { AudioManager.Instance.PlaySfx(_slideOverSfx); });
            _sequence.Append(slides[i].DOFade(0, .2f));
        }

        _sequence.OnComplete(() => {
            OnIntroOver?.Invoke();
            Destroy(gameObject);
        });
    }

    public void StopIntro(){
        _sequence.Rewind();
        OnIntroOver?.Invoke();
        Destroy(gameObject);
    }
}