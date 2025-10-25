using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  //  for Button
using TMPro; // For TMP text (you’ll connect it later)

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float crouchSpeedMultiplier = 0.5f;
    private Rigidbody2D body;
    public Animator animator;
    private BoxCollider2D boxCollider;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public GameObject CheckPoint;
    public AudioClip jumpSound;
    public AudioSource audioSource;

    private int x_direction = 1;

    [Header("Dizzy & Fall")]
    public float dizzyFallThreshold = 6f;
    public float dizzyDuration = 1.5f;
    private bool isDizzy = false;
    private float lastGroundY;
    private bool wasGrounded;

    [Header("Wall Jump")]
    public float wallJumpForceX = 8f;
    public float wallJumpForceY = 12f;
    public float wallCheckDistance = 0.4f;
    private bool isCrouching = false;

    // ----------------------------------------
    [Header("Player Stats")]
    public int maxLives = 3;
    private int currentLives;
    public int coinsCollected = 0;

    [Header("UI References (Assign in Inspector)")]
    public TMP_Text livesText;
    public TMP_Text coinsText;

    public TMP_Text gameOverText;

    public TMP_Text levelCompleteText;

    public Button restartButton;
    public TMP_Text restartButtonText;

    public Button quitButton;
    public TMP_Text quitButtonText;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator.SetBool("jump", false);

        if (body != null)
        {
            body.freezeRotation = true;
            body.angularVelocity = 0f;
        }

        lastGroundY = transform.position.y;

        // Initialize lives
        currentLives = maxLives;
        UpdateUI();
    }

    void Update()
    {
        if (isDizzy) return;
        if ( currentLives <= 0 ) return;

        if (body != null && body.angularVelocity != 0f)
            body.angularVelocity = 0f;

        // -------- CROUCH --------
        isCrouching = Input.GetKey(KeyCode.C);
        animator.SetBool("crouch", isCrouching);

        float moveInput = Input.GetAxis("Horizontal");
        float currentSpeed = isCrouching ? speed * crouchSpeedMultiplier : speed;
        body.velocity = new Vector2(moveInput * currentSpeed, body.velocity.y);

        // -------- Jump --------
        if (Mathf.Abs(body.velocity.y) < 0.001f)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                audioSource.PlayOneShot(jumpSound);
                body.velocity = new Vector2(body.velocity.x, speed);
                animator.SetBool("jump", true);
            }
        }

        if (Mathf.Abs(body.velocity.y) < 0.001f)
            animator.SetBool("jump", false);

        // -------- Wall Jump --------
        if (!isGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            if (IsTouchingWall())
            {
                float dir = transform.localScale.x > 0 ? -1 : 1;
                body.velocity = new Vector2(dir * wallJumpForceX, wallJumpForceY);
            }
        }

        // -------- Quick Fall --------
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            body.velocity = new Vector2(body.velocity.x, -speed);
        }

        // -------- Flip --------
        if (moveInput > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
            x_direction = 1;
        }
        else if (moveInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            x_direction = -1;
        }

        // -------- Walk animation --------
        animator.SetBool("walk", Mathf.Abs(body.velocity.x) > 0.01f && isGrounded());

        // -------- Fly Kick --------
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            animator.SetTrigger("flykick");

        // -------- Dizzy Check --------
        bool grounded = isGrounded();
        if (!wasGrounded && grounded)
        {
            float fallDistance = lastGroundY - transform.position.y;
            if (fallDistance > dizzyFallThreshold)
                StartCoroutine(DoDizzy());
        }

        if (grounded)
            lastGroundY = transform.position.y;

        wasGrounded = grounded;
    }

    // ------------------ HELPER METHODS ------------------

    public bool isGrounded()
    {
        if (boxCollider == null) return false;
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size,
                                             0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    private bool IsTouchingWall()
    {
        if (boxCollider == null) return false;
        float facing = Mathf.Sign(transform.localScale.x);
        Vector2 origin = (Vector2)boxCollider.bounds.center + Vector2.right * (boxCollider.bounds.extents.x + 0.02f) * facing;
        Vector2 dir = Vector2.right * facing;
        Debug.DrawRay(origin, dir * wallCheckDistance, Color.red, 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, wallCheckDistance, wallLayer);
        return hit.collider != null;
    }

    private IEnumerator DoDizzy()
    {
        if (isDizzy) yield break;
        isDizzy = true;
        animator.SetTrigger("dizzy");
        body.velocity = Vector2.zero;
        yield return new WaitForSeconds(dizzyDuration);
        isDizzy = false;
    }

    public bool canAttack() => (Input.GetAxis("Horizontal") == 0) && isGrounded();
    public int getDirection() => x_direction;

    public void death()
    {
        animator.SetTrigger("death");
        body.velocity = Vector2.zero;
        currentLives--;

        UpdateUI();

        if (currentLives > 0)
            Invoke(nameof(respawn), 1.2f);
        else
            StartCoroutine(GameOver());
    }

    public void respawn()
    {
        transform.position = CheckPoint.transform.position;
        animator.SetTrigger("reset");
    }

    private IEnumerator GameOver()
    {
        Debug.Log("Game Over - no lives left!");
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        restartButtonText.gameObject.SetActive(true);
        quitButtonText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        // TODO: Add your GameOver UI or restart logic here
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("trap"))
        {
            Debug.Log("Player hit trap");
            animator.SetBool("walk", false);
            animator.SetBool("jump", false);
            death();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("coin"))
        {
            coinsCollected++;
            UpdateUI();
            Destroy(other.gameObject);
        }

        //if (other.CompareTag("exit"))
       // {
            Debug.Log("Reached exit — level complete (handle scene transition here)");
            // Leave functionality to you — e.g.:
            // SceneManager.LoadScene("MainMenu");
        //}
    }

    private void UpdateUI()
    {
        if (livesText != null)
            livesText.text = "Lives: " + currentLives;

        if (coinsText != null)
            coinsText.text = "Coins: " + coinsCollected;
    }
}
