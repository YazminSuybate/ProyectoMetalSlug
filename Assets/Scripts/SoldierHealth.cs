using UnityEngine;

public class SoldierHealth : MonoBehaviour
{
    public Animator animator;

    private const string DeathTrigger = "Die";

    private bool isDead = false;

    public void TakeHit()
    {
        if (isDead) return;

        isDead = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger(DeathTrigger);
        }

        Destroy(gameObject, 2.0f);
    }
}