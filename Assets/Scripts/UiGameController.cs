using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UiGameController : MonoBehaviour{
    public DemonListSo demonListSo;
    public float victoryAnimationTime = 5f;
    [SerializeField] private CanvasGroup victoryCanvasGroup;
    [SerializeField] private CanvasGroup defeatCanvasGroup;
    [SerializeField] private TMP_Text _leftList;
    [SerializeField] private TMP_Text _rightList;

    private void Awake(){
        victoryCanvasGroup.alpha = 0;
        victoryCanvasGroup.blocksRaycasts = false;
        victoryCanvasGroup.interactable = false;
        defeatCanvasGroup.alpha = 0;
        defeatCanvasGroup.blocksRaycasts = false;
        defeatCanvasGroup.interactable = false;

        _leftList.text = "";
        _rightList.text = "";

        GameController.OnGameOver += OnGameOver;
        GameController.OnVictory += OnVictory;
    }

    private void OnDestroy(){
        GameController.OnGameOver -= OnGameOver;
        GameController.OnVictory -= OnVictory;
    }

    private void OnVictory(){
        victoryCanvasGroup.DOFade(1, victoryAnimationTime);
        victoryCanvasGroup.interactable = true;
        victoryCanvasGroup.blocksRaycasts = true;
        var killedDemonsNames = demonListSo.GetNames(GameController.devilsKilled);
        for (var i = 0; i < killedDemonsNames.Count; i++){
            if (i % 2 == 0){
                _leftList.text += killedDemonsNames[i] + "\n";
            }
            else{
                _rightList.text += killedDemonsNames[i] + "\n";
            }
        }
    }

    private void OnGameOver(){
        defeatCanvasGroup.DOFade(1, victoryAnimationTime);
        defeatCanvasGroup.interactable = true;
        defeatCanvasGroup.blocksRaycasts = true;
    }
}