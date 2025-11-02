using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{

    [SerializeField] private int health = 3;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    public float range = 1f;
    public int walkRange = 5;
    public float moveSpeed = 2f;

    private bool LefttoRight = true;

    private float cooldownTimer = Mathf.Infinity;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;

        //Attack if cool satisfied AND player is in sight
        if (PlayerInSight() && cooldownTimer >= attackCooldown) //
        {
            //attack
            //cooldownTimer = 0;
            animator.SetBool("moving", false);
            animator.SetTrigger("meleeattack");
            //cooldownTimer = 0;
        }
        else
        {animator.SetBool("moving", true);
            //move left and right within walkRange
            if (LefttoRight)
            {
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
                if (transform.position.x >= walkRange)
                {
                    LefttoRight = false;
                    Flip();
                }
            }
            else
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
                if (transform.position.x <= -walkRange)
                {
                    LefttoRight = true;
                    Flip();
                }
            }
        }

        
    }

    private bool PlayerInSight()
    {
        //check if player in sight
        RaycastHit2D hit = Physics2D.BoxCast( boxCollider.bounds.center +transform.right *range * transform.localScale.x, boxCollider.bounds.size, 0, Vector2.left, 0, playerLayer );
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range, boxCollider.bounds.size);
    }
    
    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
