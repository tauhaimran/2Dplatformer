using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float crouchSpeedMultiplier = 0.5f; // NEW FEATURE
    private Rigidbody2D body;
    public Animator animator;
    private BoxCollider2D boxCollider;
    public LayerMask groundLayer;
    public LayerMask wallLayer; // add this near your other public LayerMask fields
    public GameObject CheckPoint;
    public AudioClip jumpSound;
    public AudioSource audioSource;


    private int x_direction = 1; //1 is right, -1 is left

    [Header("Dizzy & Fall")]
    public float dizzyFallThreshold = 6f; 
    public float dizzyDuration = 1.5f; 
    private bool isDizzy = false;
    private float lastGroundY; 
    private bool wasGrounded; 

    [Header("Wall Jump")]
    public float wallJumpForceX = 8f; 
    public float wallJumpForceY = 12f; 
    public float wallCheckDistance = 0.4f; 
    private bool isCrouching = false; 

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator.SetBool("jump", false);

        if (body != null)
        {
            body.freezeRotation = true;
            body.angularVelocity = 0f;
        }

        lastGroundY = transform.position.y;
    }

    void Update()
    {
        if (isDizzy) return; // — disable input when dizzy

        if (body != null && body.angularVelocity != 0f)
            body.angularVelocity = 0f;

        // -------- CROUCH SYSTEM (hold C key) --------
        if (Input.GetKey(KeyCode.C))
            isCrouching = true;
        else
            isCrouching = false;

        animator.SetBool("crouch", isCrouching);

        // Move 2D player
        float moveInput = Input.GetAxis("Horizontal");
        float currentSpeed = isCrouching ? speed * crouchSpeedMultiplier : speed;
        body.velocity = new Vector2(moveInput * currentSpeed, body.velocity.y);

        // -------- Jump --------
        if (Mathf.Abs(body.velocity.y) < 0.001f)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                //playing the jump sound
                audioSource.PlayOneShot(jumpSound);
                
                //actual jump
                body.velocity = new Vector2(body.velocity.x, speed);
                animator.SetBool("jump", true);

            }
        }
        if (Mathf.Abs(body.velocity.y) < 0.001f)
        {
            animator.SetBool("jump", false);
        }

        // -------- Wall Jump (press jump near wall) --------
        if (!isGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            if (IsTouchingWall())
            {
                float dir = transform.localScale.x > 0 ? -1 : 1;
                body.velocity = new Vector2(dir * wallJumpForceX, wallJumpForceY);
            }
        }

        // -------- Quick Fall --------
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            body.velocity = new Vector2(body.velocity.x, -speed);
        }

        // -------- Flip character --------
        if (moveInput > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
            x_direction = 1;
        }
        else if (moveInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            x_direction = -1;
        }

        // -------- Walk animation --------
        if (Mathf.Abs(body.velocity.x) > 0.01f && isGrounded())
            animator.SetBool("walk", true);
        else
            animator.SetBool("walk", false);

        // -------- Fly Kick -------- ( when shift pressed anywhere)
        //if key pressec once
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            animator.SetTrigger("flykick");
            //animator.SetBool("walk", false);
            //animator.SetBool("jump", false);
        }

        // -------- Ground Check for Dizzy Fall --------
        bool grounded = isGrounded();
        if (!wasGrounded && grounded)
        {
            float fallDistance = lastGroundY - transform.position.y;
            if (fallDistance > dizzyFallThreshold)
                StartCoroutine(DoDizzy()); // changed to start coroutine so dizzyDuration is honored
        }

        if (grounded)
            lastGroundY = transform.position.y;

        wasGrounded = grounded;
    }

    // ------------------ HELPER METHODS ------------------

    public bool isGrounded()
    {
        if (boxCollider == null)
            return false;

        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 
                                             0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    private bool IsTouchingWall() // NEW: side-based check using wallLayer
    {
        if (boxCollider == null)
            return false;

        // origin at the side edge of the player's collider
        float facing = Mathf.Sign(transform.localScale.x);
        Vector2 origin = (Vector2)boxCollider.bounds.center + Vector2.right * (boxCollider.bounds.extents.x + 0.02f) * facing;
        Vector2 dir = Vector2.right * facing;

        // debug draw
        Debug.DrawRay(origin, dir * wallCheckDistance, Color.red, 0.1f);

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, wallCheckDistance, wallLayer);
        return hit.collider != null;
    }

    private IEnumerator DoDizzy() //  coroutine to actually wait for dizzyDuration
    {
        if (isDizzy) yield break; // already dizzy, don't stack

        isDizzy = true;
        animator.SetTrigger("dizzy");
        if (body != null) body.velocity = Vector2.zero;

        yield return new WaitForSeconds(dizzyDuration);

        isDizzy = false;
    }

    public bool canAttack()
    {
        return (Input.GetAxis("Horizontal") == 0) && isGrounded();
    }

    public int getDirection()
    {
        return x_direction;
    }

    public void death()
    {
        animator.SetTrigger("death");
        this.enabled = false;
        body.velocity = new Vector2(0, 0);
    }

    public void respawn()
    {
        this.enabled = true;
        transform.position = CheckPoint.transform.position;
        animator.SetTrigger("reset");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "trap")
        {
            Debug.Log("Player hit trap");
            animator.SetBool("walk", false);
            animator.SetBool("jump", false);
            death();
        }
    }
}
