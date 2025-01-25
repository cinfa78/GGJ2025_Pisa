using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour {
	[SerializeField] private float _movementSpeed;
	private float _targetHorizontal;
	private bool _canMove;
	public Vector2 horizontalLimit;
	[SerializeField] private float _bubbleRadius;
	[SerializeField] private GameObject _holyHandGrenade;
	private Rigidbody _rigidbody;
	[SerializeField] private float _maxSize = 5;
	[SerializeField] private float _incrementPerGrenade = .1f;
	[SerializeField] private GameObject _bubble;

	private void OnDrawGizmos() {
		Gizmos.DrawLine(transform.position, transform.position + Vector3.right * horizontalLimit.x);
		Gizmos.DrawLine(transform.position, transform.position + Vector3.right * horizontalLimit.y);
	}

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void Update() {
		_targetHorizontal = Input.GetAxis("Horizontal");
		if (Input.GetButtonDown("Fire1")) {
			var newGrenade = Instantiate(_holyHandGrenade, transform.position, Quaternion.identity);
			newGrenade.name = "Grenade";
			if (_bubble.transform.localScale.x < _maxSize)
				_bubble.transform.localScale += Vector3.one * _incrementPerGrenade;
		}
		if (transform.position.x < horizontalLimit.x) {
			transform.position = new Vector3(horizontalLimit.x, transform.position.y, transform.position.z);
		}
		else if (transform.position.x > horizontalLimit.y) {
			transform.position = new Vector3(horizontalLimit.y, transform.position.y, transform.position.z);
		}
	}

	private void FixedUpdate() {
		_rigidbody.MovePosition(transform.position + Vector3.right * (_targetHorizontal * Time.fixedDeltaTime * _movementSpeed));
	}
}