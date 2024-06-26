using UnityEngine.InputSystem;
using UnityEngine;
using System;
using Cinemachine;
using System.Collections;
using UnityEditor.Rendering.LookDev;
using System.Threading;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    // Importants
    private Vector2 moveDir;
    static public bool lookupCamActivity;
    public float player_gravity = 7.0f;

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
    public bool isCharging = false;
    public bool isJumping = false;
    public bool isGrounded = false;
    private bool isRunning = false;
    private bool isRising = false;
    private bool isFalling = false;
    private bool isFacingRight = true;
    private bool isGoingRight = false;
    private bool isGoingLeft = false;
    private bool isTurningDirection = false;
    private bool isHittingWall = false;
    public bool alreadyCreatedCam = false;

    // Unity items
    public ChargeHandler chargeHandler;
    private InputActionReference inputReference;
    public CinemachineVirtualCamera LookUpCam;
    public CinemachineVirtualCamera vCam;
    public Camera mainCam;
    public Rigidbody2D rb;
    Animator animator;
    public AudioSource sfx;

    void Start()
    {
        dir = 0;
        chargeHandler = GetComponent<ChargeHandler>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = player_gravity;
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
            player_gravity = 0.0f;
        }
        else
        {
            player_gravity = 7.0f;
        }

        dir = 0;
        // Handle inputs
        if (UI.isPaused) return;

        dir = moveDir.x;

        isGoingRight = dir > 0;
        isGoingLeft = dir < 0;
        if (isJumping) { oldDirOnJump = dir; }
        if (dir != 0)
            isRunning = true;
        else
        {
            isGoingRight = false;
            isGoingLeft = false;
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
            isLookingUp = Input.GetKey(KeyCode.UpArrow);

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                lookupCamActivity = true;
                mainCam.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = 20f;
            }
            else
            {
                lookupCamActivity = true;
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

        // Handle animation states
        HandleStates();
    }
    void FixedUpdate()
    {
        OnRun();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveDir = ctx.ReadValue<Vector2>();
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
                Vector2 targetVelocity = new Vector2(dir * Constants.RUN_SPEED, rb.velocity.y);

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

    public void SetFacingDirection(float dir)
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

    public void CreateDust(float magnitude)
    {
        int speed = Mathf.RoundToInt(Utils.Map(magnitude, Constants.MIN_JUMP_MAGNITUDE, Constants.MAX_JUMP_MAGNITUDE, 2f, 6f));
        int emissionAmount = Mathf.RoundToInt(Utils.Map(magnitude, Constants.MIN_JUMP_MAGNITUDE, Constants.MAX_JUMP_MAGNITUDE, 20f, 120f));
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
                }
                else
                {
                    SoundManager.Instance.PlaySound2D("Land");
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

            yield return new WaitForSeconds(0.3f);
        }
    }
}
