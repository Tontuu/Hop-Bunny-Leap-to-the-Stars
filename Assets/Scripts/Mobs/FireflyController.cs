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
    public float fireflyVelocity = 4f;
    private readonly float directionChangeTime = 8f;
    private Vector2 movementDirection;
    private Vector2 movementPerSecond;

    // Conditions
    private bool isOnTop = false;
    private bool isOnWall = false;
    private bool isWalking = false;
    private bool isGrounded = true;

    // Animation States
    private string currentState;

    // Unity items
    Rigidbody2D rb;
    Animator animator;
    const string FF_FLYING = "flying";
    const string FF_WALKING = "walking";

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        ChangeAnimationState("flying");


        latestDirectionChangeTime = 0f;
        calculateNewMovementVector();
    }


    void HandleStates()
    {
        if (isWalking)
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

    void calculateNewMovementVector()
    {
        // Create a random direction vector with the magnitude of 1, later multiply it with the velocity of the enemy
        movementDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * fireflyVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        //if the changeTime was reached, calculate a new movement vector
        if (Time.time - latestDirectionChangeTime > directionChangeTime)
        {
            latestDirectionChangeTime = Time.time;
            calculateNewMovementVector();
        }

        // Moving firefly
        rb.velocity = movementDirection;

        // Check facing direction
        if (rb.velocity.x < 0)
        {
            if (isOnWall)
            {
                if (rb.velocity.y < 0) GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        else
        {
            if (isOnWall)
            {
                if (rb.velocity.y < 0) GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
        }

        if (isOnTop)
        {
            GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipY = false;
        }

        // Handle animation states
        HandleStates();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isWalking = true;
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            // Check if is colliding with top otherwise colliding with walls
            Vector2 originPoint = transform.position;
            Vector2 rayDirectionTop = Vector2.up;
            RaycastHit2D hittingTop = Physics2D.Raycast(originPoint, rayDirectionTop, 5f, LayerMask.GetMask("Platform"));

            if (hittingTop.collider != null)
            {
                isOnTop = true;
                isWalking = true;
            }
            else
            {
                isWalking = true;
                isOnWall = true;
                Vector2 rayDirectionLeft = Vector2.left;
                RaycastHit2D hittingLeft = Physics2D.Raycast(originPoint, rayDirectionLeft, 5f, LayerMask.GetMask("Platform"));
                if (hittingLeft.collider != null)
                {
                    transform.Rotate(new Vector3(0, 0, -90));
                }
                else
                {
                    transform.Rotate(new Vector3(0, 0, 90));
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isOnTop)
        {
            isOnTop = false;
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            isOnWall = false;
            isWalking = false;
            ChangeAnimationState(FF_FLYING);
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            isWalking = false;
            isGrounded = false;
            ChangeAnimationState(FF_FLYING);
        }
    }
}


