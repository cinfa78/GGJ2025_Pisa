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
    [SerializeField] private TMP_Text _popeNameLabel;
    public UnityEvent _onPanComplete;
    private Camera _camera;
    private bool _nameUpdated;
    private bool _loading;
    public static event Action OnPanStart;
    public static event Action<float> OnPlayAgain;

    private static string[] _popeNames = new string[]{
        "Alexander",
        "Bonifacius",
        "Celestinus",
        "Clementius",
        "Eugenius",
        "Gregorius",
        "Hadrianus",
        "Innocentius",
        "Laurentius",
        "Leo",
        "Leoninus",
        "Marcus",
        "Maximilianus",
        "Nicolaus",
        "Paulus",
        "Petrus",
        "Pius",
        "Raphael",
        "Silvester",
        "Sextus",
        "Stefanus",
        "Theodorus",
        "Urbanus"
    };

    private static string newName;

    private void Awake(){
        _camera = Camera.main;
        _camera.transform.position = new Vector3(_camera.transform.position.x, _cameraStartY, _camera.transform.position.z);
        _popeNameLabel.enabled = false;
    }

    private void Start(){
        OnPanStart?.Invoke();
        _camera.transform.DOMoveY(_cameraTargetY, _panDuration).SetEase(Ease.InOutQuad).OnComplete(() => { _onPanComplete.Invoke(); });
        //aggiorno subito il nome del papa
        UpdateSavedName();
        _popeNameLabel.text = newName;
    }

    public void ShowPopeName(){
        _popeNameLabel.enabled = true;
    }

    public static void UpdateSavedName(){
        newName = _popeNames[UnityEngine.Random.Range(0, _popeNames.Length)];
        SaveManager.Instance.AddPopeName(newName);
        newName = newName + " " + SaveManager.Instance.GetPopeNumber(newName);
        SaveManager.Instance.Save();
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