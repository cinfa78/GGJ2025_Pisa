using System;
using DG.Tweening;
using UnityEngine;

public class UiMenuController : MonoBehaviour{
    [SerializeField] private CanvasGroup _mainMenuCanvasGroup;
    [SerializeField] private CanvasGroup _instructionsCanvasGroup;

    private void Awake(){
        _mainMenuCanvasGroup.alpha = 0f;
        _mainMenuCanvasGroup.interactable = false;
        _mainMenuCanvasGroup.blocksRaycasts = false;

        _instructionsCanvasGroup.alpha = 0f;
        _instructionsCanvasGroup.interactable = false;
        _instructionsCanvasGroup.blocksRaycasts = false;
    }

    private void Start(){
        _mainMenuCanvasGroup.DOFade(1, 1).OnComplete(() => {
            _mainMenuCanvasGroup.interactable = true;
            _mainMenuCanvasGroup.blocksRaycasts = true;
        });
    }

    public void ShowInstructions(){
        _mainMenuCanvasGroup.interactable = false;
        _mainMenuCanvasGroup.blocksRaycasts = false;
        _mainMenuCanvasGroup.DOFade(0, 1);
        _instructionsCanvasGroup.DOFade(1, 1).OnComplete(() => {
            _instructionsCanvasGroup.interactable = true;
            _instructionsCanvasGroup.blocksRaycasts = true;
        });
    }

    public void ShowMainMenu(){
        _instructionsCanvasGroup.interactable = false;
        _instructionsCanvasGroup.blocksRaycasts = false;
        _instructionsCanvasGroup.DOFade(0, 1);
        _mainMenuCanvasGroup.DOFade(1, 1).OnComplete(() => {
            _mainMenuCanvasGroup.interactable = true;
            _mainMenuCanvasGroup.blocksRaycasts = true;
        });
    }
}