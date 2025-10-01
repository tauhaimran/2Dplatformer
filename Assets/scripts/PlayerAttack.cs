using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerMovement playerMovement;
    private float coolDownTimer;
    public float attackCoolDown;
    public Animator animator;
    public GameObject FireBallPrefab;
    public GameObject firePoint;
    

    void Start()
    {
        animator = GetComponent<Animator>();
        coolDownTimer = attackCoolDown; //ready to attack immediately
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && playerMovement.canAttack() && coolDownTimer >= attackCoolDown)
        {
            //attack
            Debug.Log("attack");
            Attack();
        }

        coolDownTimer += Time.deltaTime;
    }

    public void Attack()
    {
        //attack
        Debug.Log("attacking");
        animator.SetTrigger("attack");
        coolDownTimer = 0f;
        /// <summary>
        /// Represents an object in the Unity scene. GameObjects are the fundamental objects in Unity that can represent characters, props, scenery, cameras, waypoints, and more.
        /// </summary>
        if (FireBallPrefab == null)
        {
            Debug.LogError("FireBallPrefab not assigned on PlayerAttack!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("firePoint not assigned on PlayerAttack!");
            return;
        }

        // Instantiate
        GameObject fireballInstantiated = GameObject.Instantiate(FireBallPrefab, firePoint.transform.position, Quaternion.identity);
        //setting the direction of the fireball based on player direction
        fireballInstantiated.GetComponent<fireball>().setDirection(playerMovement.getDirection());
        //fireballInstantiated.setActive(true);
    }
}
