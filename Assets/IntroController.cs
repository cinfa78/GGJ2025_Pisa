using System;
using DG.Tweening;
using UnityEngine;

public class IntroController : MonoBehaviour {
	public CanvasGroup[] slides;
	public static event Action OnIntroOver;
	[SerializeField] private AudioClip _slideOverSfx;

	private void Awake() {
		for (int i = 0; i < slides.Length; i++) {
			slides[i].alpha = 0;
		}
	}

	private void Start() {
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < slides.Length; i++) {
			sequence.Append(slides[i].DOFade(1, .2f));
			sequence.AppendInterval(2);
			sequence.AppendCallback(() => { AudioManager.Instance.PlaySfx(_slideOverSfx); });
			sequence.Append(slides[i].DOFade(0, .2f));
		}
		sequence.OnComplete(() => {
			OnIntroOver?.Invoke();
			Destroy(gameObject);
		});
	}
}