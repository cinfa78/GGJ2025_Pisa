using System;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class BubbleController : MonoBehaviour{
    [SerializeField] private PopeStatistics _popeStatistics;

    [Header("Graphics")] //[SerializeField] private SpriteRenderer _popeSpriterenderer;
    [SerializeField]
    private SpriteAnimator _popeAnimator;

    //[SerializeField] private Sprite _fallingPopeSprite;
    [SerializeField] private GameObject _popeHat;

    [Header("Bubble")] [SerializeField] private GameObject _bubbleContainer;
    [SerializeField] private GameObject[] _bubbles;
    [SerializeField] private Material[] _bubbleMaterials;
    [SerializeField] private float _graceTime;

    private int _health;
    [SerializeField] private float _bubbleRadius;

    [SerializeField] private float _maxBubbleSize = 5;

    [SerializeField] private float _incrementPerEnemyShot = .1f;
    [SerializeField] private ParticleSystem _popVfx;
    [Header("Shot")] [SerializeField] private Bullet _shot;
    [SerializeField] protected float _shotCoolDown = 0;
    [SerializeField] private Bullet _holyHandGrenade;
    [SerializeField] protected float _grenadeCoolDown = 0;
    [SerializeField] private GameObject _crossHairContainer;

    [Header("Sfx")] [SerializeField] private AudioClip _bubblePopSound;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _chargeShotSound;
    private float _shotCooldownTimer;
    private float _grenadeCooldownTimer;
    private int _spikeLayer;
    private Rigidbody _rigidbody;
    private float _bubbleDecrementPerSecond = .1f;
    private float _targetHorizontal;
    private float _targetVertical;
    private bool _canMove = true;
    private AudioSource _chargeAudioSource;
    private float _shootAngle = -45f;
    private int _uniqueCode;

    public bool IsAlive => _canMove;
    public bool _godMode;
    public bool _endingMode;
    private bool _chargingGrenade;
    public static event Action OnPlayerdeath;

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0, _popeStatistics.MinAngle) * Vector3.right);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0, _popeStatistics.MaxAngle) * Vector3.right);
    }

    private void Awake(){
        _rigidbody = GetComponent<Rigidbody>();
        _crossHairContainer.SetActive(false);
        _spikeLayer = LayerMask.NameToLayer("Spikes");
        _health = _bubbles.Length - 1;
        _shotCooldownTimer = _shotCoolDown;
    }

    private void Start(){
        _popeStatistics = SaveManager.Instance.GetSavedData.PopeStatistics;
        _uniqueCode = SaveManager.GetDeterministicHashCode(_popeStatistics.Name);
        _popeAnimator.SetLoop("idle");
    }

    private void Update(){
        _targetHorizontal = Input.GetAxis("Horizontal");
        _targetVertical = Input.GetAxis("Vertical");
        _shotCooldownTimer -= Time.deltaTime;
        _grenadeCooldownTimer -= Time.deltaTime;
        if (_canMove){
            if (_grenadeCooldownTimer <= 0){
                if (Input.GetButtonDown("Fire2")){
                    _popeAnimator.SetLoop("grenade");
                    _chargingGrenade = true;
                    _crossHairContainer.SetActive(true);
                    _chargeAudioSource = AudioManager.Instance.PlaySfx(_chargeShotSound);
                }

                if (Input.GetButton("Fire2")){
                    _shootAngle += Time.deltaTime * _popeStatistics.AngleIncrement;
                    if (_shootAngle > _popeStatistics.MaxAngle) _shootAngle = _popeStatistics.MaxAngle;
                    _crossHairContainer.transform.localRotation = Quaternion.Euler(0, 0, _shootAngle);
                    _crossHairContainer.transform.localRotation = Quaternion.Euler(0, 0, _shootAngle);
                }

                if (Input.GetButtonUp("Fire2") && _chargingGrenade){
                    _chargingGrenade = false;
                    if (_chargeAudioSource.clip == _chargeShotSound){
                        _chargeAudioSource.Stop();
                    }

                    _crossHairContainer.SetActive(false);
                    _grenadeCooldownTimer = _grenadeCoolDown;
                    FireGrenade();
                    _popeAnimator.SetLoop("idle");
                }
            }

            if (Input.GetButtonDown("Fire1") && !_chargingGrenade){
                if (_shotCooldownTimer <= 0){
                    _popeAnimator.SetLoop("shoot");
                    _popeAnimator.OnLoopOver += SetIdleAnimation;
                    _shotCooldownTimer = _shotCoolDown;
                    Shoot();
                }
            }
        }
        else{
            if (_endingMode){
                _popeAnimator.SetLoop("idle");
                if (_crossHairContainer.activeSelf) _crossHairContainer.SetActive(false);
                transform.position = Vector3.Lerp(transform.position, Vector3.zero, .5f * Time.deltaTime);
            }
        }

        if (_bubbleContainer.transform.localScale.x > 1)
            _bubbleContainer.transform.localScale -= Vector3.one * (_bubbleDecrementPerSecond * Time.deltaTime);
    }

    
    private void SetIdleAnimation(){
        _popeAnimator.OnLoopOver -= SetIdleAnimation;
        _popeAnimator.SetLoop("idle");
    }
    

    private void FireGrenade(){
        var newGrenade = Instantiate(_holyHandGrenade, transform.position, Quaternion.identity);
        newGrenade.name = "Grenade";
        var gc = newGrenade.GetComponent<GrenadeController>();
        gc.ApplyDirection(Quaternion.Euler(0, 0, z: _shootAngle) * Vector3.right);
        gc.SetupSprite(_uniqueCode);
        if (_bubbleContainer.transform.localScale.x < _maxBubbleSize)
            _bubbleContainer.transform.localScale += Vector3.one * _popeStatistics.IncrementPerShot;
        _shootAngle = _popeStatistics.MinAngle;
    }

    private void Shoot(){
        var newShot = Instantiate(_shot, transform.position + Vector3.right * _bubbleRadius / 2f, Quaternion.identity);
        newShot.name = "Shot";
        newShot.ApplyDirection(Vector3.right);
    }

    private void FixedUpdate(){
        if (_canMove)
            _rigidbody.MovePosition(transform.position + Vector3.up * (_targetVertical * _popeStatistics.MovementSpeed * Time.fixedDeltaTime) + Vector3.right * (_targetHorizontal * Time.fixedDeltaTime * _popeStatistics.MovementSpeed));
    }

    [Button("Test Pop")]
    private void TestPop(){
        PopBubble(_health);
        _health--;
    }

    private void OnCollisionEnter(Collision other){
        if (!_godMode){
            if (other.gameObject.layer == _spikeLayer){
                PopBubble(_health);
                _health--;
                if (_health < 0){
                    PopLastBubble();
                }

                var sc = other.gameObject.GetComponent<ShotController>();
                if (sc)
                    sc.DeactivateShot();
            }
        }

        if (_canMove){
            if (other.gameObject.layer == LayerMask.NameToLayer("Pope")){
                //gonfio la bolla
                _bubbleContainer.transform.localScale += Vector3.one * (_incrementPerEnemyShot);
            }
        }
    }

    public void SetMovement(bool canMove){
        _canMove = canMove;
    }

    private void PopBubble(int index){
        SquashAndStretch();
        AudioManager.Instance.PlaySfx(_bubblePopSound);
        _bubbles[index].SetActive(false);
        _popVfx.transform.localScale = _bubbleContainer.transform.localScale;
        _popVfx.Play();
        _godMode = true;
        if (_health > 0)
            StartCoroutine(GraceTimeCountdown());
    }

    private IEnumerator GraceTimeCountdown(){
        float timer = 0;
        MeshRenderer bubbleMesh = _bubbles[_health - 1].GetComponent<MeshRenderer>();
        bubbleMesh.material = _bubbleMaterials[1];
        while (timer < _graceTime){
            timer += Time.deltaTime;
            yield return null;
        }

        bubbleMesh.material = _bubbleMaterials[0];
        _godMode = false;
    }

    private void SquashAndStretch(){
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_bubbleContainer.transform.DOPunchScale(Vector3.right * .1f, .1f));
        sequence.Append(_bubbleContainer.transform.DOPunchScale(Vector3.up * .1f, .1f));
        sequence.Append(_bubbleContainer.transform.DOPunchScale(Vector3.right * .1f, .2f));
        sequence.Append(_bubbleContainer.transform.DOPunchScale(Vector3.up * .1f, .2f));
        sequence.Append(_bubbleContainer.transform.DOPunchScale(Vector3.right * .1f, .3f));
        sequence.Append(_bubbleContainer.transform.DOPunchScale(Vector3.up * .1f, .3f));
        //sequence.Append(_bubbleContainer.transform.DOScale(Vector3.one, .1f));
        sequence.Play();
    }

    private void PopLastBubble(){
        _canMove = false;
        _rigidbody.useGravity = true;

        _popeAnimator.SetLoop("dead");
        ;
        //_popeSpriterenderer.sprite = _fallingPopeSprite;

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