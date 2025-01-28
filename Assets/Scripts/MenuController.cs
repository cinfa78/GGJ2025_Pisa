using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour{
    [SerializeField] private AudioClip _introMusic;
    private bool _isLoading;

    private void Start(){
        AudioManager.Instance.PlayMusic(_introMusic);
    }

    public void StartGame(){
        if (!_isLoading){
            _isLoading = true;
            SceneManager.LoadScene(1);
        }
    }

    public void QuitGame(){
        Application.Quit();
    }
}