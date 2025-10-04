using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LevelSelection");
       // SceneManager.UnloadSceneAsync("StartScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}