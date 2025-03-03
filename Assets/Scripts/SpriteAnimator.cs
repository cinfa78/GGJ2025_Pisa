using System;
using NaughtyAttributes;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteAnimation _spriteAnimation;
    private float _timer;
    private int _frame = 0;
    [SerializeField] private bool _playing = true;
    public event Action OnLoopOver;
    private SpriteAnimation.Loop _currentLoop;

    private void OnEnable(){
        if (_spriteAnimation == null){
            Debug.LogError($"{name} SpriteAnimator missing sprites");
        }
        else{
            _currentLoop = _spriteAnimation.GetFirstLoop();
            _spriteRenderer.sprite = _currentLoop.frames[0];
            _timer = 0;
        }
    }

    public void SetLoop(string loopName){
        _playing = true;
        _currentLoop = _spriteAnimation.GetLoop(loopName);
        _frame = 0;
        _spriteRenderer.sprite = _currentLoop.frames[_frame];
    }

    private void Update(){
        _timer += Time.deltaTime;
        if (_playing && _timer >= 1f / _currentLoop.fps){
            _timer = 0;
            _frame = (_currentLoop.frames.Length + (_frame + 1)) % _currentLoop.frames.Length;
            if (_currentLoop.loop || _frame != 0){
                _spriteRenderer.sprite = _currentLoop.frames[_frame];
            }
            else{
                OnLoopOver?.Invoke();
                _playing = false;
            }
        }
    }

    [Button("Toggle")]
    public void Toggle(){
        _timer = 0;
        _playing = !_playing;
    }
}