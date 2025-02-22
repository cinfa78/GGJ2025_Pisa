using System;
using System.Collections;
using UnityEngine;

public class BubbleController : MonoBehaviour{
    [SerializeField] private PopeStatistics _popeStatistics;
    //[SerializeField] private float _movementSpeed;

    [Header("Graphics")] [SerializeField] private SpriteRenderer _popeSpriterenderer;
    [SerializeField] private Sprite _fallingPopeSprite;
    [SerializeField] private GameObject _popeHat;

    [Header("Bubble")] [SerializeField] private GameObject _bubble;
    [SerializeField] private float _bubbleRadius;

    [SerializeField] private float _maxBubbleSize = 5;

    //[SerializeField] private float _incrementPerGrenade = .1f;
    [SerializeField] private float _incrementPerEnemyShot = .1f;

    [Header("Shot")] [SerializeField] private GameObject _holyHandGrenade;

    [SerializeField] private GameObject _crossHairContainer;
    //[SerializeField] private Vector2 _minMaxAngle;
    //[SerializeField] private float _angleIncrement = 60;

    [Header("Sfx")] [SerializeField] private AudioClip _bubblePopSound;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _chargeShotSound;

    private Rigidbody _rigidbody;
    private float _bubbleDecrementPerSecond = .1f;
    private float _targetHorizontal;
    private float _targetVertical;
    private bool _canMove = true;
    private AudioSource _chargeAudioSource;
    private float _shootAngle = -45f;

    public bool IsAlive => _canMove;
    public bool _godMode;

    public static event Action OnPlayerdeath;

    private void Awake(){
        _rigidbody = GetComponent<Rigidbody>();
        _crossHairContainer.SetActive(false);
        _popeStatistics = new PopeStatistics();
    }

    private void Update(){
        _targetHorizontal = Input.GetAxis("Horizontal");
        _targetVertical = Input.GetAxis("Vertical");
        if (_canMove){
            if (Input.GetButtonDown("Fire1")){
                _crossHairContainer.SetActive(true);
                _chargeAudioSource = AudioManager.Instance.PlaySfx(_chargeShotSound);
            }

            if (Input.GetButton("Fire1")){
                _shootAngle += Time.deltaTime * _popeStatistics.AngleIncrement;
                if (_shootAngle > _popeStatistics.MaxAngle) _shootAngle = _popeStatistics.MaxAngle;
                _crossHairContainer.transform.localRotation = Quaternion.Euler(0, 0, _shootAngle);
                _crossHairContainer.transform.localRotation = Quaternion.Euler(0, 0, _shootAngle);
            }

            if (Input.GetButtonUp("Fire1")){
                if (_chargeAudioSource.clip == _chargeShotSound){
                    _chargeAudioSource.Stop();
                }

                _crossHairContainer.SetActive(false);
                FireGrenade();
            }
        }
        else{
            if (_godMode){
                if (_crossHairContainer.activeSelf) _crossHairContainer.SetActive(false);
                transform.position = Vector3.Lerp(transform.position, Vector3.zero, .5f * Time.deltaTime);
            }
        }

        if (_bubble.transform.localScale.x > 1)
            _bubble.transform.localScale -= Vector3.one * (_bubbleDecrementPerSecond * Time.deltaTime);
    }

    private void FireGrenade(){
        var newGrenade = Instantiate(_holyHandGrenade, transform.position, Quaternion.identity);
        newGrenade.name = "Grenade";
        newGrenade.GetComponent<GrenadeController>().ApplyDirection(Quaternion.Euler(0, 0, z: _shootAngle) * Vector3.right);
        if (_bubble.transform.localScale.x < _maxBubbleSize)
            _bubble.transform.localScale += Vector3.one * _popeStatistics.IncrementPerShot;
        _shootAngle = _popeStatistics.MinAngle;
    }

    private void FixedUpdate(){
        if (_canMove)
            _rigidbody.MovePosition(transform.position + Vector3.up * (_targetVertical * _popeStatistics.MovementSpeed * Time.fixedDeltaTime) + Vector3.right * (_targetHorizontal * Time.fixedDeltaTime * _popeStatistics.MovementSpeed));
    }

    private void OnCollisionEnter(Collision other){
        if (_canMove){
            if (!_godMode){
                if (other.gameObject.layer == LayerMask.NameToLayer("Spikes")){
                    PopBubble();
                }
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Pope")){
                //gonfio la bolla
                _bubble.transform.localScale += Vector3.one * (_incrementPerEnemyShot);
                Destroy(other.gameObject);
            }
        }
    }

    public void SetMovement(bool canMove){
        _canMove = canMove;
    }

    private void PopBubble(){
        SaveManager.Instance.AddDeadPope();
        AudioManager.Instance.PlaySfx(_bubblePopSound);
        _bubble.SetActive(false);
        _canMove = false;
        _rigidbody.useGravity = true;
        _popeSpriterenderer.sprite = _fallingPopeSprite;
        _popeHat.transform.SetParent(null);
        _popeHat.SetActive(true);
        _popeHat.GetComponent<Rigidbody>().AddForce(Vector3.up * 5, ForceMode.Impulse);
        StartCoroutine(Dying());
    }

    private IEnumerator Dying(){
        yield return new WaitForSeconds(.5f);
        AudioManager.Instance.PlaySfx(_deathSound);
        OnPlayerdeath?.Invoke();
    }
}