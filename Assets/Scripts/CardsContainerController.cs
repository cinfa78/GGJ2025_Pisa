using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CardsContainerController : MonoBehaviour{
    [SerializeField] private Vector2 _size;
    [SerializeField] private DemonListSo _demonList;
    [SerializeField] private GameObject _cardPrefab;
    private int _cardsPerColumn;
    private int _cardsPerRow;
    private float xStep;
    private float yStep;
    private List<UiCardManager> _cards = new();

    private void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _size);
        _cardsPerColumn = (int)(_demonList.data.Count / 4f) - 3;
        _cardsPerRow = _demonList.data.Count / _cardsPerColumn;
        xStep = _size.x / _cardsPerColumn;
        yStep = _size.y / _cardsPerRow;
        Gizmos.color = Color.red;
        for (int i = 0; i < _cardsPerRow; i++){
            for (int j = 0; j < _cardsPerColumn; j++){
                Gizmos.DrawWireSphere(transform.position + new Vector3(-_size.x / 2, +_size.y / 2, 0) + Vector3.right * (xStep * j + xStep / 2) + Vector3.down * (yStep * i + yStep / 2), .05f);
            }
        }
    }

    private void Awake(){
        _cardsPerColumn = (int)(_demonList.data.Count / 4f) - 3;
        _cardsPerRow = _demonList.data.Count / _cardsPerColumn;
        xStep = _size.x / _cardsPerColumn;
        yStep = _size.y / _cardsPerRow;
    }

    private void Start(){
        
        GenerateCards();
    }

    [Button("Generate Cards")]
    private void GenerateCards(){
        Debug.Log($"Generating cards: {_demonList.data.Count}");
        StartCoroutine(GeneratingCards());
    }

    private IEnumerator GeneratingCards(){
        int card = 0;
        for (int i = 0; i < _cardsPerRow; i++){
            for (int j = 0; j < _cardsPerColumn; j++){
                var newCard = Instantiate(_cardPrefab, transform.position + new Vector3(-_size.x / 2, +_size.y / 2, 0) + Vector3.right * (xStep * j + xStep / 2) + Vector3.down * (yStep * i + yStep / 2), Quaternion.identity, transform);
                newCard.name = "Card_" + _demonList.data[card].name;
                yield return null;
                newCard.GetComponent<UiCardManager>().DemonData = _demonList.data[card];
                _cards.Add(newCard.GetComponent<UiCardManager>());
                yield return null;
                card++;
                if (card == _demonList.data.Count){
                    break;
                }
            }

            if (card == _demonList.data.Count){
                break;
            }
        }
    }
}