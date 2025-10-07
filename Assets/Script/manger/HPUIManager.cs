using UnityEngine;
using UnityEngine.UI;

public class HPUIManager : MonoBehaviour
{
    public Image HPUI; 
    public Sprite HP3;   
    public Sprite HP2;   
    public Sprite HP1;    
    public Sprite HP0;

    private int currentHP; 
    private int maxHP=3; 

    void Start()
    {
        currentHP = maxHP; 
        UpdateHPUI();
    }

    public void SetHP(int newHP)
    {
        Debug.Log("SetHP called with newHP: " + newHP);
        currentHP = Mathf.Clamp(newHP, 0, maxHP); 
        UpdateHPUI();
    }

    private void UpdateHPUI()
    {
        switch (currentHP) {
            case 3:
                HPUI.sprite = HP3;
                break;
            case 2:
                HPUI.sprite = HP2;
                break;
            case 1:
                HPUI.sprite = HP1;
                break;
            case 0:
                HPUI.sprite = HP0;
                break;
            default:

                Debug.LogError("Invalid HP value: " + currentHP);
                HPUI.sprite = HP0;
                break;
        }
    }
}