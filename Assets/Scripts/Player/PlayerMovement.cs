using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float jumpForce = 500f;

    public CameraFollow cameraFollow;

    public float moveSpeed = 5f;

    public GameObject bulletPrefab;

    public Transform firePoint;

    public Animator torsoAnim;
    public Animator legsAnim;

    private Rigidbody2D rb;
    private int groundContactCount = 0;
    private bool IsGrounded => groundContactCount > 0;

    private bool canMove = false;

    public Transform playerVisuals;

    public GameTimer gameTimer;

    private bool hasMoved = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("El componente Rigidbody2D no se encuentra en el objeto principal.");
        }
        if (torsoAnim == null || legsAnim == null)
        {
            Debug.LogError("Arrastra los componentes Animator de Torso y Piernas al Inspector.");
        }

        if (cameraFollow == null)
        {
            cameraFollow = FindObjectOfType<CameraFollow>();
        }
        if (gameTimer == null)
        {
            gameTimer = FindObjectOfType<GameTimer>();
        }
    }

    public void SetMovementEnabled(bool state)
    {
        canMove = state;
        if (!canMove)
        {
            if (rb != null)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            if (legsAnim != null) legsAnim.SetBool("IsRunning", false);
            if (torsoAnim != null) torsoAnim.SetBool("IsRunning", false);
        }
    }

    void Update()
    {
        if (!canMove)
        {
            if (rb.velocity.x != 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            return;
        }


        float moveInput = Input.GetAxisRaw("Horizontal");

        if (!hasMoved && Mathf.Abs(moveInput) > 0)
        {
            hasMoved = true;
            if (gameTimer != null)
            {
                gameTimer.StartTimer();
            }
        }

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);


        if (cameraFollow != null)
        {
            float minPlayerX = cameraFollow.GetMinPlayerX();
            Vector3 currentPosition = transform.position;

            if (currentPosition.x < minPlayerX)
            {
                currentPosition.x = minPlayerX;
                transform.position = currentPosition;

                if (rb.velocity.x < 0)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Shoot();
        }

        if (moveInput > 0)
        {
            playerVisuals.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            playerVisuals.localScale = new Vector3(-1, 1, 1);
        }

        bool isRunning = Mathf.Abs(moveInput) > 0;

        legsAnim.SetBool("IsRunning", isRunning);
        torsoAnim.SetBool("IsRunning", isRunning);

        bool isAirborne = !IsGrounded;
        bool isMovingVertically = Mathf.Abs(rb.velocity.y) > 5f;

        bool isJumpingAnimation = isAirborne && isMovingVertically;

        legsAnim.SetBool("IsGrounded", IsGrounded);
        torsoAnim.SetBool("IsGrounded", IsGrounded);

        torsoAnim.SetBool("IsJumping", isJumpingAnimation);
        legsAnim.SetBool("IsJumping", isJumpingAnimation);

        if (IsGrounded && Input.GetKeyDown(KeyCode.Z))
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(0f, jumpForce));
    }

    void Shoot()
    {
        torsoAnim.SetTrigger("Shoot");

        if (bulletPrefab != null && firePoint != null)
        {
            float directionX = playerVisuals.localScale.x > 0 ? 1f : -1f;

            Vector3 spawnPosition = firePoint.position;

            spawnPosition.x += 0.5f * directionX;

            GameObject bulletObject = Instantiate(bulletPrefab, spawnPosition, firePoint.rotation);

            Bullet bulletScript = bulletObject.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                bulletScript.Initialize(directionX);
            }
        }
        else
        {
            Debug.LogError("Faltan referencias de 'bulletPrefab' o 'firePoint'.");
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Suelo"))
        {
            if (col.GetContact(0).normal.y > 0.5f)
            {
                groundContactCount++;
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Suelo"))
        {
            if (groundContactCount > 0)
            {
                groundContactCount--;
            }
        }
    }
}