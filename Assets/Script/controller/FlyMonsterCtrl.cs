using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMonsterCtrl : MonoBehaviour
{
    
    public float checkDistance = 0.5f; 
    public LayerMask buildingLayer;    
    public LayerMask groundLayer;      
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject coin;
    
    private SpriteRenderer sr;
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform tf;



    [SerializeField] private int maxHealth = 2;
    private int currentHealth;
    public bool isDead = false;
    [SerializeField] 
    public float moveInterval = 0f; 
    public float moveSpeed = 2f;    
    private Vector2 moveDir;
    [SerializeField]
    private bool _canTakeDamage = true;
    
    // 实现IDamageable接口的canTakeDamage属性
    public bool canTakeDamage 
    { 
        get { return _canTakeDamage; }
        private set { _canTakeDamage = value; }
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        tf = transform;
        rb = GetComponent<Rigidbody2D>();
        
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        //buildLayer = LayerMask.GetMask("Building");
    }
    
    void Start()
    {
        rb.gravityScale = 0f;
        currentHealth = maxHealth;

        moveDir = Vector2.right;
        Debug.Log("FlyMonster initialized with " + maxHealth + " health");
        //moveTimer = moveCooldown; // so it can move immediately
        InvokeRepeating("TryMove", 0f, moveInterval);

    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
    }
    
    void FixedUpdate()
    {
        if (isDead) return;
        CommonTools.CheckScreenWrap(tf, rb, !isDead);
    }
     private void TryMove()
    {
        if (CanMove())
        {
            anim.SetTrigger("Move");
            StartCoroutine(MoveToTarget(transform.position + (Vector3)moveDir));
        }
        else
        {
            Debug.Log("Fly:Can't move, turning around");
            moveDir = -moveDir; 
        }
    }

    private bool CanMove()
    {
        Vector2 frontCheckPos = (Vector2)transform.position + moveDir * 1f;
        bool noFrontBlock = !Physics2D.Raycast(frontCheckPos, moveDir, checkDistance, buildingLayer);

        Debug.Log("noFrontBlock: " + noFrontBlock );
        return noFrontBlock ;
    }

    private System.Collections.IEnumerator MoveToTarget(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 frontCheckPos = (Vector2)transform.position + moveDir * 1f;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(frontCheckPos, frontCheckPos + moveDir * checkDistance);
    }


    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (isDead) return;
        if (!canTakeDamage) return;
        else
        {
            _canTakeDamage = false;

            currentHealth -= damage;
           /* if (audioController != null)
            {
                audioController.PlaySound();
            }*/
            Debug.Log("FlyMonster took " + damage + " damage. Health: " + currentHealth + "/" + maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                rb.AddForce(knockbackDirection * 10+new Vector2(0,1), ForceMode2D.Impulse);
                CommonTools.FlashCoroutine(sr, 3, 0.2f, () => _canTakeDamage = true);
            }
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        coll.enabled = false;
        rb.velocity += new Vector2(0, 5f);
        rb.gravityScale = 1f;
        Debug.Log("FlyMonster died");
        anim.SetTrigger("Die");
        // anounce to game manager
        // anounce to game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyDefeated();
        }
        StartCoroutine(DelayedDestroy());
        // instantiate coin
        if (coin != null)
        {
            Instantiate(coin, tf.position+new Vector3(0,0.5f,0), Quaternion.identity);
        }
    }
    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    
    /*void Move()
    {
        if (isDead) return;
        transform.Translate(Vector2.right * moveSpeed * moveDirection * Time.deltaTime);
    }
    */
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("Slime collided with player");
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null && damageable.canTakeDamage)
            {
                damageable.TakeDamage(1, other.transform.position-transform.position);
                Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 2f);
                }
            }
            /*
            else
            {
                PlayerController player = other.gameObject.GetComponent<PlayerController>();
                if (player != null && player.canTakeDamage)
                {
                    player.TakeDamage(1);

                    Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.velocity = new Vector2(playerRb.velocity.x, 2f);
                    }
                }
            }*/
        }
    }
}
