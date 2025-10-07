using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jmpBoardCtrl : MonoBehaviour
{
    private Collider2D collider;
    private Animator anim;
    private float jmpBoardCoolDown = 1f;
    private float jmpBoardCoolDownTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        jmpBoardCoolDownTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (jmpBoardCoolDownTimer > 0)
        {
            jmpBoardCoolDownTimer -= Time.deltaTime;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jmpBoardCoolDownTimer <= 0)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player jumped on the board");
                jmpBoardCoolDownTimer = jmpBoardCoolDown;
                anim.SetTrigger("jmpBoardTrigger");
                other.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 15), ForceMode2D.Impulse);
            }
        }
    }
}
