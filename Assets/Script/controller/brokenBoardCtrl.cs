using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brokenBoardCtrl : MonoBehaviour
{   private Collider2D collider;
    private Animator anim;
    private SpriteRenderer sr;
    private float brkBoardCoolDown = 1f;
    private float brkBoardCoolDownTimer = 0f;
    public float duration = 1f; 
    private float elapsed = 0f;
    // Start is called before the first frame update
    public enum BrokenBoardState
    {
        ready,
        Broken
    }
    private BrokenBoardState currState;
    void Start()
    {
        currState = BrokenBoardState.ready;
        collider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        brkBoardCoolDownTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (brkBoardCoolDownTimer > 1f)
        {
            brkBoardCoolDownTimer -= Time.deltaTime;
        }
    }
    private IEnumerator breakBoard()
    {
        
        yield return new WaitForSeconds(1f);
        
        Color originalColor = sr.color;
        collider.enabled = false;
        
        anim.SetTrigger("Break");
        collider.enabled = false;   
        yield return new WaitForSeconds(0.6f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        yield return new WaitForSeconds(1f);
        anim.SetTrigger("Repair");
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        collider.enabled = true;
        sr.color = Color.white;
        currState = BrokenBoardState.ready;
    }
    

    public void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("OnCollisionEnter2D called");
        if (other.gameObject.CompareTag("Player")&&currState == BrokenBoardState.ready)
        {
            Debug.Log("board broken");
            currState = BrokenBoardState.Broken;
            StartCoroutine(breakBoard());
            }
     
        
    }
}
