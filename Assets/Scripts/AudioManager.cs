using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour {
	public static AudioManager Instance;

	[Header("OST")] [FormerlySerializedAs("_audioSources")] [SerializeField]
	private AudioSource[] _ostAudioSources;
	[SerializeField] private AudioMixerGroup _ostMixerGroup;
	[SerializeField] private float _fadeTime = 1;
	[SerializeField] private AudioClip[] _canzoni;
	private int _currentSongIndex;

	[Header("SFX")] [SerializeField] private AudioSource[] _sfxAudioSources;
	[SerializeField] private AudioMixerGroup _sfxMixerGroup;
	[SerializeField] private int _sfxChannels;

	private static int _currentOstChannel = 0;
	private static int _currentSfxChannel = 0;
	private static int channels;
	private bool _isFading;

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
		}
		else Instance = this;

		DontDestroyOnLoad(gameObject);
		channels = _ostAudioSources.Length;
		_sfxAudioSources = new AudioSource[_sfxChannels];
		for (int i = 0; i < _sfxChannels; i++) {
			var newAudioSource = gameObject.AddComponent<AudioSource>();
			newAudioSource.outputAudioMixerGroup = _sfxMixerGroup;
			newAudioSource.loop = false;
			newAudioSource.playOnAwake = false;
			_sfxAudioSources[i] = newAudioSource;
		}
		ShuffleArray(_canzoni);
	}

	private void ShuffleArray(AudioClip[] array) {
		for (int i = array.Length - 1; i > 0; i--) {
			int randomIndex = Random.Range(0, i + 1);
			(array[i], array[randomIndex]) = (array[randomIndex], array[i]);
		}
	}

	public void PlayNextSong() {
		_currentSongIndex = (_currentSongIndex + 1) % _canzoni.Length;
		PlayMusic(_canzoni[_currentSongIndex]);
	}

	public void PlayMusic(AudioClip clip) {
		if (_ostAudioSources[_currentOstChannel].clip == clip && _ostAudioSources[_currentOstChannel].isPlaying) return;
		int nextChannel = (_currentOstChannel + 1) % channels;
		_ostAudioSources[nextChannel].clip = clip;
		_ostAudioSources[nextChannel].volume = 0;
		_ostAudioSources[nextChannel].Play();
		if (_isFading)
			StopAllCoroutines();
		StartCoroutine(CrossFading(nextChannel));
	}

	public void PlaySfx(AudioClip clip) {
		_currentSfxChannel = (_currentSfxChannel + 1) % _sfxChannels;
		_sfxAudioSources[_currentSfxChannel].clip = clip;
		_sfxAudioSources[_currentSfxChannel].Play();
	}

	private IEnumerator CrossFading(int nextChannel) {
		_isFading = true;
		float timer = 0;
		while (timer < _fadeTime) {
			_ostAudioSources[_currentOstChannel].volume = Mathf.Lerp(1, 0, timer / _fadeTime);
			_ostAudioSources[nextChannel].volume = Mathf.Lerp(0, 1, timer / _fadeTime);
			timer += Time.deltaTime;
			yield return null;
		}

		_ostAudioSources[_currentOstChannel].volume = 0;
		_ostAudioSources[nextChannel].volume = 1;
		_currentOstChannel = nextChannel;
		_isFading = false;
	}
}