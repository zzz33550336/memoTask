using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionController : MonoBehaviour
{
    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene($"Level{levelIndex}");
    }

    public void BackToStartMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
}