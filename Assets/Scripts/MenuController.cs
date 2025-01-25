using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
	[SerializeField] private AudioClip _introMusic;

	private void Start() {
		AudioManager.Instance.PlayMusic(_introMusic);
	}

	public void StartGame() {
		SceneManager.LoadScene(1);
	}

	public void QuitGame() {
		Application.Quit();
	}
}