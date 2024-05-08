using System;
using System.Collections;
using System.Collections.Generic;
using Aarthificial.PixelGraphics.Universal;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

public class FireflyController : MonoBehaviour
{

    // Importants
    private float latestDirectionChangeTime;
    [SerializeField]
    private float fireflyVelocity = -4f;
    private readonly float directionChangeTime = 8f;
    public Vector2 movementDirection;
    private Vector2 movementPerSecond;

    // Conditions
    private bool isOnTop = false;
    private bool isOnWall = false;
    private bool isWalking = false;
    private bool isGrounded = true;
    private bool isRotated = false;
    private bool isFacingRight = true;

    // Animation States
    private string currentState;
    const string FF_FLYING = "flying";
    const string FF_WALKING = "walking";

    // Unity items
    Rigidbody2D rb;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movementDirection = new Vector2(fireflyVelocity, 0);
        SetDirection(fireflyVelocity);

        if (IsOnWall(Vector2.up))
        {
            Debug.Log("pinto");
        } else if (IsOnWall(Vector2.down))
        {
            // Debug.Log("cu");
        } else if (IsOnWall(Vector2.left))
        {
            if (IsOnWall(Vector2.down))
            {
                fireflyVelocity = Math.Abs(fireflyVelocity);
            } else 
            {
                movementDirection = new Vector2(movementDirection.x, Math.Abs(fireflyVelocity));
                RotateSprite(-90);
            }
        } else if (IsOnWall(Vector2.right))
        {
            if (IsOnWall(Vector2.down))
            {
                fireflyVelocity = -Math.Abs(fireflyVelocity);
            } else 
            {
                movementDirection = new Vector2(movementDirection.x, -Math.Abs(fireflyVelocity));
                RotateSprite(90);
            }
        } else 
        {
        }

        // Handle animation states
        HandleStates();
    }
    private void FixedUpdate()
    {
        rb.velocity = movementDirection;
    }

    void SetDirection(float axis)
    {
        if (axis > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        } else 
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    void HandleStates()
    {
        if (isGrounded)
        {
            ChangeAnimationState(FF_WALKING);
        }
        else
        {
            ChangeAnimationState(FF_FLYING);
        }
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }

    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;

        float distance = 1.0f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, LayerMask.GetMask("Platform"));
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    bool IsOnWall(Vector2 direction)
    {
        Vector2 position = transform.position;

        float distance = 1.0f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, LayerMask.GetMask("Platform"));
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    void RotateSprite(float degree)
    {
        if (!isRotated)
        {
            transform.Rotate(0, 0, degree);
            isRotated = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isOnTop)
        {
        }

        if (other.gameObject.CompareTag("Wall"))
        {
        }

        if (other.gameObject.CompareTag("Ground"))
        {
        }
    }
}

