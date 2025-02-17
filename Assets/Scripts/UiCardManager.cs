using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UiCardManager : MonoBehaviour{
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private DemonListSo.DemonData demonData;
    private bool _isShowing;
    private bool _locked;
    private float _defaultZRotation;

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
    }

    private void Start(){
        //Flip();
    }

    private void UpdateCard(){
        _nameLabel.text = demonData.name;
        _spriteRenderer.sprite = demonData.sprite;
    }

    private void OnMouseDown(){
        Flip();
    }

    [Button("Flip")]
    private void Flip(){
        _isShowing = !_isShowing;
        transform.DOKill();
        if (_isShowing){
            transform.DORotate(Vector3.up * 0, Random.Range(.5f, 1f));
        }
        else{
            transform.DORotate(Vector3.up * 180, Random.Range(.5f, 1f));
        }
    }
}