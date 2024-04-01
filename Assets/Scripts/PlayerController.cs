using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Constants
    const float runSpeed = 17.0f;
    const float jumpImpulse = 20.0f;

    // Animation States
    private string currentState;
    const string PLAYER_IDLE = "player_idle";
    const string PLAYER_RUN = "player_run";
    const string PLAYER_JUMP = "player_jump";
    const string PLAYER_RISING = "player_rising";
    const string PLAYER_FALLING = "player_falling";

    // Importants
    private bool jump = false;
    private float horizontalValue;
    private float jumpValue = 0.0f;
    Vector2 moveInput;

    // Conditions
    private bool canJump = true;
    private bool isCharging = false;
    private bool isCharged = false;
    private bool isGrounded = false;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isRising = false;
    private bool isFalling = false;
    private bool isFacingRight = true;

    // Unity items
    Rigidbody2D rb;
    Animator animator;


    /// ================================================
    // Start is called before the first frame update
    /// ================================================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.Play("player_idle");
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }

    void HandleStates()
    {
        if (isCharging)
        {
            ChangeAnimationState(PLAYER_JUMP);
        }
        else if (isRunning)
        {
            ChangeAnimationState(PLAYER_RUN);
        } 
        else if (isRising) 
        {
            ChangeAnimationState(PLAYER_RISING);
        }
        else if (isFalling) 
        {
            ChangeAnimationState(PLAYER_FALLING);
        }
        else
        {
            ChangeAnimationState(PLAYER_IDLE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Velocidade X: " + rb.velocity.x);
        Debug.Log("Velocidade Y: " + rb.velocity.y);
        horizontalValue = 0;
        if (isGrounded)
            isJumping = false;
        {
            if (Input.GetKey(KeyCode.Space))
            {
                rb.velocity = new Vector2(0, 0);
                jumpValue += 0.10f;
                jumpValue = Mathf.Clamp(jumpValue, 4f, 25f);
                isCharging = true;
            } 

            if (Input.GetKeyUp(KeyCode.Space) || jumpValue > 25f)
            {
                isCharged = true;
            }

            if (isCharging == true && isCharged == true) 
            {
                jump = true;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)) {
                horizontalValue = Input.GetAxisRaw("Horizontal");
                isRunning = true;
            } else {
                isRunning = false;
            }
        }
        isFalling = (rb.velocity.y < 0.0f);
        isRising = (rb.velocity.y > 0.0f);

        // Get run input
        HandleStates();
    }
    void FixedUpdate()
    {
        if (jump) {
            Debug.Log(isCharged);
            Debug.Log(jumpValue);
            rb.velocity = new Vector2(horizontalValue * runSpeed, jumpValue);
            jumpValue = 0f;
            isCharging = false;
            isCharged = false;
            isJumping = true;
            jump = false;
        }
        OnRun();
    }

    void OnRun()
    {
        if (isRunning)
        {
            if (isCharging)
            {
                isRunning = false;
                // rb.velocity = new Vector2(0, rb.velocity.y);
            }

            // Cannot run either while charging or not on ground
            if (!isCharging && isGrounded)
            {
                rb.velocity = new Vector2(horizontalValue * runSpeed, rb.velocity.y);
                setFacingDirection(horizontalValue);
            }
        }
        else
        {
            // Only lose the velocity on the ground, not on the air (falling)
            if (isGrounded)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }
    void ResetJump()
    {
        isCharging = false;
        canJump = false;
        jumpValue = 0f;
    }
    private void setFacingDirection(float horizontalValue)
    {
        if (horizontalValue > 0 && !isFacingRight)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            isFacingRight = true;
        }
        else if (horizontalValue < 0 && isFacingRight)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            isFacingRight = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
