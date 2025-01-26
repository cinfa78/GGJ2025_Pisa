using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable {
	public void Kill();
}

public class DevilBottomController : MonoBehaviour, IKillable {
	[SerializeField] private float _movementSpeed = 10;
	Rigidbody _rigidbody;
	[SerializeField] private GameObject _spikePrefab;
	[SerializeField] private float _shotFrequency;
	private float _shotTimer;
	private bool _isAlive;
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private Sprite _deadDevilSprite;
	[SerializeField] private AudioClip _deathSfx;
	private Transform _playerTransform;
	private bool _hasPlayerTransform;
	public static event Action<IKillable> DevilSpawned;
	public static event Action<IKillable> DevilKilled;

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
		_isAlive = true;
	}

	private void OnEnable() {
		DevilSpawned?.Invoke(this);
	}

	private void Update() {
		if (_isAlive) {
			if (!_hasPlayerTransform) {
				_playerTransform = GameController.Instance.GetPlayerTransform();
				_hasPlayerTransform = true;
			}
			_shotTimer += Time.deltaTime;
			if (_shotTimer > _shotFrequency) {
				_shotTimer = 0;
				Shoot();
			}
			if (transform.position.x < _playerTransform.position.x) {
				transform.position += Vector3.right * (_movementSpeed * Time.deltaTime);
			}
			else if (transform.position.x > _playerTransform.position.x) {
				transform.position += Vector3.left * (_movementSpeed * Time.deltaTime);
			}
		}
	}

	private void Shoot() {
		Instantiate(_spikePrefab, transform.position, Quaternion.identity);
	}

	private void OnCollisionEnter(Collision other) {
		if (_isAlive) {
			if (other.gameObject.layer == LayerMask.NameToLayer("Grenade")) {
				DoDeath();
			}
		}
	}

	private void DoDeath() {
		
		_isAlive = false;
		if (_spriteRenderer)
			_spriteRenderer.sprite = _deadDevilSprite;
		if (_rigidbody)
			_rigidbody.useGravity = true;
		GetComponent<Collider>().enabled = false;
		AudioManager.Instance.PlaySfx(_deathSfx);
		DevilKilled?.Invoke(this);
		Destroy(gameObject, 3f);
	}

	public void Kill() {
		DoDeath();
	}
}