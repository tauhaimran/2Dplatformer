using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    private Rigidbody2D body;
    bool moving = false;
    private int x_direction = 1; //1 is right, -1 is left
    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        //this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {

             body.velocity = new Vector2(x_direction * 10f, body.velocity.y);
            //if (x_direction == 1)
                //body.velocity = new Vector2(10f, body.velocity.y);
                //body.addForce2D(new Vector2(10f, 0f));
                //transform.Translate(Vector2.right * 10f * Time.deltaTime);
                //transform.Translate(Vector2.right * 10f * Time.deltaTime);
           // else if (x_direction == -1)
                //body.addForce2D(new Vector2(-10f, 0f));
                //body.velocity = new Vector2(-10f, body.velocity.y);
               // transform.Translate(Vector2.left * 10f * Time.deltaTime);
        }


    }

    //on any collision set trigger to explode
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("fireball hit " + collision.gameObject.name);
        animator.SetTrigger("explode");
        moving = false;
        //Destroy(gameObject);
    }
    public void DestroyFireball()
    {
        Destroy(this.gameObject);
    }
    
    public void setDirection(int dir)
    {
        x_direction = dir;
        float xScale = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(dir * xScale, transform.localScale.y, transform.localScale.z);
        moving = true;
    }
}
