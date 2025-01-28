using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiElectionController : MonoBehaviour{
    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] private GameObject _playButton;
    [SerializeField] private CanvasGroup _fadeCanvasGroup;

    [SerializeField] private float _textDuration;

    private void Awake(){
        _titleLabel.maxVisibleCharacters = 0;
        ElectionManager.OnPanStart += OnPanStart;
        ElectionManager.OnPlayAgain += FadeOut;
        PopeSelector.OnSelectionStart += OnSelectionStart;
        PopeSelector.OnSelectionDone += OnSelectionDone;
    }


    private void OnDestroy(){
        ElectionManager.OnPanStart -= OnPanStart;
        ElectionManager.OnPlayAgain -= FadeOut;
        PopeSelector.OnSelectionStart -= OnSelectionStart;
        PopeSelector.OnSelectionDone -= OnSelectionDone;
    }

    private void OnSelectionStart(){
        SetTitleText("Habemus Papam");
        _textDuration = 2;
    }

    private void OnSelectionDone(){
        _playButton.SetActive(true);
        ShowTitle();
    }

    private void OnPanStart(){
        ShowTitle();
    }

    public void SetTitleText(string title){
        _titleLabel.text = title;
        _titleLabel.maxVisibleCharacters = 0;
    }

    [Button("ShowTitle")]
    public void ShowTitle(){
        StopAllCoroutines();
        StartCoroutine(ShowingTitle());
    }

    private IEnumerator ShowingTitle(){
        float timer = 0;
        while (timer < _textDuration){
            float t = timer / _textDuration;
            _titleLabel.maxVisibleCharacters = (int)(t * _titleLabel.text.Length);
            timer += Time.deltaTime;
            yield return null;
        }

        _titleLabel.maxVisibleCharacters = _titleLabel.text.Length;
    }

    public void FadeOut(float fadeDuration){
        Cursor.visible = false;
        ElectionManager.OnPlayAgain -= FadeOut;
        StopAllCoroutines();
        _playButton.SetActive(false);
        _fadeCanvasGroup.DOFade(1, fadeDuration).OnComplete(() => { SceneManager.LoadScene(1); });
    }
}