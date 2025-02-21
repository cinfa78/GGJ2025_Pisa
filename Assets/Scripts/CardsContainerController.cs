using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CardsContainerController : MonoBehaviour{
    [SerializeField] private Vector2 _size;
    [SerializeField] private DemonListSo _demonList;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField, ReadOnly] private int _cardsPerColumn;
    [SerializeField, ReadOnly] private int _cardsPerRow;
    private float _xStep;
    private float _yStep;
    private List<UiCardManager> _cards = new();

    private void OnDrawGizmos(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _size);
        SetupGrid();
        Gizmos.color = Color.red;
        for (int i = 0; i < _cardsPerRow; i++){
            for (int j = 0; j < _cardsPerColumn; j++){
                Gizmos.DrawWireSphere(transform.position + new Vector3(-_size.x / 2, +_size.y / 2, 0) + Vector3.right * (_xStep * j + _xStep / 2) + Vector3.down * (_yStep * i + _yStep / 2f), .05f);
            }
        }
    }

    private void Awake(){
        SetupGrid();
    }

    private void Start(){
        GenerateCards();
    }

    [Button("Generate Cards")]
    private void GenerateCards(){
        Debug.Log($"Generating cards: {_demonList.data.Count}");
        StartCoroutine(GeneratingCards());
    }

    private void SetupGrid(){
        _cardsPerColumn = (int)15;
        _cardsPerRow = _demonList.data.Count / _cardsPerColumn + 1;
        _xStep = _size.x / _cardsPerColumn;
        _yStep = _size.y / _cardsPerRow;
    }

    private IEnumerator GeneratingCards(){
        int card = 0;
        for (int i = 0; i < _cardsPerRow; i++){
            for (int j = 0; j < _cardsPerColumn; j++){
                var newCard = Instantiate(_cardPrefab, transform.position + new Vector3(-_size.x / 2, +_size.y / 2, 0) + Vector3.right * (_xStep * j + _xStep / 2) + Vector3.down * (_yStep * i + _yStep / 2f), Quaternion.identity, transform);
                newCard.name = "Card_" + _demonList.data[card].name;
                yield return null;
                var newCardManager = newCard.GetComponent<UiCardManager>();
                newCardManager.DemonData = _demonList.data[card];
                if (SaveManager.freshlyUnlockedDemons.Contains(_demonList.data[card].name)){
                    newCardManager.UnlockCard();
                }
                else if (SaveManager.Instance.GetSavedData.Contains(_demonList.data[card].name)){
                    newCardManager.ShowCard();
                }

                _cards.Add(newCardManager);
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