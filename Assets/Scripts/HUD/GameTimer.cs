using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [Header("Sprites de Números")]
    public Sprite[] numberSprites; 

    [Header("Dígitos del HUD")]
    public SpriteRenderer minuteRenderer; 
    public SpriteRenderer secondTensRenderer;
    public SpriteRenderer secondUnitsRenderer;

    [Header("Configuración de Temporizador")]
    public float startTimeSeconds = 60f; 

    private float currentTime; 
    private bool isRunning = false;

    void Start()
    {
        currentTime = startTimeSeconds;
        UpdateTimerDisplay((int)currentTime);
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            isRunning = true;
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0; 
            }

            UpdateTimerDisplay((int)currentTime);
        }
    }

    void UpdateTimerDisplay(int timeInSeconds)
    {
        if (numberSprites == null || numberSprites.Length < 10)
        {
            return;
        }

        int minutes = timeInSeconds / 60;
        int seconds = timeInSeconds % 60;

        int minuteUnit = minutes % 10;
        if (minuteRenderer != null)
        {
            minuteRenderer.sprite = numberSprites[minuteUnit];
        }

        int secondTens = seconds / 10;
        if (secondTensRenderer != null)
        {
            secondTensRenderer.sprite = numberSprites[secondTens];
        }

        int secondUnits = seconds % 10;
        if (secondUnitsRenderer != null)
        {
            secondUnitsRenderer.sprite = numberSprites[secondUnits];
        }
    }
}