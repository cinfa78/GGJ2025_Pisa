using UnityEngine;
using Random = UnityEngine.Random;

public class BishopGenerator : MonoBehaviour{
    [SerializeField] private GameObject _bishopPrefab;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Vector4 _generationArea;
    [SerializeField] private int _amount;
    [SerializeField] private int _bishopsPerRow = 10;
    [SerializeField] private bool _flipX;

    private void OnDrawGizmos(){
        Gizmos.DrawWireSphere(transform.position + new Vector3(_generationArea.x, _generationArea.y, 0), .1f);
        Gizmos.DrawWireSphere(transform.position + new Vector3(_generationArea.z, _generationArea.w, 0), .1f);
        var startPosition = transform.position + new Vector3(_generationArea.x, _generationArea.y, 0);
        var xStep = (_generationArea.z - _generationArea.x) / _bishopsPerRow;
        var yStep = (_generationArea.w - _generationArea.y) / _bishopsPerRow;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < _amount; i++){
            Gizmos.DrawWireSphere(startPosition + new Vector3((i % _bishopsPerRow) * xStep + (_flipX ? ((i / 100f)) : (-(i / 100f))) + Random.Range(-.1f, 0.1f), (i / _bishopsPerRow) * yStep, 0), .1f);
        }
    }

    private void Awake(){
        var startPosition = transform.position + new Vector3(_generationArea.x, _generationArea.y, 0);
        var xStep = (_generationArea.z - _generationArea.x) / _bishopsPerRow;
        var yStep = (_generationArea.w - _generationArea.y) / _bishopsPerRow;
        for (int i = 0; i < _amount; i++){
            
            var newBishop = Instantiate(_bishopPrefab,transform);
            newBishop.transform.position = startPosition + new Vector3((i % _bishopsPerRow) * xStep + (_flipX ? ((i / 100f)) : (-(i / 100f))) + Random.Range(-.1f, 0.1f), (i / _bishopsPerRow) * yStep, 0);
            var newSpriteRender = newBishop.GetComponent<SpriteRenderer>();
            newSpriteRender.sprite = sprites[Random.Range(0, sprites.Length)];
            newSpriteRender.sortingOrder = _amount - i;
            newSpriteRender.flipX = _flipX;
        }
    }
}