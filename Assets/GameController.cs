using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public AudioClip music;

	private void Start() {
		AudioManager.Instance.PlayMusic(music);
	}
}