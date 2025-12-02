using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;

    [Header("Death Settings")]
    public string gameOverSceneName = "GameOverScene";
    
    public float fadeOutDuration = 1.0f; 
    
    public CanvasGroup faderCanvasGroup; 

    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        
        if (faderCanvasGroup != null)
        {
            faderCanvasGroup.alpha = 0f;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; 

        currentHealth -= damage;
        Debug.Log("Player ha recibido daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true; 

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        Debug.Log("Player ha muerto. Iniciando secuencia de transición...");

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Static;

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