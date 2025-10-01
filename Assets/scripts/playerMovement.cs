using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 5f;
    private Rigidbody2D body;
    public Animator animator;
    private BoxCollider2D boxCollider;
    public LayerMask groundLayer;


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator.SetBool("jump", false);
        // Prevent physics from rotating the player sprite when hitting things or moving fast
        if (body != null)
        {
            body.freezeRotation = true; // Unity convenience to lock rotation
            body.angularVelocity = 0f; // ensure no residual spin
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ensure angular velocity remains zero in case other forces try to rotate the body
        if (body != null && body.angularVelocity != 0f)
            body.angularVelocity = 0f;

        //move 2d player
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

        //move up - jump
        //only if on ground
        if (Mathf.Abs(body.velocity.y) < 0.001f)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                //only if on ground
                //if (Mathf.Abs(body.velocity.y) < 0.001f)
                //{
                body.velocity = new Vector2(body.velocity.x, speed);
                animator.SetBool("jump", true);
            }

        }
        if (Mathf.Abs(body.velocity.y) < 0.001f)
        {
            animator.SetBool("jump", false);
        }

        ////quick fall
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            body.velocity = new Vector2(body.velocity.x, -speed);
        }

        float HorizontalInput = Input.GetAxis("Horizontal");
        //making it turn left and right
        if (HorizontalInput > 0.01f)//body.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else //if (body.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Mathf.Abs(body.velocity.x) > 0.01f && isGrounded())
        {
            animator.SetBool("walk", true);
        }
        else
        {
            animator.SetBool("walk", false);
        }
    }

    public bool isGrounded()
    {
        if (boxCollider == null)
            return false;

        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    public bool canAttack()
    {
        return (Input.GetAxis("Horizontal") == 0) && isGrounded();
    }
}
