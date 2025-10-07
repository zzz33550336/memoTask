using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    [Header("UI References")]
    public GameObject PauseMenuUI;
    public GameObject gameOverUI;
    public GameObject victoryUI;
    public TextMeshProUGUI resultText;

    [Header("Settings")]
    public string levelSelectScene = "LevelSelection";
    public float delayBeforeLoad = 2f;

    [Header("Game Objects")]
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject monsterParent;
    /*
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject victoryUI;
    */
    private int enemyCount; 
    private bool isLevelComplete; // 防止重复触发胜利条件
    private int currentLevelIndex;
    private GameState currentState = GameState.Menu;

    public GameState CurrentState => currentState;

    private int totalMonsters = 0;
    private int defeatedMonsters = 0;
    public int score = 0;
    public void AddScore(int score)
    {
        CommonTools.addScore(score);
    }
    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        HandleInput();
        if (currentState == GameState.Playing)
        {
            CheckGameConditions();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("level"))
        {
            currentLevelIndex = int.Parse(scene.name.Replace("level", ""));
            InitializeLevel();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeGame()
    {
        isLevelComplete = false;
        if (PauseMenuUI) PauseMenuUI.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);
        if (victoryUI) victoryUI.SetActive(false);
        //if (PauseMenuUI) PauseMenuUI.SetActive(false);
        /*
        enemyCount = GameObject.FindGameObjectsWithTag("Monster").Length;
        Debug.Log($"当前怪物数: {enemyCount}");
*/
        if (monsterParent != null)
        {
            totalMonsters = monsterParent.transform.childCount;
            Debug.Log($"Total monsters in scene: {totalMonsters}");
            enemyCount = totalMonsters;
        }
        else
        {
            Debug.LogError("Monster parent not assigned!");
        }
        StartGame();
    }

    private void InitializeLevel()
    {
        Debug.Log($"Initializing level {currentLevelIndex}");
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        Debug.Log("Game started");

        if (resultText != null)
            resultText.text = "";
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                currentState = GameState.Paused;
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                currentState = GameState.Playing;
                ResumeGame();
            }
        }
    }

    private void CheckGameConditions()
    {
        if (player != null && player.IsDead())
        {
            currentState = GameState.GameOver;
            GameOver();
            return;
        }

        if (monsterParent != null)
        {
            if (defeatedMonsters >= totalMonsters)
            {
                //AudioTool.PlaySound("victory");
                currentState = GameState.Victory;
                Victory();
            }
        }
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;
        Time.timeScale = 0f;
        if (PauseMenuUI) PauseMenuUI.SetActive(true);
        Debug.Log("Game paused");
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        if (PauseMenuUI) PauseMenuUI.SetActive(false);
        Debug.Log("Game resumed");
    }

    public void GameOver()
    {
        currentState = GameState.GameOver;
        //Time.timeScale = 0f;
        if (gameOverUI) gameOverUI.SetActive(true);
        Debug.Log("Game over");
    }

    public void Victory()
    {
        //if (isLevelComplete) return; 
    
        currentState = GameState.Victory;
        isLevelComplete = true;

        if (victoryUI) victoryUI.SetActive(true);
        if(resultText)resultText.text = "Your score: " + score;
        else Debug.LogError("Result text not assigned!");
        Debug.Log("Victory!");

    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (currentState == GameState.Paused)
        {
            ResumeGame();
        }
    }

    public void RestartGame()
    {
        
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        Debug.Log("Game restarted");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void ReturnToMenu()
    {
        Debug.Log("Return to menu");
        Time.timeScale = 1f;

        SceneManager.LoadScene(levelSelectScene, LoadSceneMode.Single);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadNextLevel()
    {
        int nextLevelIndex =2;
        string nextLevelName = $"level{nextLevelIndex}";
        SceneManager.LoadScene(nextLevelName);
        Debug.Log($"Loading next level: {nextLevelName}");
    }

    public void ReturnToSelect()
    {
        SceneManager.LoadScene(levelSelectScene);
        Debug.Log("Returning to level selection");
    }

    // 当敌人被击败时调用
    public void OnEnemyDefeated()
    {
        enemyCount--;
        //defeatedMonsters++;

        if (enemyCount <= 0 && !isLevelComplete)
        {
            isLevelComplete = true;
            Victory();
           // StartCoroutine(ShowResult(true));
        }
    }

   private IEnumerator ShowResult(bool isVictory)
    {
       // if (resultText)
           // resultText.text = isVictory ? "胜利！" : "失败！";

        //if (resultText)
           // resultText.text = isVictory ? "胜利！" : "失败！";

        
        yield return new WaitForSeconds(delayBeforeLoad);

        if (isVictory)
        {
            
        }
    }
}