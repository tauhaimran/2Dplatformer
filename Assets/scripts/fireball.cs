using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D body;

    [Header("Audio")]
    public AudioClip EXPLOSIONsound;
    public AudioSource audioSource;

    private bool moving = false;
    private int x_direction = 1; // 1 = right, -1 = left

    void Awake()
    {
        // Always cache these early and safely
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (body == null)
            Debug.LogError($"[fireball] Rigidbody2D missing on {name}");
        if (animator == null)
            Debug.LogError($"[fireball] Animator missing on {name}");
        
        if (audioSource == null)
        {
            // Auto-add AudioSource so runtime clone can play explosion audio even if prefab missed it
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            Debug.LogWarning($"[fireball] AudioSource was missing on {name}; added automatically.");
        }
    }

    void Update()
    {
        if (moving && body != null)
        {
            body.velocity = new Vector2(x_direction * 10f, body.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[fireball] hit {collision.gameObject.name}");

        if (audioSource && EXPLOSIONsound)
            audioSource.PlayOneShot(EXPLOSIONsound);

        if (animator)
            animator.SetTrigger("explode");

        moving = false;
        body.velocity = Vector2.zero;
    }

    // Called by animation event after explode animation ends
    public void DestroyFireball()
    {
        // Instead of Destroy() for pooling — disable it
        gameObject.SetActive(false);
    }

    public void setDirection(int dir)
    {
        x_direction = dir;
        float xScale = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(dir * xScale, transform.localScale.y, transform.localScale.z);
        moving = true;
    }
}
