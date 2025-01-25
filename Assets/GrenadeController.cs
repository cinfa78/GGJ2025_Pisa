using System;
using UnityEngine;

public class GrenadeController : MonoBehaviour {
	[SerializeField] private Vector3 _intitialDirection = Vector3.right;
	[SerializeField] private float _initialVelocity;
	[SerializeField] private GameObject _splashVfx;
	[SerializeField] private AudioClip _splashSfx;
	Rigidbody _rigidbody;

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
		_intitialDirection.Normalize();
		Destroy(gameObject, 3);
	}

	private void Start() {
		_rigidbody.useGravity = false;
	}

	public void ApplyDirection(Vector3 startDirection) {
		_intitialDirection = startDirection.normalized;
		_rigidbody.useGravity = true;
		_rigidbody.velocity = _intitialDirection * _initialVelocity;
	}

	private void OnCollisionEnter(Collision other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Devil") || other.gameObject.layer == LayerMask.NameToLayer("Spikes")) {
			_splashVfx.transform.SetParent(null);
			_splashVfx.SetActive(true);
			AudioManager.Instance.PlaySfx(_splashSfx);
			Destroy(gameObject);
		}
	}
}