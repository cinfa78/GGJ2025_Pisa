using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiCardManager : MonoBehaviour{
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public DemonListSo.DemonData demonData;

    private DemonListSo.DemonData DemonData{
        get => demonData;
        set{
            demonData = value;
            UpdateCard();
        }
    }

    private void OnValidate(){
        UpdateCard();
    }

    private void UpdateCard(){
        _nameLabel.text = demonData.name;
        _spriteRenderer.sprite = demonData.sprite;
    }
}