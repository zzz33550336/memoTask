using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    [SerializeField] private EdgeCollider2D colli;
    [SerializeField] private BoxCollider2D platformColli;
    [SerializeField] private CircleCollider2D atkcolli;

    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private float _flyTimeout = 5f;
    [SerializeField] private float _platformTimeout = 8f;
    private float _stateTimer;
    private enum ArrowState
    {
        Flying,
        Sliding,
        Fixed,
        None
    }
    private ArrowState currentState = ArrowState.Flying;
    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        colli = GetComponent<EdgeCollider2D>();
        atkcolli = GetComponent<CircleCollider2D>();
        platformColli = GetComponent<BoxCollider2D>();

    }
    void Start()
    {
        colli.enabled = true;
        atkcolli.enabled = false;
        platformColli.enabled = false;
        
        transform.localScale = new Vector3(rb.velocity.x > 0 ? -1 : 1, 1, 1);
        currentState = ArrowState.Flying;
    }

    // Update is called once per frame
    void Update()
    {
        _stateTimer += Time.deltaTime;

    }
    void FixedUpdate()
    {
        CommonTools.CheckScreenWrap(transform, rb,currentState!=ArrowState.None);
        switch (currentState)
        {
            case ArrowState.Flying:

                if (_stateTimer > _flyTimeout)
                {
                    Destroy(gameObject);
                }
                else
                {
                    atkcolli.enabled = true;
                }
                break;
            case ArrowState.Sliding:

                if (_stateTimer > _flyTimeout)
                {
                    Destroy(gameObject);
                }
                else
                {
                    atkcolli.enabled = false;
                }
                break;
            case ArrowState.Fixed:
                Debug.Log("ArrowState:Fixed");
                if (_stateTimer >  3f)
                {
                    float restTime= _platformTimeout-_stateTimer;
                   CommonTools.FlashCoroutine(sr, 1, 0.1f*restTime, null );
                }
                if (_stateTimer > _platformTimeout)
                {
                    Destroy(gameObject);
                }
                else
                {
                    return;

                }
                break;
            case ArrowState.None:

                //Destroy(gameObject);
                colli.enabled = false;
                atkcolli.enabled = false;
                platformColli.enabled = false;
                if (_stateTimer > _flyTimeout)
                {
                    Destroy(gameObject);
                }
                break;
            default:
                break;
        }
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == ArrowState.Fixed)return;
        
        if (((1 << collision.gameObject.layer) & buildingLayer) != 0&&_stateTimer>0.3f)
        {
            currentState = ArrowState.Sliding;
        }

    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (currentState == ArrowState.Fixed)return;
        if (((1 << collision.gameObject.layer) & buildingLayer) != 0)
        {
            currentState = ArrowState.Flying;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
{
        if (currentState == ArrowState.Fixed)
        {
            return;
        }
        if (((1 << collision.gameObject.layer) & buildingLayer) != 0 &&_stateTimer<0.5f)
        {
            Debug.Log("Fixed");
            currentState = ArrowState.Fixed;

            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            colli.enabled = false;
            atkcolli.enabled = false;
            platformColli.enabled = true;
            rb.bodyType = RigidbodyType2D.Static;
            return;
        }
        //Debug.Log($"OnTriggerEnter2D {collision.gameObject.name}");
        else if (((1 << collision.gameObject.layer) & monsterLayer) != 0)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null&&damageable.canTakeDamage)
            {
                damageable.TakeDamage(1,collision.transform.position-transform.position);
                Debug.Log("Arrow hit monster and dealt damage");
                currentState = ArrowState.None;
                rb.velocity=Vector2.zero;
            }
            else
            {
                Debug.LogWarning("Monster doesn't implement IDamageable interface: " + collision.gameObject.name);
            
            }

        
        }
        else if (((1 << collision.gameObject.layer) & playerLayer) != 0 && _stateTimer>1f)
        {
            Debug.Log("trigger atkPlayer");
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null&&player._canTakeDamage)
            {
                player.Strike();
                
            }
            currentState = ArrowState.None;
            rb.velocity=Vector2.zero;
        }
    }
}