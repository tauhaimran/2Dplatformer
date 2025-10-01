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
        //coolDownTimer = 0f;
        
        GameObject fireball = Instantiate(FireBallPrefab, firePoint.transform.position, Quaternion.identity);
    }
}
