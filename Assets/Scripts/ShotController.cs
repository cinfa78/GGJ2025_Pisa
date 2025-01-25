using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ShotController : MonoBehaviour {
	[FormerlySerializedAs("_direction")] public Vector3 direction = Vector3.zero;
	[SerializeField] private float _shotSpeed;

	private void Awake() {
		Destroy(gameObject, 5);
	}

	private void Update() {
		transform.position += direction * (_shotSpeed * Time.deltaTime);
	}
}