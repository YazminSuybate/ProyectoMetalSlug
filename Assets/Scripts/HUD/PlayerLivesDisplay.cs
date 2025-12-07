using UnityEngine;

public class PlayerLivesDisplay : MonoBehaviour
{
    [Header("Sprites de Corazones (Vidas)")]
    public SpriteRenderer[] heartRenderers;

    public void UpdateDisplay(int currentLives)
    {
        if (heartRenderers == null || heartRenderers.Length == 0)
        {
            Debug.LogError("No hay SpriteRenderers de Corazones asignados en PlayerLivesDisplay.");
            return;
        }

        int lives = Mathf.Max(0, currentLives);

        for (int i = 0; i < heartRenderers.Length; i++)
        {
            if (heartRenderers[i] != null)
            {
                heartRenderers[i].enabled = (i < lives);
            }
        }
    }
}