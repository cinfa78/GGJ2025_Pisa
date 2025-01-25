using System;
using UnityEngine;

public class DevilController : MonoBehaviour {
	[SerializeField] private float _movementSpeed;
	[SerializeField] private GameObject _spriteContainer;
	private SpriteRenderer _spriteRenderer;
	[SerializeField] private Sprite _deadDevilSprite;
	private Rigidbody _rigidbody;
	private Transform _bubbleTransform;
	private bool _isAlive;

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
		_isAlive = true;
		_spriteRenderer = _spriteContainer.GetComponent<SpriteRenderer>();
	}

	private void Start() {
		_bubbleTransform = GameController.Instance.GetPlayerTransform();
	}

	private void Update() {
		if (_isAlive) {
			_rigidbody.velocity = (_bubbleTransform.position - transform.position).normalized * (_movementSpeed * Time.deltaTime);
			_spriteRenderer.flipX = _bubbleTransform.position.x > transform.position.x;
		}
	}

	private void OnCollisionEnter(Collision other) {
		if (_isAlive) {
			if (other.gameObject.layer == LayerMask.NameToLayer("Grenade")) {
				_isAlive = false;
				_spriteRenderer.sprite = _deadDevilSprite;
				_rigidbody.useGravity = true;
			}
		}
	}
}