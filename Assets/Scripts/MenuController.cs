using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour{
    [SerializeField] private AudioClip _introMusic;
    [SerializeField] private TMP_Text _popeLabel;
    private bool _isLoading;

    private void Start(){
        AudioManager.Instance.PlayMusic(_introMusic);
        if(_popeLabel)
            _popeLabel.text = $"Pope #{SaveManager.PopeNumber:000}";
    }

    public void StartGame(){
        if (!_isLoading){
            _isLoading = true;
            SceneManager.LoadScene("PapalBull");
        }
    }

    public void LoadScene(string sceneName){
        if (!_isLoading){
            _isLoading = true;
            SceneManager.LoadScene(sceneName);
        }
    }

    public void QuitGame(){
        Application.Quit();
    }
}