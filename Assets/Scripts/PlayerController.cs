using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Constants
    const float horizontalJumpSpeed = 15.0f;
    const float runSpeed = 18.0f;
    const float MAX_JUMP_MAGNITUDE = 52.0f;
    const float JUMP_CHARGE_MAGNITUDE = 0.40f;
    const float MIN_CHARGE_MAGNITUDE = 10.0f;
    const float PLAYER_GRAVITY = 7.0f;

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
    public float jumpValue = 0.0f;
    Vector2 moveInput;

    // Misc
    private float oldDirOnJump;
    private float dir;
    private float oldDir;

    // Conditions
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
        dir = 0;
        oldDir = dir;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = PLAYER_GRAVITY;
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
        if (isGrounded && !isRunning &&
            !isCharging && !isJumping &&
            !isLanded)
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
        dir = 0;
        oldDir = dir;
        {
            if (Input.GetKey(KeyCode.Space) && !isJumping)
            {
                rb.velocity = new Vector2(0, 0);
                jumpValue += JUMP_CHARGE_MAGNITUDE;
                jumpValue = Mathf.Clamp(jumpValue, MIN_CHARGE_MAGNITUDE, MAX_JUMP_MAGNITUDE);
                isCharging = true;
            }

            if (jumpValue >= MAX_JUMP_MAGNITUDE || Input.GetKeyUp(KeyCode.Space))
            {
                isCharged = true;
            }

            if (isCharging == true && isCharged == true)
            {
                jump = true;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
            {
                // Don't move if both keys are being pressed.
                if (!(Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)))
                {
                    dir = Input.GetAxisRaw("Horizontal");
                    if (dir != 0)
                    {
                        if (dir == -Math.Sign(rb.velocity.x))
                        {
                            setFacingDirection(dir);
                        }
                    }
                    if (isJumping) { oldDirOnJump = dir; }
                    isRunning = true;
                }
            }
            else
            {
                isRunning = false;
            }
        }

        if (rb.velocity.y < -10f)
        {
            isFalling = true;
            isRising = false;
        }
        else if (rb.velocity.y > 1.0f)
        {
            isFalling = false;
            isRising = true;
        }
        else
        {
            isFalling = false;
            isRising = false;
        }


        if (isRunning)
        {
            if (!isCharging && isGrounded)
            {
                setFacingDirection(dir);
            }
        }


        // Get run input
        HandleStates();
    }
    void FixedUpdate()
    {
        OnLand();
        if (jump)
        {
            setFacingDirection(dir);
            rb.velocity = new Vector2(dir * horizontalJumpSpeed, jumpValue);
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
            if (isRunning && dir == 0)
            {
                isRunning = false;
            }
            if (isCharging)
            {
                isRunning = false;
            }

            // Cannot run either while charging or not on ground
            if (!isCharging && isGrounded)
            {
                rb.velocity = new Vector2(dir * runSpeed, rb.velocity.y);
            }

            if (isGrounded && dir == 0)
            {
            }

        }
        else
        {
            // Only lose the velocity on the ground, not on the air (falling)
            if (isGrounded && dir == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x * 0.2f, rb.velocity.y);
            }
        }
    }

    void ResetJump()
    {
        oldDirOnJump = 0;
        isCharging = false;
        jumpValue = 0f;
    }
    private void setFacingDirection(float dir)
    {
        if (!isJumping)
        {
            if (dir > 0.0 && !isFacingRight)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                isFacingRight = true;
            }
            else if (dir < 0.0 && isFacingRight)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                isFacingRight = false;
            }
        }
    }

    void OnWallHit()
    {
        // Invert player
        rb.velocity = new Vector2((rb.velocity.x * -1) / 2.5f, rb.velocity.y / 1.25f);

        if (oldDirOnJump < 0.0 && !isFacingRight)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            isFacingRight = true;
        }
        else if (oldDirOnJump > 0.0 && isFacingRight)
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
