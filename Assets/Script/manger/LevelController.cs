using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public void ReturnToLevelSelection()
    {
        SceneManager.LoadScene("LevelSelection",LoadSceneMode.Single);
    }
}