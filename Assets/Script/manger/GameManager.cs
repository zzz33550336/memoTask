using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    //[Header("Game State")]
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
    
    [SerializeField] private GameState currentState = GameState.Menu;
    public GameState CurrentState => currentState;
    
    [Header("Game Objects")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform MonsterParent; 
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject victoryUI;
    
    [Header("Game Settings")]
    //[SerializeField] private float restartDelay = 2f;
    
    private int totalSlimes = 0;
    private int defeatedSlimes = 0;
    
    void Awake()
    {
        // 确保只有一个GameManager实例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // 初始化游戏
        InitializeGame();
    }
    
    void Update()
    {
        HandleInput();
        
        // 检查游戏状态
        switch (currentState)
        {
            case GameState.Playing:
                CheckGameConditions();
                break;
        }
    }
    
    private void InitializeGame()
    {
        // 初始化游戏状态
        currentState = GameState.Menu;
        
        // 隐藏所有UI
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);
        if (victoryUI) victoryUI.SetActive(false);
        
        // monster total
        if (MonsterParent != null)
        {
            totalSlimes = MonsterParent.childCount;
            Debug.Log("Total slimes in scene: " + totalSlimes);
        }
        
        // 开始游戏
        StartGame();
    }
    
    public void StartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        Debug.Log("Game started");
    }
    
    private void HandleInput()
    {
        // 按ESC键暂停/取消暂停
        if (Input.GetKeyDown(KeyCode.Escape))
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
    }
    
    private void CheckGameConditions()
    {
        // 检查玩家是否死亡
        if (player != null && player.IsDead())
        {
            GameOver();
            return;
        }
        
        // check all monsters
        if (MonsterParent != null)
        {
            int currentSlimes = 0;
            foreach (Transform child in MonsterParent)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    currentSlimes++;
                }
            }
            
            // 如果史莱姆数量为0，玩家获胜
            if (currentSlimes <= 0)
            {
                Victory();
            }
        }
    }
    
    public void PauseGame()
    {
        currentState = GameState.Paused;
        Time.timeScale = 0f;
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
        Debug.Log("Game paused");
    }
    
    public void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        Debug.Log("Game resumed");
    }
    
    public void GameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0f;
        if (gameOverUI) gameOverUI.SetActive(true);
        Debug.Log("Game over");
    }
    
    public void Victory()
    {
        currentState = GameState.Victory;
        Time.timeScale = 0f;
        if (victoryUI) victoryUI.SetActive(true);
        Debug.Log("Victory!");
    }
    
    // 通过按钮调用的暂停方法
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
    
    // 重新开始游戏
    public void RestartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Game restarted");
    }
    
    // 返回主菜单
    public void ReturnToMenu()
    {
        currentState = GameState.Menu;
        Time.timeScale = 1f;
        // 这里可以加载主菜单场景
        // SceneManager.LoadScene("MainMenu");
        Debug.Log("Return to menu");
    }
    
    // 退出游戏
    public void QuitGame()
    {
        Debug.Log("Quitting game");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    // 史莱姆被击败时调用
    public void OnSlimeDefeated()
    {
        defeatedSlimes++;
        Debug.Log("Slime defeated. Total defeated: " + defeatedSlimes);
    }
}