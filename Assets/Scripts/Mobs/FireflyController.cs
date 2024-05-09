using UnityEngine;

public class FireflyController : MonoBehaviour
{

    // Importants
    [SerializeField]
    private float delayToStart;
    private float fireflyVelocity;
    private float gravityScale;
    public Vector2 movementDirection;

    // Conditions
    private bool isGrounded = true;

    // Animation States
    private string currentState;
    const string FF_FLYING = "flying";
    const string FF_WALKING = "walking";

    // Unity items
    Rigidbody2D rb;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        currentState = FF_FLYING;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        delayToStart = Random.Range(1.0f, 25.0f);
        GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (delayToStart >= 0f)
        {
            delayToStart -= Time.deltaTime;
        } else 
        {
            GetComponent<Renderer>().enabled = true;
            if (IsOnWall(Vector2.left))
            {
                fireflyVelocity = 0;
                Destroy(gameObject, 2);
            }
            
            if (IsOnWall(Vector2.down))
            {
                isGrounded = true;
                fireflyVelocity = -1f;
                gravityScale = 0f;
            } else 
            {
                isGrounded = false;
                fireflyVelocity = -1.2f;
                gravityScale = 2.5f;
            }

            movementDirection = new Vector2(fireflyVelocity, -gravityScale);

            // Handle animation states
            HandleStates();
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = movementDirection;
    }
    void HandleStates()
    {
        if (isGrounded)
        {
            ChangeAnimationState(FF_WALKING);
        }
        else
        {
            ChangeAnimationState(FF_FLYING);
        }
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        animator.Play(newState);
        currentState = newState;
    }

    bool IsOnWall(Vector2 direction)
    {
        Vector2 position = transform.position;

        float distance = 0.25f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, LayerMask.GetMask("Platform"));
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }
}

