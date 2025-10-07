using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsMove : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CommonTools.CheckScreenWrap(transform, rb,true);
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
    }
}
