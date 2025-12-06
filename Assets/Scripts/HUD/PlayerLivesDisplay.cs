using UnityEngine;

public class PlayerLivesDisplay : MonoBehaviour
{
    [Header("Sprites de Números")]
    public Sprite[] numberSprites;

    [Header("Dígitos del HUD")]
    public SpriteRenderer unitRenderer; 

    public void UpdateDisplay(int currentLives)
    {
        if (numberSprites == null || numberSprites.Length < 10)
        {
            return;
        }

        int lives = Mathf.Max(0, currentLives);

        int unitDigit = lives % 10;
        
        if (unitRenderer != null)
        {
            unitRenderer.sprite = numberSprites[unitDigit];
        }
    }
}