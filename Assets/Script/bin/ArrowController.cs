using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class ArrowController : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private BoxCollider2D colli;
    private EdgeCollider2D edgeColli;

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
    void Start()
    {

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        colli = GetComponent<BoxCollider2D>();
        edgeColli = GetComponent<EdgeCollider2D>();
        colli.enabled = true;
        edgeColli.enabled = false;
        // arrow direction
        sr.flipX = (rb.velocity.x > 0);
        

    }

    // Update is called once per frame
    void Update()
    {
        _stateTimer += Time.deltaTime;
        if (currentState == ArrowState.Fixed)
        { 
            if (_stateTimer > _platformTimeout)
            {
                Destroy(gameObject);
            }
        }
        else{
            if (_stateTimer > _flyTimeout)
            {
                Destroy(gameObject);
            }
            currentState = rb.IsTouchingLayers(buildingLayer) ? ArrowState.Flying : ArrowState.Sliding;
            switch (currentState)
            {
                case ArrowState.Flying:
                    colli.enabled = false;
                    edgeColli.enabled = true;
                    if (colli.IsTouchingLayers(buildingLayer))
                    {
                        currentState = ArrowState.Sliding;
                    }
                    else if (edgeColli.IsTouchingLayers(monsterLayer))
                    {
                        IDamageable monster = edgeColli.IsTouchingLayers(monsterLayer) ? edgeColli.GetComponent<IDamageable>() : null;
                        if (monster != null)
                        {
                            monster.TakeDamage(1);
                        }
                        currentState = ArrowState.None;
                    }
                    else if (edgeColli.IsTouchingLayers(playerLayer))
                    {
                        // player.Strike();
                        currentState = ArrowState.None;
                    }
                    else if (edgeColli.IsTouchingLayers(buildingLayer))
                    {
                        currentState = ArrowState.Fixed;
                    }

                    break;
                case ArrowState.Sliding:
                    colli.enabled = true;
                    edgeColli.enabled = false;
                    break;
                case ArrowState.Fixed:
                    colli.enabled = true;
                    edgeColli.enabled = false;
                    break;
                case ArrowState.None:
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
*/