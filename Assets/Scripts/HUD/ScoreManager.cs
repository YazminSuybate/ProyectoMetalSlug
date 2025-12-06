using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    [Header("Sprites de Números")]
    public Sprite[] numberSprites;

    [Header("Dígitos del HUD")]
    public SpriteRenderer scoreTensThousandsRenderer; 
    public SpriteRenderer scoreThousandsRenderer;
    public SpriteRenderer scoreHundredsRenderer;
    public SpriteRenderer scoreTensRenderer;
    public SpriteRenderer scoreUnitsRenderer;

    [Header("Configuración de Puntuación")]
    public int scoreToWin = 10000; 
    public string winSceneName = "WinScene"; 
    public float fadeOutDuration = 1.0f;
    public CanvasGroup faderCanvasGroup; 

    private static ScoreManager instance;
    private int currentScore = 0;
    private bool gameWon = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreDisplay();
        if (faderCanvasGroup != null)
        {
            faderCanvasGroup.alpha = 0f;
        }
    }

    public static void AddScore(int amount)
    {
        if (instance != null && !instance.gameWon)
        {
            instance.currentScore += amount;
            instance.UpdateScoreDisplay();
            instance.CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (currentScore >= scoreToWin && !gameWon)
        {
            gameWon = true;
            StartCoroutine(WinSequence());
        }
    }

    IEnumerator WinSequence()
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
        
        SceneManager.LoadScene(winSceneName);
    }

    void UpdateScoreDisplay()
    {
        if (numberSprites == null || numberSprites.Length < 10)
        {
            return;
        }

        int score = Mathf.Max(0, currentScore);

        int tensThousands = (score / 10000) % 10;
        if (scoreTensThousandsRenderer != null)
        {
            scoreTensThousandsRenderer.sprite = numberSprites[tensThousands];
        }

        int thousands = (score / 1000) % 10;
        if (scoreThousandsRenderer != null)
        {
            scoreThousandsRenderer.sprite = numberSprites[thousands];
        }

        int hundreds = (score / 100) % 10;
        if (scoreHundredsRenderer != null)
        {
            scoreHundredsRenderer.sprite = numberSprites[hundreds];
        }

        int tens = (score / 10) % 10;
        if (scoreTensRenderer != null)
        {
            scoreTensRenderer.sprite = numberSprites[tens];
        }

        int units = score % 10;
        if (scoreUnitsRenderer != null)
        {
            scoreUnitsRenderer.sprite = numberSprites[units];
        }
    }
}