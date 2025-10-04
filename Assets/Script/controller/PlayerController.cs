using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D coll;
    private bool wasGrounded;
    private bool isGrounded;

    [Header("Move")]
    public float moveForce = 10f;
    public float baseMoveSpeed = 3f;
    private float moveSpeed;
    private bool isStriking;
    public float jumpForce = 5f;
    private int jumpCount = 0;
    private int maxJumpCount = 1;
    [SerializeField] public LayerMask groundLayer;

    [Header("Fight")]
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _arrowSpeed = 5f;
    [SerializeField] private float _attackCD = 0.5f;
    [SerializeField] private GameObject hpui;
    private bool shooted = false;
    public bool _canTakeDamage = true;  
    private float _cdTimer;
    private bool _canAttack => _cdTimer <= 0;
    public int maxHp = 3;
    private int currentHp;
    
    [Header("Death")]
    [SerializeField] private bool isDead = false;

    // 实现IDamageable接口的canTakeDamage属性
    public bool canTakeDamage 
    { 
        get { return _canTakeDamage; }
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>(); 
    }
    
    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
        hpui.GetComponent<HPUIManager>().SetHP(currentHp);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return; 

        if (_cdTimer > 0) _cdTimer -= Time.deltaTime;

        // move
        if (Input.GetAxis("Horizontal") != 0)
        {
            Move();
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        // jump
        if (Input.GetKeyDown(KeyCode.W) && jumpCount > 0)
        {
            jumpCount--;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // atk
        if (Input.GetKeyDown(KeyCode.Space) && _canAttack)
        {
            animator.SetTrigger("shoot");
            shooted = true;
            _cdTimer = _attackCD;
        }
        
        if (!isGrounded)
        {
            if(rb.velocity.y < 0) animator.SetBool("isFalling", true);
            else animator.SetBool("isFalling", false);
        }
        
        animator.SetBool("isGrounded", isGrounded);
    }
    
    void FixedUpdate()
    {
        if (isDead) return;

        if (shooted && _cdTimer <= _attackCD - 0.32f)
        {
            SpawnArrow();
            shooted = false;
        }
        CommonTools.CheckScreenWrap(transform, rb);
        
        wasGrounded = isGrounded;
        Vector2 rayOrigin1 = new Vector2(coll.bounds.min.x, coll.bounds.min.y);
        Vector2 rayOrigin2 = new Vector2(coll.bounds.max.x, coll.bounds.min.y);
        RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin1, Vector2.down, 0.1f, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin2, Vector2.down, 0.1f, groundLayer);
         
        isGrounded = hit1.collider != null || hit2.collider != null;
        isGrounded &= rb.IsTouchingLayers(groundLayer);
        if (isLanded())
        {
            // Debug.Log("Landed, resetting jump count");
            jumpCount = maxJumpCount;
        }
        
    }
    
    private void SpawnArrow()
    {
        if (_arrowPrefab == null || _attackPoint == null) return;

        GameObject arrow = Instantiate(_arrowPrefab, _attackPoint.position+new Vector3(0,-0.1f,0), _attackPoint.rotation);

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            Vector2 attackDir = new Vector2(sr.flipX ? 1 : -1, 0);
            arrowRb.velocity = attackDir * _arrowSpeed;
        }
    }
    
    void Move()
    {
        if (isStriking)
        {
            moveSpeed = baseMoveSpeed * 0.7f;
        }
        else
        {
            moveSpeed = baseMoveSpeed;
        }
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(rb.velocity.x) < moveSpeed)
        {
            Vector2 force = new Vector2(horizontalInput * moveForce, 0f);
            rb.AddForce(force);
        }

        if (horizontalInput != 0)
        {
            sr.flipX = horizontalInput > 0;
        }
    }
    
    public void Strike()
    {
        Debug.Log("Player is striking");
        isStriking = true;
        //TakeDamage(1);
    }

    // 实现IDamageable接口的TakeDamage方法
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (_canTakeDamage)
        {
            Debug.Log("Player TakeDamage called with damage: " + damage);
            _canTakeDamage = false;
            currentHp -= damage;
            hpui.GetComponent<HPUIManager>().SetHP(currentHp);
            Debug.Log("Player took " + damage + " damage. Health: " + currentHp + "/" + maxHp);
            if (currentHp <= 0)
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
        Debug.Log("Player died");
        
        animator.SetTrigger("Die");
        
        this.enabled = false;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
    
    private bool isLanded()
    {
        return isGrounded && !wasGrounded;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
}