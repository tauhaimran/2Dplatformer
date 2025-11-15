using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  //  for Button
using TMPro; // For TMP text (you’ll connect it later)
using UnityEngine.SceneManagement;

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
    public GameObject StartPoint;

    public GameObject exitDoor;
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

    private string GetNextLevelName()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "level 1") return "level 2";
        if (currentScene == "level 2") return "level 3";
        if (currentScene == "level 3") return "level 4";
        if (currentScene == "level 4") return "level 5";
        if (currentScene == "level 5") return "MainMenu";
        
        return "MainMenu";
    }

    private int GetCurrentLevelNumber()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "level 1") return 1;
        if (currentScene == "level 2") return 2;
        if (currentScene == "level 3") return 3;
        if (currentScene == "level 4") return 4;
        if (currentScene == "level 5") return 5;
        
        return 0;
    }


        void Update()
    {
        if (isDizzy) return;
        if (currentLives <= 0) return;


        //DEV SECRETS TO TEST OUT GAMEPLAY
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }

        //if F5 reset all progress
        if (Input.GetKeyDown(KeyCode.F5))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs reset - all progress cleared.");
            SceneManager.LoadScene("MainMenu");
        }



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
        //set all movement to zero
        this.enabled = false;
        body.velocity = Vector2.zero;
        currentLives--;
        

        UpdateUI();
        //currentLives--;
        if (currentLives > 0)
            Invoke(nameof(respawn), 1.2f);
        else
            StartCoroutine(GameOver());
    }

    public void respawn()
    { 
        this.enabled = true;
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
            
            // Add coins to playerManager for persistence
            if (playerManager.Instance != null)
            {
                playerManager.Instance.AddCoins(1);
            }
            
            UpdateUI();
            Destroy(other.gameObject);

            if (coinsCollected >= 30)
            {
                levelCompleteText.gameObject.SetActive(true);
                exitDoor.SetActive(true);
            }
        }

        if( other.CompareTag("checkpoint"))
        {
            CheckPoint.transform.position = other.transform.position;
            //CheckPoint.transform.position = StartPoint.transform.position;
            Debug.Log("Checkpoint reached!");
        }

        if (other.CompareTag("exit"))
        {
            Debug.Log("Level complete!");

            // Save coins and unlock next level
            if (playerManager.Instance != null)
            {
                int currentLevel = GetCurrentLevelNumber();

                // Unlock next level if current level is completed
                if (currentLevel > 0 && playerManager.Instance.unlockedLevels == currentLevel)
                {
                    playerManager.Instance.UnlockNextLevel();
                    Debug.Log("Unlocked level " + (currentLevel + 1));
                }
            }
            
            // Load next level
            //string nextLevel = GetNextLevelName();
            //loading the main menu now
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void UpdateUI()
    {
        if (livesText != null)
            livesText.text = "Lives: " + currentLives;

        if (coinsText != null)
            coinsText.text = "Coins: " + coinsCollected;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
