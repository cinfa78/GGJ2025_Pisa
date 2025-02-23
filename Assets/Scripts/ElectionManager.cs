using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ElectionManager : MonoBehaviour{
    [SerializeField] private float _cameraStartY;
    [SerializeField] private float _cameraTargetY;
    [SerializeField] private float _panDuration = 5;
    [SerializeField] private float _fadeOutDuration = 2;
    [SerializeField] private CanvasGroup _popeBannerCanvasGroup;
    [SerializeField] private TMP_Text _popeNameLabel;
    public UnityEvent _onPanComplete;

    private Camera _camera;
    private bool _nameUpdated;
    private bool _loading;

    public static event Action OnPanStart;
    public static event Action<float> OnPlayAgain;
    private static string newPopeName;

    private void Awake(){
        _popeBannerCanvasGroup.alpha = 0;
        _camera = Camera.main;
        _camera.transform.position = new Vector3(_camera.transform.position.x, _cameraStartY, _camera.transform.position.z);
    }

    private void Start(){
        OnPanStart?.Invoke();
        _camera.transform.DOMoveY(_cameraTargetY, _panDuration).SetEase(Ease.InOutQuad).OnComplete(() => { _onPanComplete.Invoke(); });
        //aggiorno subito il nome del papa
        GenerateNewPopeName();
        _popeNameLabel.text = newPopeName;
    }

    public void ShowPopeName(){
        _popeBannerCanvasGroup.DOFade(1, 1);
    }

    public static void GenerateNewPopeName(){
        newPopeName = SaveManager.Instance.GenerateNewPope(null);
    }

    public void PlayAgain(){
        if (_loading) return;
        _loading = true;
        OnPlayAgain?.Invoke(_fadeOutDuration);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(_fadeOutDuration);
        sequence.AppendCallback(() => { SceneLoader.Game(); });
        sequence.Play();
    }

    private void Update(){
        if (Input.GetButtonDown("Cancel")){
            SceneLoader.MainMenu();
        }
    }
}