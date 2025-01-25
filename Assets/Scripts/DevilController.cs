using System;
using UnityEngine;

public class DevilController : MonoBehaviour {
	[SerializeField] private float _movementSpeed;
	[SerializeField] private GameObject _spriteContainer;
	private Rigidbody _rigidbody;
	private Transform _bubbleTransform;

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void Start() {
		_bubbleTransform = GameController.Instance.GetPlayerTransform();
	}

	private void Update() {
		_rigidbody.velocity = (_bubbleTransform.position - transform.position).normalized * (_movementSpeed * Time.deltaTime);
	}
}