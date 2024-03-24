using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float runSpeed = 12.0f;
    [SerializeField]
    private bool _isRunning = false;
    public bool IsRunning
    {
        get { return _isRunning; }
        private set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    [SerializeField]
    private bool _isGrounded = false;
    public bool IsGrounded
    {
        get { return _isGrounded; }
        private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                // Flip the local scale to make the player face the opposite direction
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    private bool canJump = true;
    [SerializeField]
    private float jumpValue = 0.0f;
    [SerializeField]
    private float jumpImpulse = 20.0f;
    Rigidbody2D rb;
    Animator animator;
    Vector2 moveInput;
    float directionInput;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        directionInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey("space") && IsGrounded && canJump)
        {
            jumpValue += 0.2f;
        }


        if (Input.GetKeyDown("space") && IsGrounded && canJump)
        {
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }

        if (jumpValue >= 27f && IsGrounded)
        {
            float tempx = directionInput * runSpeed;
            float tempy = jumpValue;
            rb.velocity = new Vector2(tempx, tempy);
            Invoke("ResetJump", 0.1f);
        }

        if (Input.GetKeyUp("space"))
        {
            if (IsGrounded)
            {
                rb.velocity = new Vector2(directionInput * runSpeed, jumpValue);
                jumpValue = 0.0f;
            }
            canJump = true;
        }
    }

    private void FixedUpdate()
    {
        if (jumpValue == 0.0f && IsGrounded)
        {
            rb.velocity = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
        }
    }

    void ResetJump()
    {
        canJump = false;
        jumpValue = 0f;
    }

    public void OnRun(InputAction.CallbackContext context)
    {

        if (IsGrounded)
        {
            moveInput = context.ReadValue<Vector2>();
            IsRunning = moveInput != Vector2.zero;
            setFacingDirection(moveInput);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // if (context.started && IsGrounded)
        // {
        //     animator.SetTrigger(AnimationStrings.jump);
        //     rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        // }
    }

    private void setFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
        }
    }
}
