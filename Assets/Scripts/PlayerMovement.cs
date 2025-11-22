using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float jumpForce = 500f;

    public float moveSpeed = 5f;

    public Animator torsoAnim;
    public Animator legsAnim;

    private Rigidbody2D rb;
    private bool isGrounded;

    public Transform playerVisuals;

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
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

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


        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(new Vector2(0f, jumpForce));

            legsAnim.SetTrigger("Jump");
            torsoAnim.SetTrigger("Jump");
        }

        bool isJumping = !isGrounded && rb.velocity.y > 0.1f;
        bool isFalling = !isGrounded && rb.velocity.y < -0.1f;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Suelo"))
        {
            Vector2 playerBottom = (Vector2)transform.position + GetComponent<Collider2D>().offset - (Vector2)GetComponent<Collider2D>().bounds.extents;
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