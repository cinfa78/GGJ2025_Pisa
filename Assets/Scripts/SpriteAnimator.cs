using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpriteAnimator : MonoBehaviour {
	public SpriteRenderer spriteRenderer;
	[FormerlySerializedAs("sprites")] public Sprite[] frames;
	public float fps;
	private float timer;
	private int frame;

	private void Start() {
		spriteRenderer.sprite = frames[frame];
	}

	private void Update() {
		timer += Time.deltaTime;
		if (timer >= 1f / fps) {
			timer = 0;
			frame = (frame + 1) % frames.Length;
			spriteRenderer.sprite = frames[frame];
		}
	}
}