using UnityEngine;

public class SoldierMovement : MonoBehaviour
{
    // --- Configuración Pública ---
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float jumpForce = 400f;

    [Header("Patrol")]
    public float patrolTime = 3f; 

    [Header("IA/Melee Attack")]
    public float detectionRange = 7f; 
    // Rango de ataque CORTÍSIMO para el cuchillo (ajustar en Unity)
    public float attackRange = 0.5f; 
    // Tasa de ataque: ataques por segundo
    public float attackRate = 1f; 
    // Daño que hace el ataque melee
    public int meleeDamage = 1; 
    // ¡ELIMINADO! public GameObject bulletPrefab; 

    [Header("Obstacle Check")]
    public Transform wallCheckPoint; 
    public float wallCheckDistance = 0.5f; 
    public LayerMask whatIsGround;
    // Capa del Jugador para el OverlapCircle (DEBE ASIGNARSE en Unity)
    public LayerMask whatIsPlayer; 

    [Header("References")]
    public Animator animator; 
    public Transform soldierVisuals; 

    // --- Variables Privadas ---
    private Rigidbody2D rb;
    private bool isGrounded;
    private int direction = 1; 
    private float patrolTimer;
    private float attackTimer; // Temporizador para controlar el ataque
    private Transform player; 
    private bool isAttacking = false; 
    
    // CONSTANTES
    private const string IsRunningParam = "IsRunning"; 
    private const string JumpTrigger = "Jump"; 
    private const string AttackTrigger = "Attack"; 
    private const string PlayerTag = "Player"; // Tag del jugador

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
        
        // Buscar el objeto del jugador por Tag
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

        // 1. Lógica de IA - Determinar el comportamiento
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                // Estado: ATAQUE CUCHILLO (Melee)
                AttemptMeleeAttack();
                StopMovement();
                FacePlayer();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                // Estado: PERSEGUIR (CHASE)
                isAttacking = false;
                ChasePlayer();
            }
            else
            {
                // Estado: PATRULLA
                isAttacking = false;
                Patrol();
            }
        }
        else
        {
            Patrol(); 
        }
        
        // 2. Ejecutar movimiento y volteo basado en la dirección
        ApplyMovementAndFlip();
        
        // 3. Chequeo de obstáculos (Salto)
        bool hitWall = CheckForWall();
        if (isGrounded && hitWall && !isAttacking)
        {
            Jump();
        }

        // Animación: Actualizar el parámetro IsRunning
        bool isRunning = Mathf.Abs(rb.velocity.x) > 0.1f && !isAttacking;
        if (animator != null)
        {
            animator.SetBool(IsRunningParam, isRunning);
        }
    }

    // --- MÉTODOS DE ATAQUE MELEE ---

    void AttemptMeleeAttack()
    {
        isAttacking = true;
        attackTimer -= Time.deltaTime;
        
        if (attackTimer <= 0)
        {
            MeleeAttack(); // Ejecuta el ataque
            attackTimer = 1f / attackRate;
        }
    }

    void MeleeAttack()
    {
        // 1. Activar animación de ataque de cuchillo
        if (animator != null)
        {
            animator.SetTrigger(AttackTrigger);
        }

        // 2. Detectar al jugador en el rango de ataque (OverlapCircle)
        // Se usa la posición del soldado y la LayerMask del jugador.
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, whatIsPlayer);
        
        if (hitPlayer != null && hitPlayer.CompareTag(PlayerTag))
        {
            // ASUMIDO: El jugador tiene un script PlayerHealth con una función TakeDamage(int)
            // Debes cambiar PlayerHealth por el nombre real de tu script de salud del jugador.
            // Y la función por la que use tu script.
            
            // Ejemplo (ASUMIDO):
            // PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
            // if (playerHealth != null)
            // {
            //     playerHealth.TakeDamage(meleeDamage);
            // } 
            
            // Usaremos Debug.Log temporalmente hasta que se defina PlayerHealth
            Debug.Log("¡Cuchillazo! El soldado intentó dañar al jugador.");
        }
    }
    
    // Método para dibujar el rango de ataque en el Editor (ayuda visual)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    
    // --- MÉTODOS DE IA Y MOVIMIENTO (Sin cambios funcionales) ---

    void ChasePlayer()
    {
        direction = (player.position.x > transform.position.x) ? 1 : -1;
    }
    
    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            soldierVisuals.localScale = new Vector3(1, 1, 1);
        }
        else if (player.position.x < transform.position.x)
        {
            soldierVisuals.localScale = new Vector3(-1, 1, 1);
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
                soldierVisuals.localScale = new Vector3(1, 1, 1);
            }
            else if (direction < 0)
            {
                soldierVisuals.localScale = new Vector3(-1, 1, 1);
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