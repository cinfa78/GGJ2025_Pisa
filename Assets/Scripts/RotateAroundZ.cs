using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundZ : MonoBehaviour {
	public float speed = 50;

	private void Update() {
		transform.Rotate(Vector3.forward, speed * Time.deltaTime);
	}
}