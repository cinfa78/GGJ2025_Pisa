using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour {
	[SerializeField] private float _movementSpeed;
	private float _targetHorizontal;
	private float _targetVertical;
	private bool _canMove;

	[SerializeField] private float _bubbleRadius;
	[SerializeField] private GameObject _holyHandGrenade;
	private Rigidbody _rigidbody;
	[SerializeField] private float _maxSize = 5;
	[SerializeField] private float _incrementPerGrenade = .1f;
	[SerializeField] private GameObject _bubble;
	private float _decrementPerSecond = .1f;
	[Header("Sfx")] [SerializeField] private AudioClip _bubbleSound;
	[SerializeField] private AudioClip _shootSound;

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void Update() {
		_targetHorizontal = Input.GetAxis("Horizontal");
		_targetVertical = Input.GetAxis("Vertical");
		if (Input.GetButtonDown("Fire1")) {
			AudioManager.Instance.PlaySfx(_shootSound);
			var newGrenade = Instantiate(_holyHandGrenade, transform.position, Quaternion.identity);
			newGrenade.name = "Grenade";
			if (_bubble.transform.localScale.x < _maxSize)
				_bubble.transform.localScale += Vector3.one * _incrementPerGrenade;
		}

		if (_bubble.transform.localScale.x > 1)
			_bubble.transform.localScale -= Vector3.one * (_decrementPerSecond * Time.deltaTime);
	}

	private void FixedUpdate() {
		_rigidbody.MovePosition(transform.position + Vector3.up * (_targetVertical * _movementSpeed * Time.fixedDeltaTime) + Vector3.right * (_targetHorizontal * Time.fixedDeltaTime * _movementSpeed));
	}
}