using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour{
    [SerializeField] private AudioClip _introMusic;
    [SerializeField] private TMP_Text _popeLabel;
    private bool _isLoading;

    private void Start(){
        AudioManager.Instance.PlayMusic(_introMusic);
        if (_popeLabel)
            _popeLabel.text = $"{SaveManager.Instance.GetLastPopeName()}";
    }
}