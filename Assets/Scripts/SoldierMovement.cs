using UnityEngine;

public class SoldierMovement : MonoBehaviour
{
    // --- Configuración Pública ---
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float jumpForce = 400f;

    [Header("Patrol")]
    // Tiempo en segundos antes de que el enemigo cambie de dirección (como respaldo)
    public float patrolTime = 3f; 

    [Header("Obstacle Check")]
    // Objeto hijo que define el punto de inicio del rayo (DEBE ASIGNARSE)
    public Transform wallCheckPoint; 
    // Distancia que el rayo debe buscar un obstáculo (pared o escalón)
    public float wallCheckDistance = 0.5f; 
    // La LayerMask que define qué es el suelo/pared para el raycast (DEBE ASIGNARSE)
    public LayerMask whatIsGround;

    [Header("References")]
    public Animator animator; 
    public Transform soldierVisuals; 

    // --- Variables Privadas ---
    private Rigidbody2D rb;
    private bool isGrounded;
    private int direction = 1; // 1 para derecha, -1 para izquierda
    private float patrolTimer;
    
    private const string IsRunningParam = "IsRunning"; 
    private const string JumpTrigger = "Jump"; 

    void Start()
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

        patrolTimer = patrolTime;
    }

    void Update()
    {
        SoldierHealth health = GetComponent<SoldierHealth>();
        // Detiene la lógica si el soldado está muerto
        if (health != null && health.isDead) return; 

        // --- Lógica de Movimiento y Patrulla ---
        patrolTimer -= Time.deltaTime;

        // Cambiar de dirección después del tiempo de patrulla (como fallback)
        if (patrolTimer <= 0)
        {
            direction *= -1;
            patrolTimer = patrolTime;
        }

        // 1. Chequeo de Obstáculos: Si está en el suelo y detecta una pared/escalón
        bool hitWall = CheckForWall();

        // 2. Lógica de Salto: SOLO si está en tierra Y golpea una pared/obstáculo
        if (isGrounded && hitWall)
        {
            Jump();
            // El enemigo sigue corriendo, esperando que el salto lo libre del obstáculo.
        }

        // 3. Aplicar movimiento horizontal
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // Voltear el visual del enemigo
        if (direction > 0)
        {
            soldierVisuals.localScale = new Vector3(1, 1, 1);
        }
        else if (direction < 0)
        {
            soldierVisuals.localScale = new Vector3(-1, 1, 1);
        }

        // Animación: Actualizar el parámetro IsRunning
        bool isRunning = Mathf.Abs(rb.velocity.x) > 0.1f;
        if (animator != null)
        {
            animator.SetBool(IsRunningParam, isRunning);
        }
    }

    bool CheckForWall()
    {
        if (wallCheckPoint == null) return false;

        // Calcula la dirección del rayo (basado en 'direction')
        Vector2 rayDirection = new Vector2(direction, 0);

        // Dibuja el rayo en el editor para visualización (rojo si golpea)
        Debug.DrawRay(wallCheckPoint.position, rayDirection * wallCheckDistance, Color.yellow);

        // Realiza el Raycast para detectar obstáculos en la LayerMask 'whatIsGround'
        RaycastHit2D hit = Physics2D.Raycast(wallCheckPoint.position, rayDirection, wallCheckDistance, whatIsGround);

        // Devuelve verdadero si el rayo golpea algo
        return hit.collider != null;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f); 
        rb.AddForce(new Vector2(0f, jumpForce));

        if (animator != null)
        {
            animator.SetTrigger(JumpTrigger);
        }
    }

    // --- Detección de Suelo (Sin cambios) ---
    void OnCollisionEnter2D(Collision2D col)
    {
        // El PlayerMovement usa la etiqueta "Suelo"
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