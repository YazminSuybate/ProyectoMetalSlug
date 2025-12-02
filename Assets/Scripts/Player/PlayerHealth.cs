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

    [Header("Sprite Components")]
    public SpriteRenderer[] spriteRenderers;

    private int currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;
    private Vector3 deathPosition;

    void Start()
    {
        currentHealth = maxHealth;
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            Debug.LogError("PlayerHealth requiere uno o más SpriteRenderers asignados en el Inspector.");
        }

        if (faderCanvasGroup != null)
        {
            faderCanvasGroup.alpha = 0f;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        deathPosition = transform.position;

        Die(false);
    }

    void Die(bool isGameOver)
    {
        if (isDead) return;
        isDead = true;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Static;

        SetSpritesEnabled(false);

        StartCoroutine(RespawnSequence());
    }

    IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(disappearDuration);

        if (currentHealth <= 0)
        {
            StartCoroutine(GameOverSequence());
            yield break;
        }

        // Respawn:
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

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            isInvincible = false;
            yield break;
        }

        SetSpritesEnabled(true);

        while (Time.time < startTime + invincibilityDuration)
        {
            SetSpritesEnabled(!spriteRenderers[0].enabled);
            yield return new WaitForSeconds(blinkInterval);
        }

        SetSpritesEnabled(true);
        isInvincible = false;
    }

    void SetSpritesEnabled(bool enabled)
    {
        if (spriteRenderers != null)
        {
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                if (sr != null)
                {
                    sr.enabled = enabled;
                }
            }
        }
    }

    IEnumerator GameOverSequence()
    {
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