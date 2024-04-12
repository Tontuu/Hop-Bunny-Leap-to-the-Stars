using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Constants
    const float horizontalJumpSpeed = 15.0f;
    public float acceleration = 7f;
    public float decceleration = 10f;
    public float velPower = 0.9f;

    [SerializeField]
    private float runSpeed = 50.0f;
    [SerializeField]
    private float MAX_JUMP_VALUE = 35.0f;

    // Animation States
    private string currentState;
    const string PLAYER_IDLE = "player_idle";
    const string PLAYER_RUN = "player_run";
    const string PLAYER_TURNING = "player_turning";
    const string PLAYER_JUMP = "player_jump";
    const string PLAYER_RISING = "player_rising";
    const string PLAYER_FALLING = "player_falling";
    const string PLAYER_LAND = "player_land";

    // Importants
    private bool jump = false;
    private float jumpValue = 0.0f;
    Vector2 moveInput;

    // Misc
    private float oldDirOnJump;
    public float dir;
    public float oldDir;

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
    private bool isTurningDir = false;
    public bool isHittingWall = false;

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
            !isLanded && !isTurningDir)
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
        if (isTurningDir && isGrounded && !isJumping)
        {
            ChangeAnimationState(PLAYER_TURNING);
        }
        else if (isRunning && !isJumping && !isCharging && !isTurningDir)
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
        isHittingWall = false;
        dir = 0;
        oldDir = dir;
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

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
            {
                dir = Input.GetAxisRaw("Horizontal");
                if (!isTurningDir && dir != 0)
                {
                    if (dir == -Math.Sign(rb.velocity.x))
                    {
                        if (Math.Abs(rb.velocity.x) > 6f)
                        {
                            isTurningDir = true;
                            Invoke("ResetTurningDir", 0.2f);
                        }
                        else
                        {
                            setFacingDirection(dir);
                        }
                    }
                }
                if (isJumping) { oldDirOnJump = dir; }
                isRunning = true;
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
        else if (rb.velocity.y > 5f)
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
            if (!isCharging && isGrounded && !isTurningDir)
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
            if (isCharging)
            {
                isRunning = false;
            }

            // Cannot run either while charging or not on ground
            if (!isCharging && isGrounded)
            {
                float targetSpeed = dir * runSpeed;
                float speedDif = targetSpeed - rb.velocity.x;
                float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
                float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
                rb.AddForce(movement * Vector2.right);
                // rb.velocity = new Vector2(dir * runSpeed, rb.velocity.y);
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
                // rb.velocity = new Vector2(rb.velocity.x * 0.2f, rb.velocity.y);
            }
        }
    }

    void ResetTurningDir()
    {
        isTurningDir = false;
        if (!isJumping)
            setFacingDirection(dir);
    }

    void ResetJump()
    {
        oldDirOnJump = 0;
        isCharging = false;
        jumpValue = 0f;
    }
    private void setFacingDirection(float dir)
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
