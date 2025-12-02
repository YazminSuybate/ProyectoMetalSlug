using UnityEngine;

public class SoldierMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float jumpForce = 400f;

    [Header("Patrol")]
    public float patrolTime = 3f;

    [Header("IA/Melee Attack")]
    public float detectionRange = 7f;
    public float attackRange = 0.5f;
    public float attackRate = 1f;
    public int meleeDamage = 1;

    [Header("Obstacle Check")]
    public Transform wallCheckPoint;
    public float wallCheckDistance = 0.5f;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    [Header("References")]
    public Animator animator;
    public Transform soldierVisuals;

    // --- Variables Privadas ---
    private Rigidbody2D rb;
    private bool isGrounded;
    private int direction = 1;
    private float patrolTimer;
    private float attackTimer;
    private Transform player;
    private bool isAttacking = false;

    private const string IsRunningParam = "IsRunning";
    private const string JumpTrigger = "Jump";
    private const string AttackTrigger = "Attack";
    private const string PlayerTag = "Player";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("El componente Rigidbody2D no se encuentra en el objeto.");
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (soldierVisuals == null)
        {
            soldierVisuals = transform;
        }
    }

    void Start()
    {
        patrolTimer = patrolTime;
        attackTimer = 1f / attackRate;

        GameObject playerObj = GameObject.FindGameObjectWithTag(PlayerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        SoldierHealth health = GetComponent<SoldierHealth>();
        if (health != null && health.isDead) return;

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                AttemptMeleeAttack();
                StopMovement();
                FacePlayer();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                isAttacking = false;
                ChasePlayer();
            }
            else
            {
                isAttacking = false;
                Patrol();
            }
        }
        else
        {
            Patrol();
        }

        ApplyMovementAndFlip();

        bool hitWall = CheckForWall();
        if (isGrounded && hitWall && !isAttacking)
        {
            Jump();
        }

        bool isMovingHorizontally = Mathf.Abs(rb.velocity.x) > 0.1f;

        bool isRunning = isMovingHorizontally && !isAttacking;

        if (animator != null)
        {
            animator.SetBool(IsRunningParam, isRunning);
        }
    }

    void AttemptMeleeAttack()
    {
        isAttacking = true;
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            MeleeAttack();
            attackTimer = 1f / attackRate;
        }
    }

    void MeleeAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger(AttackTrigger);
        }

        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, whatIsPlayer);

        if (hitPlayer != null && hitPlayer.CompareTag(PlayerTag))
        {
            Debug.Log("¡Cuchillazo! El soldado intentó dañar al jugador.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void ChasePlayer()
    {
        direction = (player.position.x > transform.position.x) ? 1 : -1;
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            soldierVisuals.localScale = new Vector3(-1, 1, 1);
        }
        else if (player.position.x < transform.position.x)
        {
            soldierVisuals.localScale = new Vector3(1, 1, 1);
        }
        direction = (player.position.x > transform.position.x) ? 1 : -1;
    }

    void StopMovement()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0)
        {
            direction *= -1;
            patrolTimer = patrolTime;
        }
    }

    void ApplyMovementAndFlip()
    {
        if (!isAttacking)
        {
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
        }

        if (!isAttacking)
        {
            if (direction > 0)
            {
                soldierVisuals.localScale = new Vector3(-1, 1, 1);
            }
            else if (direction < 0)
            {
                soldierVisuals.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    bool CheckForWall()
    {
        if (wallCheckPoint == null) return false;

        Vector2 rayDirection = new Vector2(direction, 0);
        Debug.DrawRay(wallCheckPoint.position, rayDirection * wallCheckDistance, Color.yellow);
        RaycastHit2D hit = Physics2D.Raycast(wallCheckPoint.position, rayDirection, wallCheckDistance, whatIsGround);

        return hit.collider != null;
    }

    public void Jump()
    {
        if (rb == null) return;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(0f, jumpForce));

        if (animator != null)
        {
            animator.SetTrigger(JumpTrigger);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Suelo"))
        {
            if (col.GetContact(0).normal.y > 0.5f)
            {
                isGrounded = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Suelo"))
        {
            isGrounded = false;
        }
    }
}