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

    // Importants
    private float horizontalValue;
    private float jumpValue = 0.0f;
    Vector2 moveInput;

    // Conditions
    private bool canJump = true;
    private bool isCharging = false;
    private bool isGrounded = false;
    private bool isRunning = false;
    private bool isJumping = false;
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

    // Update is called once per frame
    void Update()
    {
        // Get run input
        horizontalValue = Input.GetAxisRaw("Horizontal");
        isRunning = horizontalValue != 0;
    }
    void FixedUpdate()
    {
        if (isRunning)
        {
            rb.velocity = new Vector2(horizontalValue * runSpeed, rb.velocity.y);
            setFacingDirection(horizontalValue);
            ChangeAnimationState(PLAYER_RUN);
        }
        else
        {
            ChangeAnimationState(PLAYER_IDLE);
        }
    }


    void ResetJump()
    {
        isJumping = false;
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
