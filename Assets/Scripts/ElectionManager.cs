using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ElectionManager : MonoBehaviour{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _cameraStartY;
    [SerializeField] private float _cameraTargetY;
    [SerializeField] private float _panDuration = 5;
    [SerializeField] private float _fadeOutDuration = 2;
    public UnityEvent _onPanComplete;
    private bool _loading;
    public static event Action OnPanStart;
    public static event Action<float> OnPlayAgain;

    private void Awake(){
        _camera = Camera.main;
        _camera.transform.position = new Vector3(_camera.transform.position.x, _cameraStartY, _camera.transform.position.z);
    }

    private void Start(){
        OnPanStart?.Invoke();
        _camera.transform.DOMoveY(_cameraTargetY, _panDuration).SetEase(Ease.InOutQuad).OnComplete(() => { _onPanComplete.Invoke(); });
    }

    public void PlayAgain(){
        if (_loading) return;
        _loading = true;
        OnPlayAgain?.Invoke(_fadeOutDuration);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(_fadeOutDuration);
        sequence.AppendCallback(() => { SceneManager.LoadScene(1); });
        sequence.Play();
    }

    private void Update(){
        if (Input.GetButtonDown("Cancel")){
            PlayAgain();
        }
    }
}