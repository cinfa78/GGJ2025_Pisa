using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BubbleController : MonoBehaviour {
	[SerializeField] private float _movementSpeed;
	private float _targetHorizontal;
	private float _targetVertical;
	private bool _canMove = true;

	[SerializeField] private float _bubbleRadius;
	[SerializeField] private GameObject _holyHandGrenade;
	[SerializeField] private Vector2 _minMaxAngle;
	private Rigidbody _rigidbody;
	[SerializeField] private float _maxSize = 5;
	[SerializeField] private float _incrementPerGrenade = .1f;
	[SerializeField] private GameObject _bubble;
	private float _decrementPerSecond = .1f;
	[SerializeField] private GameObject _crossHairContainer;
	[SerializeField] private float _angleIncrement = 2;
	[Header("Sfx")] [SerializeField] private AudioClip _bubbleSound;
	[SerializeField] private AudioClip _shootSound;
	[SerializeField] private AudioClip _deathSound;

	private float _shootAngle = -45f;

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
		_crossHairContainer.SetActive(false);
	}

	private void Update() {
		_targetHorizontal = Input.GetAxis("Horizontal");
		_targetVertical = Input.GetAxis("Vertical");
		if (Input.GetButtonDown("Fire1")) {
			_crossHairContainer.SetActive(true);
		}
		if (Input.GetButton("Fire1")) {
			_shootAngle += Time.deltaTime * _angleIncrement;
			if (_shootAngle > _minMaxAngle.y) _shootAngle = _minMaxAngle.y;
			_crossHairContainer.transform.localRotation = Quaternion.Euler(0, 0, _shootAngle);
			_crossHairContainer.transform.localRotation = Quaternion.Euler(0, 0, _shootAngle);
		}
		if (Input.GetButtonUp("Fire1")) {
			_crossHairContainer.SetActive(false);
			FireGrenade();
		}
		if (_bubble.transform.localScale.x > 1)
			_bubble.transform.localScale -= Vector3.one * (_decrementPerSecond * Time.deltaTime);
	}

	private void FireGrenade() {
		Debug.Log(_shootAngle);
		AudioManager.Instance.PlaySfx(_shootSound);
		var newGrenade = Instantiate(_holyHandGrenade, transform.position, Quaternion.identity);
		newGrenade.name = "Grenade";
		newGrenade.GetComponent<GrenadeController>().ApplyDirection(Quaternion.Euler(0, 0, z: _shootAngle) * Vector3.right);
		if (_bubble.transform.localScale.x < _maxSize)
			_bubble.transform.localScale += Vector3.one * _incrementPerGrenade;
		_shootAngle = _minMaxAngle.x;
	}

	private void FixedUpdate() {
		if (_canMove)
			_rigidbody.MovePosition(transform.position + Vector3.up * (_targetVertical * _movementSpeed * Time.fixedDeltaTime) + Vector3.right * (_targetHorizontal * Time.fixedDeltaTime * _movementSpeed));
	}

	private void OnCollisionEnter(Collision other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Spikes")) {
			PopBubble();
		}
	}

	private void PopBubble() {
		AudioManager.Instance.PlaySfx(_bubbleSound);
		_bubble.SetActive(false);
		_canMove = false;
		_rigidbody.useGravity = true;
		StartCoroutine(Dying());
	}

	private IEnumerator Dying() {
		yield return new WaitForSeconds(.5f);
		AudioManager.Instance.PlaySfx(_deathSound);
	}
}