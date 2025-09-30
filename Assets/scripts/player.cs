using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    
    public float speed = 5f;
    private Rigidbody2D body;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();        
        animator = GetComponent<Animator>();
        animator.SetBool("jump", false);
    }

    // Update is called once per frame
    void Update()
    {
        //move 2d player
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

        //move up - jump
        //only if on ground
        if (Mathf.Abs(body.velocity.y) < 0.001f)
            {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) )
            {
            //only if on ground
            //if (Mathf.Abs(body.velocity.y) < 0.001f)
            //{
                body.velocity = new Vector2(body.velocity.x, speed);
                animator.SetBool("jump", true);
            }
            
        }
        if (Mathf.Abs(body.velocity.y) < 0.001f){
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

        if (Mathf.Abs(body.velocity.x) > 0.01f)
        {
            animator.SetBool("walk", true);
        }
        else
        {
            animator.SetBool("walk", false);
        }
    }
}
