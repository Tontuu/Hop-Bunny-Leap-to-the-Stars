using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEditor.Rendering;
using UnityEngine.Diagnostics;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Constants
    const float horizontalJumpSpeed = 15.0f;
    const float runSpeed = 18.0f;
    const float MAX_JUMP_MAGNITUDE = 52.0f;
    const float JUMP_CHARGE_MAGNITUDE = 1.00f;
    const float MIN_JUMP_MAGNITUDE = 15.0f;
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
    public ParticleSystem dust;
    public ParticleSystem wallDust;
    public ParticleSystem landDust;
    private float oldDirOnJump;
    private float dir;
    private float desacceleration_value = 0.80f;

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
        {
            if (Input.GetKey(KeyCode.Space) && !isJumping)
            {
                isCharging = true;
            }

            if (Input.GetKeyUp(KeyCode.Space))
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


        // Handle animation states
        HandleStates();
    }
    void FixedUpdate()
    {

        if (isCharging)
        {
            rb.velocity = new Vector2(0, 0);
            jumpValue += JUMP_CHARGE_MAGNITUDE;
            jumpValue = Math.Clamp(jumpValue, MIN_JUMP_MAGNITUDE, MAX_JUMP_MAGNITUDE);
        }
        OnLand();
        if (jump)
        {
            setFacingDirection(dir);
            rb.velocity = new Vector2(dir * horizontalJumpSpeed, jumpValue);
            isCharging = false;
            isCharged = false;
            jump = false;
            CreateDust(jumpValue);
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
        }
        else
        {
            // Only lose the velocity on the ground, not on the air (falling)
            if (isGrounded && dir == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x * desacceleration_value, rb.velocity.y);
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
        CreateWallDust(rb.velocity.magnitude);
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

    void CreateDust(float magnitude)
    {
        int speed = Mathf.RoundToInt(Utils.Map(magnitude, MIN_JUMP_MAGNITUDE, MAX_JUMP_MAGNITUDE, 2f, 6f));
        int emissionAmount = Mathf.RoundToInt(Utils.Map(magnitude, MIN_JUMP_MAGNITUDE, MAX_JUMP_MAGNITUDE, 20f, 120f));
        ParticleSystem.VelocityOverLifetimeModule dustVel = dust.velocityOverLifetime;
        ParticleSystem.EmissionModule dustEmission = dust.emission;
        dustEmission.rateOverTime = emissionAmount;
        dustVel.speedModifier = speed;
        dust.Play();
    }

    void CreateWallDust(float magnitude)
    {
        ParticleSystem.EmissionModule dustEmission = wallDust.emission;
        ParticleSystem.ShapeModule dustShape = wallDust.shape;
        ParticleSystem.VelocityOverLifetimeModule dustVelocity = wallDust.velocityOverLifetime;
        int emissionAmount = Mathf.RoundToInt(Utils.Map(magnitude, 17f, 45f, 80f, 120f));
        int x_offset = 0;

        if (isFacingRight)
        {
            x_offset = -1;
        }
        else
        {
            x_offset = 1;
        }
        dustShape.position = new Vector3(-x_offset * 2, 1f, 0f);
        dustVelocity.xMultiplier = x_offset;
        dustEmission.rateOverTime = emissionAmount;

        wallDust.Play();
    }

    void CreateLandDust(float magnitude)
    {
        int speed = Mathf.RoundToInt(Utils.Map(magnitude, 0f, 80f, 1f, 6f));
        int emissionAmount = Mathf.RoundToInt(Utils.Map(magnitude, 0f, 80f, 20f, 120f));
        ParticleSystem.VelocityOverLifetimeModule dustVel = landDust.velocityOverLifetime;
        ParticleSystem.EmissionModule dustEmission = landDust.emission;
        dustEmission.rateOverTime = emissionAmount;
        dustVel.speedModifier = speed;
        landDust.Play();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            OnWallHit();
        }


        if (other.gameObject.CompareTag("Ground"))
        {
            CreateLandDust(rb.velocity.magnitude);
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
