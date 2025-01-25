using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class DevilBoss : MonoBehaviour, IKillable {
	[SerializeField] private Collider[] _colliders;
	[SerializeField] private Transform[] _spikeNozzles;
	[SerializeField] private GameObject _spikeBulletPrefab;
	private int _spikesToShoot;
	[SerializeField] private float _shotInterval = 1;
	private float _shotTimer;

	[SerializeField] private Transform[] _bubbleNozzle;
	[SerializeField] private GameObject _bubbleBulletPrefab;
	private int _bubblesToShoot;
	[SerializeField] private int _health;
	private bool _isAlive;
	[SerializeField] private AudioClip _bossHurtSfx;
	[SerializeField] private AudioClip _bossdeathSfx;
	public static event Action OnBossDeath;
	public static event Action<IKillable> DevilSpawned;
	public static event Action<IKillable> DevilKilled;
	private Rigidbody _rigidbody;
	[SerializeField] private float _targetX;
	[SerializeField] private float _movementSpeed = 40;
	private Transform _targetTransform;
	private bool _hasPlayerTransform;

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void OnEnable() {
		DevilSpawned?.Invoke(this);
		_isAlive = true;
	}

	private void OnDisable() {
		DevilKilled?.Invoke(this);
	}

	private void Update() {
		if (_isAlive) {
			if (!_hasPlayerTransform) {
				_targetTransform = GameController.Instance.GetPlayerTransform();
				if (_targetTransform != null)
					_hasPlayerTransform = true;
			}
			_shotTimer += Time.deltaTime;
			if (_shotTimer >= _shotInterval) {
				Shoot();
				_shotTimer = 0;
			}
			if (transform.position.x > _targetX) transform.position += Vector3.left * (_movementSpeed * Time.deltaTime);
			if (transform.position.y < _targetTransform.position.y) transform.position += Vector3.up * (_movementSpeed * Time.deltaTime);
			if (transform.position.y > _targetTransform.position.y) transform.position += Vector3.down * (_movementSpeed * Time.deltaTime);
		}
	}

	private void Shoot() {
		var rnd = Random.Range(0, 1f);
		if (rnd < 0.5f) {
			for (int i = 0; i < _spikesToShoot; i++) {
				var newSpike = Instantiate(_spikeBulletPrefab, _spikeNozzles[i].position, _spikeNozzles[i].rotation);
			}
			_spikesToShoot++;
			if (_spikesToShoot > _spikeNozzles.Length)
				_spikesToShoot = _spikeNozzles.Length;
		}
		else {
			for (int i = 0; i < _bubblesToShoot; i++) {
				var newBubble = Instantiate(_bubbleBulletPrefab, _bubbleNozzle[i].position, _bubbleNozzle[i].rotation);
			}
			_bubblesToShoot++;
			if (_bubblesToShoot > _bubbleNozzle.Length)
				_bubblesToShoot = _bubbleNozzle.Length;
		}
	}

	private void OnCollisionEnter(Collision other) {
		if (_isAlive) {
			if (other.gameObject.layer == LayerMask.NameToLayer("Grenade")) {
				_health--;
				if (_health <= 0) {
					DoDeath();
				}
				else {
					AudioManager.Instance.PlaySfx(_bossHurtSfx);
				}
			}
		}
	}

	private void DoDeath() {
		_rigidbody.useGravity = true;
		_rigidbody.AddForce(Vector3.up * 3f, ForceMode.Impulse);
		AudioManager.Instance.PlaySfx(_bossdeathSfx);
		OnBossDeath?.Invoke();
		foreach (Collider c in _colliders) {
			c.enabled = false;
		}
		_isAlive = false;
	}

	[Button("Kill")]
	public void Kill() {
		DoDeath();
	}
}