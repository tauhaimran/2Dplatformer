using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerMovement playerMovement;
    private float coolDownTimer;
    public float attackCoolDown;
    public Animator animator;
    public GameObject FireBallPrefab;
    public GameObject firePoint;
    public AudioClip fireballSound;
    public AudioSource audioSource;

    // NEW FEATURE — Object Pool
    private List<GameObject> fireballPool = new List<GameObject>();
    public int poolSize = 5;

    void Start()
    {
        animator = GetComponent<Animator>();
        coolDownTimer = attackCoolDown;
        CreateFireballPool(); // NEW FEATURE
    }

    void Update()
    {
        coolDownTimer += Time.deltaTime;

        // Normal Attack (ground)
        if (Input.GetKeyDown(KeyCode.Z) && playerMovement.canAttack() && coolDownTimer >= attackCoolDown)
        {
            Attack();
        }

        // Jump Attack (only while in air) — NEW FEATURE
        if (Input.GetKeyDown(KeyCode.F) && !playerMovement.isGrounded() && coolDownTimer >= attackCoolDown)
        {
            JumpAttack();
        }
    }

    void CreateFireballPool() // NEW FEATURE
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject fb = Instantiate(FireBallPrefab);
            fb.SetActive(false);
            fireballPool.Add(fb);
        }
    }

    GameObject GetFireballFromPool() // object pooling method
    {
        // remove any destroyed (null) entries and return first inactive instance
        for (int i = fireballPool.Count - 1; i >= 0; i--)
        {
            GameObject fb = fireballPool[i];
            if (fb == null)
            {
                fireballPool.RemoveAt(i);
                continue;
            }
            if (!fb.activeInHierarchy)
                return fb;
        }

        // optional: expand pool if all active
        if (FireBallPrefab == null)
        {
            Debug.LogError("FireBallPrefab is not assigned on PlayerAttack.");
            return null;
        }

        GameObject newFb = Instantiate(FireBallPrefab);
        newFb.SetActive(false);
        fireballPool.Add(newFb);
        return newFb;
    }

    public void Attack()
    {
        animator.SetTrigger("attack");
        coolDownTimer = 0f;
        ShootFireball();
    }

    public void JumpAttack() // NEW FEATURE
    {
        animator.SetTrigger("attack"); // same animation or use "jumpAttack"
        coolDownTimer = 0f;
        ShootFireball();
    }

    void ShootFireball() // NEW FEATURE
    {
        // Play sound if AudioSource and clip assigned
        if (audioSource != null && fireballSound != null)
            audioSource.PlayOneShot(fireballSound);
        else if (audioSource == null)
            Debug.LogWarning("PlayerAttack: audioSource is null — skipping fire sound.");
        else
            Debug.LogWarning("PlayerAttack: fireballSound clip is not assigned.");
        
        //the rest of the stuff....
        GameObject fb = GetFireballFromPool();
        if (fb == null)
            return;
        
        if (firePoint == null)
        {
            Debug.LogError("firePoint Transform is not assigned on PlayerAttack.");
            return;
        }
        
        fb.transform.position = firePoint.transform.position;
        fb.transform.rotation = Quaternion.identity;
        fb.SetActive(true);
        
        var fbScript = fb.GetComponent<fireball>();
        if (fbScript != null)
            fbScript.setDirection(playerMovement.getDirection());
        else
            Debug.LogWarning("Instantiated fireball has no 'fireball' script attached.");
    }
}