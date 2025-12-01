using UnityEngine;
using System.Collections;

public class SoldierHealth : MonoBehaviour
{
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string DeathTrigger = "Die";
    private const float TotalDeathTime = 3.0f;
    private const float FlickerDuration = 2.0f;
    private const float FlickerStartDelay = TotalDeathTime - FlickerDuration;
    public bool isDead = false;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

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

        StartCoroutine(DieSequence());
    }

    IEnumerator DieSequence()
    {
        if (FlickerStartDelay > 0)
        {
            yield return new WaitForSeconds(FlickerStartDelay);
        }

        if (spriteRenderer != null)
        {
            float flickerTimer = 0f;
            float flickerInterval = 0.08f;

            while (flickerTimer < FlickerDuration)
            {
                Color currentColor = spriteRenderer.color;

                if (currentColor.a > 0.5f)
                {
                    spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
                }
                else
                {
                    spriteRenderer.color = Color.white;
                }

                flickerTimer += flickerInterval;
                yield return new WaitForSeconds(flickerInterval);
            }

            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        }

        Destroy(gameObject);
    }
}