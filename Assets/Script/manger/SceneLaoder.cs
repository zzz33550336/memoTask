using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    private static EventSystemManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // ȷ���ö��󲻱�ж��
        }
        else
        {
            Destroy(gameObject); // �����ظ�ʵ��
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureEventSystemExists();
    }

    private void EnsureEventSystemExists()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("EventSystem not found in the current scene. Creating a new one...");

            // ����һ���µ�EventSystem����
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();

            // ȷ��EventSystem���󲻻ᱻж��
            DontDestroyOnLoad(eventSystemObj);
        }
    }
}