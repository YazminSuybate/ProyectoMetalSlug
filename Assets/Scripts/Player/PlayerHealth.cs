using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;

    [Header("Respawn Settings")]
    public float invincibilityDuration = 2.0f;
    public float blinkInterval = 0.1f;
    public float disappearDuration = 0.5f;

    [Header("Death Settings")]
    public string gameOverSceneName = "GameOverScene";
    public float fadeOutDuration = 1.0f;
    public CanvasGroup faderCanvasGroup;

    private int currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;
    private Vector3 deathPosition;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (faderCanvasGroup != null)
        {
            faderCanvasGroup.alpha = 0f;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        Debug.Log("Player ha recibido daño. Vidas restantes: " + currentHealth);

        deathPosition = transform.position;

        if (currentHealth <= 0)
        {
            Die(true);
        }
        else
        {
            Die(false);
        }
    }

    void Die(bool isGameOver)
    {
        if (isDead) return;
        isDead = true;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Static;

        if (spriteRenderer != null) spriteRenderer.enabled = false;

        if (isGameOver)
        {
            StartCoroutine(GameOverSequence());
        }
        else
        {
            StartCoroutine(RespawnSequence());
        }
    }

    IEnumerator RespawnSequence()
    {
        Debug.Log("Player ha muerto, iniciando secuencia de respawn...");

        yield return new WaitForSeconds(disappearDuration);

        transform.position = deathPosition;

        isDead = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = true;

        StartCoroutine(InvincibilityBlink());
    }

    IEnumerator InvincibilityBlink()
    {
        isInvincible = true;
        float startTime = Time.time;

        if (spriteRenderer == null)
        {
            isInvincible = false;
            yield break;
        }

        spriteRenderer.enabled = true;

        while (Time.time < startTime + invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
        Debug.Log("Invencibilidad terminada.");
    }

    IEnumerator GameOverSequence()
    {
        Debug.Log("Player ha muerto. No quedan vidas. Iniciando Game Over...");

        if (faderCanvasGroup != null)
        {
            float timer = 0f;

            while (timer < fadeOutDuration)
            {
                timer += Time.deltaTime;
                faderCanvasGroup.alpha = Mathf.Clamp01(timer / fadeOutDuration);
                yield return null;
            }
        }

        SceneManager.LoadScene(gameOverSceneName);
    }
}