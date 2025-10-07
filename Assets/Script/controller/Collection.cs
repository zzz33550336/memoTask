using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    private Rigidbody2D rb;
    private float timer = 0f;
    private const float collectDelay = 5f;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(100);
            AudioTool.PlaySound("coin");
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= collectDelay)
        {
            timer = 0f;
            GameManager.Instance.AddScore(100);
            AudioTool.PlaySound("coin");
            Destroy(gameObject);
        }
    }
    void fixedUpdate()
    {
        CommonTools.CheckScreenWrap(transform,rb,true);
    }
}
