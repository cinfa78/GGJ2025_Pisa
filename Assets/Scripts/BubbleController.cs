using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour {
	private float _targetHorizontal;
	private bool _canMove;
	[SerializeField] private float _bubbleRadius;
	[SerializeField] private GameObject _holyHandGrenade;

	private void Update() {
		_targetHorizontal = Input.GetAxis("Horizontal");
		if (Input.GetButtonDown("Fire1")) {
			var newGrenade = Instantiate(_holyHandGrenade, transform.position, Quaternion.identity);
			newGrenade.name = "Grenade";
		}
	}
}