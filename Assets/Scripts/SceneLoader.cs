using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour{
    private static bool _isLoading = false;

    public static void MainMenu(){
        LoadScene("MainMenu");
    }

    public static void Game(){
        LoadScene("PapalBull");
    }

    public static void Election(){
        LoadScene("Election");
    }

    public static void Cards(){
        LoadScene("CardsList");
    }

    private static void LoadScene(string sceneName){
        if (!_isLoading){
            SceneManager.LoadScene(sceneName);
        }
    }

    private static IEnumerator Loading(string sceneName){
        _isLoading = true;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone){
            yield return null;
        }

        _isLoading = false;
    }

    public static void Quit(){
        Application.Quit();
    }
}