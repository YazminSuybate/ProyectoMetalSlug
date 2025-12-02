using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 2f;
    private Rigidbody2D rb;
    public float baseScaleX = 5.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void Initialize(float directionX)
    {
        rb.velocity = new Vector2(directionX * speed, 0f);

        float finalScaleX = baseScaleX * directionX;

        transform.localScale = new Vector3(finalScaleX, baseScaleX, 1f);

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            SoldierHealth enemyHealth = hitInfo.GetComponent<SoldierHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1);
            }

            Destroy(gameObject);
        }

    }
}