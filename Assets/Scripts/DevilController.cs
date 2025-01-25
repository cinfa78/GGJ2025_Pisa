using System;
using System.Collections;
using UnityEngine;

public class DevilController : MonoBehaviour {
	[SerializeField] private float _movementSpeed;
	[SerializeField] private GameObject _spriteContainer;
	[SerializeField] private GameObject _spikeCollider;
	private SpriteRenderer _spriteRenderer;
	[SerializeField] private Sprite _deadDevilSprite;
	[Header("Shots")] public bool _canShoot;
	[SerializeField] private float _shotInterval;
	[SerializeField] private GameObject _bubbleShotPrefab;
	[SerializeField] private Sprite _shootingDevilSprite;
	private Rigidbody _rigidbody;
	private Transform _playerTransform;
	private bool _isAlive;
	[Header("Sfx")] [SerializeField] private AudioClip _deathSfx;
	[SerializeField] private AudioClip _shotSfx;
	private bool _hasTarget;
	private float _shotTimer;
	public static event Action<DevilController> DevilSpawned;
	public static event Action<DevilController> DevilKilled;

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
		_isAlive = true;
		_spriteRenderer = _spriteContainer.GetComponent<SpriteRenderer>();
		DevilSpawned?.Invoke(this);
	}

	private void OnDestroy() {
		DevilKilled?.Invoke(this);
	}

	private IEnumerator Start() {
		if (_canShoot) {
			_spriteRenderer.sprite = _shootingDevilSprite;
			_spikeCollider.SetActive(false);
		}
		yield return null;
		_playerTransform = GameController.Instance.GetPlayerTransform();
		_hasTarget = _playerTransform != null;
	}

	private void Update() {
		if (_isAlive && _hasTarget) {
			_rigidbody.velocity = (_playerTransform.position - transform.position).normalized * (_movementSpeed * Time.deltaTime);
			_spriteContainer.transform.localScale = !(_playerTransform.position.x > transform.position.x) ? Vector3.one : new Vector3(-1, 1, 1);
			if (_canShoot) {
				_shotTimer += Time.deltaTime;
				if (_shotTimer >= _shotInterval) {
					_shotTimer = 0;
					Shoot();
				}
			}
		}
	}

	private void Shoot() {
		AudioManager.Instance.PlaySfx(_shotSfx);
		var newShot = Instantiate(_bubbleShotPrefab, transform.position, Quaternion.identity);
		newShot.GetComponent<ShotController>().direction =
			(_playerTransform.position.x > transform.position.x) ? Vector3.right : Vector3.left;
	}

	private void OnCollisionEnter(Collision other) {
		if (_isAlive) {
			if (other.gameObject.layer == LayerMask.NameToLayer("Grenade")) {
				_isAlive = false;
				_spriteRenderer.sprite = _deadDevilSprite;
				_rigidbody.useGravity = true;
				AudioManager.Instance.PlaySfx(_deathSfx);
			}
		}
	}
}