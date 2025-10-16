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
        if ( PlayerInSight() && cooldownTimer >= attackCooldown) //
        {   
            //attack
            //cooldownTimer = 0;
            animator.SetTrigger("meleeattack");
            //cooldownTimer = 0;
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
        Gizmos.DrawWireCube( boxCollider.bounds.center + transform.right * range , boxCollider.bounds.size );
    }
}
