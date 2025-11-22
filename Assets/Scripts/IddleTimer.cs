using UnityEngine;

public class IdleTimer : MonoBehaviour
{
    public float idleTimeoutTime = 5f;

    private float timeSinceLastInput;

    private Animator anim;

    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        timeSinceLastInput = 0f;
    }

    void Update()
    {
        if (rb == null) return;

        bool isMoving = rb.velocity.magnitude > 0.1f;
        bool hasInput = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        bool isShooting = Input.GetKey(KeyCode.X) || Input.GetButton("Fire1");

        if (isMoving || hasInput || isShooting  )
        {
            timeSinceLastInput = 0f;
        }
        else
        {
            timeSinceLastInput += Time.deltaTime;
        }

        if (timeSinceLastInput >= idleTimeoutTime)
        {
            anim.SetTrigger("IddleTimeout");

            timeSinceLastInput = 0f;
        }
    }
}