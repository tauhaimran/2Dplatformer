using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    bool moving = true;
    private int x_direction = 1; //1 is right, -1 is left
    void Start()
    {
        animator = GetComponent<Animator>();
        //this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            if (x_direction == 1)
                transform.Translate(Vector2.right * 10f * Time.deltaTime);
            else
                transform.Translate(Vector2.left * 10f * Time.deltaTime);
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
        if (dir == -1)
        {
            transform.localScale = new Vector3(-1, 0.6f, 0.6f);
        }
    }
}
