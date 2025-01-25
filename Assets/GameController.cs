using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public static GameController Instance;
	public AudioClip music;
	private BubbleController _bubbleController;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	private void Start() {
		_bubbleController = FindObjectOfType<BubbleController>();
		AudioManager.Instance.PlayMusic(music);
	}

	public Transform GetPlayerTransform() {
		return _bubbleController.transform;
	}
}