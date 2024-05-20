using UnityEngine;
using System;
using Cinemachine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Constants
    const float horizontalJumpSpeed = 15.0f;
    const float runSpeed = 15.0f;
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

    // Misc
    public ParticleSystem dust;
    public ParticleSystem wallDust;
    public ParticleSystem landDust;
    public ParticleSystem turningDust;
    private float oldDirOnJump;
    public float dir;
    public float desacceleration_value = 0.9f;
    [SerializeField]
    public float acceleration = 8f;
    public float deceleration = 5f;

    // Conditions
    public bool isLookingUp = false;
    private bool isCharging = false;
    private bool isCharged = false;
    private bool isGrounded = false;
    private bool isLanded = false;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isRising = false;
    private bool isFalling = false;
    private bool isFacingRight = true;
    public bool isGoingRight = false;
    public bool isGoingLeft = false;
    public bool isTurningDirection = false;
    public bool isHittingWall = false;

    // Unity items
    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera lookUpCam;
    Rigidbody2D rb;
    Animator animator;

    // Temp
    public float PrevVelocityX = 0.0f;


    /// ================================================
    // Start is called before the first frame update
    /// ================================================
    void Start()
    {
        dir = 0;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = PLAYER_GRAVITY;
        animator = GetComponent<Animator>();
        PrevVelocityX = rb.velocity.x;
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

        animator.SetBool("IsLookingUp", isLookingUp);
    }

    // Update is called once per frame
    void Update()
    {
        float currentVelX = rb.velocity.x;
        dir = 0;
        // Handle inputs
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

                    // Get direction for turning animations
                    isGoingRight = dir > 0;
                    isGoingLeft = dir < 0;

                    if (isJumping) { oldDirOnJump = dir; }
                    isRunning = true;
                }
            }
            else
            {
                isGoingLeft = false;
                isGoingRight = false;
                isRunning = false;
            }

            // Turning direction on animation
            // Check if the velocity changes from positive to negative or from negative to positive
            if (!isGrounded)
            {
                isTurningDirection = false;
            }

            if ((isGoingRight && rb.velocity.x < -2) || (isGoingLeft && rb.velocity.x > 2))
            {
                // Avoid bug when hitting on a wall and suddenly changing direction
                if (!isHittingWall)
                {
                    CreateTurningDust();
                    isTurningDirection = true;
                }
            }

            if (!isRunning && isGrounded && !isCharging)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    isLookingUp = true;
                }

                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    isLookingUp = false;
                }
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
                animator.SetBool("IsTurningDirection", isTurningDirection);
            }
        }

        PrevVelocityX = currentVelX;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            isTurningDirection = false;
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
                Vector2 currentVelocity = rb.velocity;

                // Calculate target velocity with acceleration  
                Vector2 targetVelocity = new Vector2(dir * runSpeed, rb.velocity.y);

                // Calculate the change in velocity  
                Vector2 deltaVelocity = targetVelocity - currentVelocity;

                // Apply acceleration or deceleration  
                Vector2 accelerationVector = deltaVelocity.normalized * (acceleration * Time.fixedDeltaTime);

                // Clamp acceleration so it doesn't exceed max acceleration  
                if (accelerationVector.sqrMagnitude > deltaVelocity.sqrMagnitude)
                {
                    accelerationVector = deltaVelocity;
                }

                // Update the velocity  
                rb.velocity = new Vector2(rb.velocity.x + accelerationVector.x * 10f, rb.velocity.y);
                // // Limit the velocity to the top speed  
                // rb.velocity = Vector2.ClampMagnitude(rb.velocity, runSpeed);


                // rb.velocity = new Vector2(dir * runSpeed, rb.velocity.y);
            }
        }
        else
        {
            // Only lose the velocity on the ground, not on the air (falling)
            if (isGrounded && dir == 0)
            {
                Vector2 decelerationVector = -rb.velocity.normalized * (deceleration * 10f * Time.fixedDeltaTime);
                rb.velocity += decelerationVector;

                // Ensure velocity doesn't go below zero  
                if (Vector2.Dot(rb.velocity + decelerationVector, decelerationVector) > 0f)
                {
                    rb.velocity = Vector2.zero;
                }

                // rb.velocity = new Vector2(rb.velocity.x * desacceleration_value, rb.velocity.y);
            }
        }
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
        isHittingWall = true;
        CreateWallDust(rb.velocity.magnitude);
        // Invert player
        if (!isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x * -1 / 2.5f, rb.velocity.y / 1.25f);
        }

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
        int x_offset;

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

    void CreateTurningDust()
    {
        ParticleSystem.ShapeModule dustShape = turningDust.shape;
        ParticleSystem.VelocityOverLifetimeModule dustVelocity = turningDust.velocityOverLifetime;
        int x_offset;

        if (isFacingRight)
        {
            x_offset = -1;
        }
        else
        {
            x_offset = 1;
        }
        dustShape.position = new Vector3(x_offset * 1.2f, 0f, 0f);
        dustVelocity.xMultiplier = x_offset;

        turningDust.Play();
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
        if (other.gameObject.CompareTag("Wall"))
        {
            isHittingWall = false;
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            isJumping = true;
        }
    }
}
