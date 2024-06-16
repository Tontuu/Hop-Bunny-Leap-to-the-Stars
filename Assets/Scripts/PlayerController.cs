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
    public float PLAYER_GRAVITY = 7.0f;

    // Importants
    private bool jump = false;
    public float jumpValue = 0.0f;

    // Misc
    public Vector2 prevVelocity;
    public ParticleSystem dust;
    public ParticleSystem wallDust;
    public ParticleSystem landDust;
    public ParticleSystem turningDust;
    private float oldDirOnJump;
    public float dir;
    public float desacceleration_value = 0.9f;
    [SerializeField]
    private float acceleration = 10f;
    private float deceleration = 5f;
    Vector3 originalCamPos;

    // Conditions
    private bool isOverBush = false;
    public bool isLookingUp = false;
    private bool isCharging = false;
    private bool isCharged = false;
    private bool isGrounded = false;
    private bool isHighLanded = false;
    private bool isLanded = false;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isRising = false;
    private bool isFalling = false;
    private bool isFacingRight = true;
    private bool isGoingRight = false;
    private bool isGoingLeft = false;
    private bool isTurningDirection = false;
    private bool isHittingWall = false;
    private bool isOneShotSound = false;
    private bool oneShotChargingSFX = false;
    public bool alreadyCreatedCam = false;

    // Unity items
    public CinemachineVirtualCamera LookUpCam;
    public CinemachineVirtualCamera vCam;
    public Camera mainCam;
    Rigidbody2D rb;
    Animator animator;
    public AudioSource sfx;

    void Start()
    {
        dir = 0;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = PLAYER_GRAVITY;
        animator = GetComponent<Animator>();
        StartCoroutine(PlayFootstepSound());
        alreadyCreatedCam = false;
    }

    void HandleStates()
    {
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsRising", isRising);
        animator.SetBool("IsCharging", isCharging);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsLookingUp", isLookingUp);
    }

    void Update()
    {
        prevVelocity = rb.velocity;
        // Disable gravity if is not on the air
        if (!isJumping || isGrounded)
        {
            PLAYER_GRAVITY = 0.0f;
        }
        else
        {
            PLAYER_GRAVITY = 7.0f;
        }

        dir = 0;
        // Handle inputs
        if (UI.isPaused) return;

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            isLanded = false;
            isCharging = true;
        }

        if (Input.GetKeyUp(KeyCode.Space) || jumpValue >= MAX_JUMP_MAGNITUDE)
        {
            isCharged = true;
        }

        if (isCharged)
        {
            if (!isGrounded)
            {
                isCharged = false;
            }
        }

        if (isCharging == true && isCharged == true)
        {
            jump = true;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            isHighLanded = false;
            isLanded = false;
            // Don't move if both keys are being pressed.
            if (!(Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)))
            {
                dir = Input.GetAxisRaw("Horizontal");
                if (dir != 0)
                {
                    if (dir == -Math.Sign(rb.velocity.x))
                    {
                        SetFacingDirection(dir);
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
            isHighLanded = false;
            isLanded = false;

            isLookingUp = Input.GetKey(KeyCode.UpArrow);

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                mainCam.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = 20f;
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                alreadyCreatedCam = false;
                Invoke("ResetCameraBlend", 0.5f);
            }
            if (isLookingUp)
            {
                SetLookUpCam();
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
            if (!isCharging && isGrounded && !isJumping)
            {
                SetFacingDirection(dir);
                animator.SetBool("IsTurningDirection", isTurningDirection);
            }
        }

        if (!isCharging && isGrounded && !isJumping)
        {
            animator.SetBool("IsTurningDirection", isTurningDirection);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            isTurningDirection = false;
        }

        // Sound manager
        if (!oneShotChargingSFX)
        {
            if (isCharging) { SoundManager.Instance.PlaySound2D("Charging"); oneShotChargingSFX = true; }
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
        if (jump)
        {
            SoundManager.Instance.PlaySound2D("Jump");
            SetFacingDirection(dir);
            rb.velocity = new Vector2(dir * horizontalJumpSpeed, jumpValue);
            isCharging = false;
            isCharged = false;
            isJumping = true;
            oneShotChargingSFX = false;
            jump = false;
            CreateDust(jumpValue);
            jumpValue = 0f;
        }
        OnRun();
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
            }
        }
    }

    private void SetFacingDirection(float dir)
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

    void ResetCameraBlend()
    {
        mainCam.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = 0.1f;
    }

    private void SetLookUpCam()
    {
        if (!alreadyCreatedCam)
        {
            originalCamPos = CameraManager.ActiveCam.transform.position;
            alreadyCreatedCam = true;
        }
        LookUpCam.transform.position = new Vector3(0, originalCamPos.y + 10f, -2f);
        CameraManager.SwitchCamera(LookUpCam);
    }

    void OnWallHit()
    {
        isHittingWall = true;
        CreateWallDust(rb.velocity.magnitude);
        // Invert player
        if (!isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x * -1 / 1.80f, rb.velocity.y / 1.10f);
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
        if (other.gameObject.CompareTag("Bush"))
        {
            isOverBush = true;
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            OnWallHit();
        }


        if (other.gameObject.CompareTag("Ground"))
        {
            if (isJumping)
            {
                if (prevVelocity.y < -50f)
                {
                    SoundManager.Instance.PlaySound2D("HighLand");
                    isHighLanded = true;
                }
                else
                {
                    SoundManager.Instance.PlaySound2D("Land");
                    isLanded = true;
                }
            }
            CreateLandDust(rb.velocity.magnitude);
            isGrounded = true;
            Physics2D.gravity = new Vector2(0, 0);
            isJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bush"))
        {
            isOverBush = false;
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            isHittingWall = false;
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            Physics2D.gravity = new Vector2(0, -9.81f);
            isGrounded = false;
        }
    }
    private IEnumerator PlayFootstepSound()
    {
        while (true)
        {
            if (isRunning && !isCharging && isGrounded)
            {
                if (isOverBush) SoundManager.Instance.PlaySound2D("Interactive-Run");
                else SoundManager.Instance.PlaySound2D("Run");
            }

            if (jump)
            {
                SoundManager.Instance.PlaySound2D("Jump");
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
}
