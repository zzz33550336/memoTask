using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour, IDamageable
{
    [SerializeField] private LayerMask buildLayer;
    [SerializeField] private LayerMask playerLayer;
    private SpriteRenderer sr;
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;
    [SerializeField] private float moveSpeed = 2f;
    private int moveDirection = -1;
    private bool canMove = true;
    private float moveTimer = 0f;
    [SerializeField]
    private float moveCooldown = 3f;

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
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        //buildLayer = LayerMask.GetMask("Building");
    }
    
    void Start()
    {
        currentHealth = maxHealth;

        Debug.Log("Slime initialized with " + maxHealth + " health");
        moveTimer = moveCooldown; // so it can move immediately
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        Vector2 rayOrigin = new Vector2(coll.bounds.center.x, coll.bounds.center.y);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * moveDirection, 2f, buildLayer);
        if (hit.collider != null)
        {
            canMove = false;

            moveDirection = -moveDirection;
        }
        else
        {
            canMove = true;
            if (moveTimer >= moveCooldown)
            {
                anim.SetTrigger("Move");
                moveTimer = 0f;
                transform.localScale = new Vector3(moveDirection, 1, 1);
            }
            else
            {
                moveTimer += Time.deltaTime;
                if (moveTimer >= 1 && moveDirection <= 2.5f) Move();
            }
        }
    }
    
    void FixedUpdate()
    {
        if (isDead) return;
        CommonTools.CheckScreenWrap(transform, rb);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (!canTakeDamage) return;
        else
        {
            _canTakeDamage = false;

            currentHealth -= damage;
            Debug.Log("Slime took " + damage + " damage. Health: " + currentHealth + "/" + maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            CommonTools.TakeDamageEffect(gameObject, 3, 0.3f, this, () => _canTakeDamage = true);
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Slime died");
        anim.SetTrigger("Die");
        // anounce to game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSlimeDefeated();
        }

        Destroy(gameObject);
    }
    
    void Move()
    {
        transform.Translate(Vector2.right * moveSpeed * moveDirection * Time.deltaTime);
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("Slime collided with player");
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null && damageable.canTakeDamage)
            {
                damageable.TakeDamage(1);
                Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 2f);
                }
            }
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
            }
        }
    }
}