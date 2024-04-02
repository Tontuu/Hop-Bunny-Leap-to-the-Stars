using System.ComponentModel;
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
    [SerializeField]
    public float MAX_JUMP_VALUE = 50.0f;

    // Animation States
    private string currentState;
    const string PLAYER_IDLE = "player_idle";
    const string PLAYER_RUN = "player_run";
    const string PLAYER_JUMP = "player_jump";
    const string PLAYER_RISING = "player_rising";
    const string PLAYER_FALLING = "player_falling";
    const string PLAYER_LAND = "player_land";

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
    private bool isLanded = false;
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
        if (isGrounded && !isRunning && !isCharging && !isJumping && !isLanded)
        {
            ChangeAnimationState(PLAYER_IDLE);
        }
        if (isJumping && !isFalling)
        {
            ChangeAnimationState(PLAYER_RISING);
        }
        if (isCharging)
        {
            ChangeAnimationState(PLAYER_JUMP);
        }
        if (isLanded)
        {
            ChangeAnimationState(PLAYER_LAND);
        }
        else if (isRunning && !isJumping && !isCharging)
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
    }

    // Update is called once per frame
    void Update()
    {
        horizontalValue = 0;
        {
            if (Input.GetKey(KeyCode.Space) && !isJumping)
            {
                rb.velocity = new Vector2(0, 0);
                jumpValue += 0.10f;
                jumpValue = Mathf.Clamp(jumpValue, 4f, MAX_JUMP_VALUE);
                isCharging = true;
            } 

            if (Input.GetKeyUp(KeyCode.Space) || jumpValue > MAX_JUMP_VALUE)
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

        if (rb.velocity.y < -10f)
        {
            isFalling = true;
            isRising = false;
        } else if (rb.velocity.y > 5f)
        {
            isFalling = false;
            isRising = true;
        } else {
            isFalling = false;
            isRising = false;
        }

        // Get run input
        HandleStates();
    }
    void FixedUpdate()
    {
        OnLand();
        if (jump) {
            rb.velocity = new Vector2(horizontalValue * runSpeed, jumpValue);
            isCharging = false;
            isCharged = false;
            jump = false;
            jumpValue = 0f;
        }
        OnRun();
    }

    void OnLand()
    {
        if (isRunning || isJumping || isCharging)
        {
            isLanded = false;
        }
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

    void OnWallHit() 
    {
        // Invert player
        rb.velocity = new Vector2((rb.velocity.x * -1) / 3f, rb.velocity.y / 1.5f);

        if (horizontalValue < 0 && !isFacingRight)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            isFacingRight = true;
        }
        else if (horizontalValue > 0 && isFacingRight)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            isFacingRight = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall")) 
        {
            OnWallHit();
        }

        
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            isJumping = true;
        }
    }
}
