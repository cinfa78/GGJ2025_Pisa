using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleSpriteAnimator : MonoBehaviour{
    public SpriteRenderer spriteRenderer;
    [SerializeField] private int _direction = 1;
    public Sprite[] frames;
    public float fps;
    private float _timer;
    private int _frame = 0;
    [SerializeField] private bool _playing = true;
    [SerializeField] private bool _loop = true;
    public event Action OnLoopOver;

    private void OnEnable(){
        if (frames == null){
            Debug.LogWarning($"{name} SpriteAnimator missing sprites");
            frames = new Sprite[0];
            frames[0] = spriteRenderer.sprite;
        }
    }

    private void Start(){
        if (_direction == 1)
            spriteRenderer.sprite = frames[_frame];
        else
            spriteRenderer.sprite = frames[frames.Length - 1];
    }

    [Button("Toggle")]
    public void Toggle(){
        _playing = !_playing;
    }

    private void Update(){
        _timer += Time.deltaTime;
        if (_playing && _timer >= 1f / fps){
            _timer = 0;
            _frame = (frames.Length + (_frame + _direction)) % frames.Length;
            if (_loop || _frame != 0){
                spriteRenderer.sprite = frames[_frame];
            }
            else{
                OnLoopOver?.Invoke();
                _playing = false;
            }
        }
    }
}