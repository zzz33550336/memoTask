using UnityEngine;
using UnityEngine.UI;

public class UICanvasSetup : MonoBehaviour
{
    public Camera mainCamera; 

    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = mainCamera; 
        }
    }
}