using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionTextAnimator : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public List<GameObject> letterObjects;

    [Header("Configuración de la Animación")]
    public float delayBetweenLetters = 0.1f;
    public float animationDuration = 0.4f;
    [Tooltip("Offset (X/Y) de la posición inicial. Ignora Z en 2D.")]
    public Vector3 startOffset = new Vector3(0, -5, 0);
    public float initialScale = 0.1f;

    [Header("Desaparición")]
    public float visibleDuration = 5f;
    public float fadeOutDuration = 0.5f;

    private Vector3[] targetPositions;
    private Vector3[] targetScales;
    private SpriteRenderer[] spriteRenderers;

    void Awake()
    {
        int count = letterObjects.Count;
        targetPositions = new Vector3[count];
        targetScales = new Vector3[count];
        spriteRenderers = new SpriteRenderer[count];

        for (int i = 0; i < count; i++)
        {
            GameObject letter = letterObjects[i];

            targetPositions[i] = letter.transform.localPosition;
            targetScales[i] = letter.transform.localScale;

            spriteRenderers[i] = letter.GetComponent<SpriteRenderer>();
            if (spriteRenderers[i] == null)
            {
                Debug.LogError($"La letra {letter.name} no tiene un componente SpriteRenderer.");
                continue;
            }

            Vector3 startPos = targetPositions[i] + new Vector3(startOffset.x, startOffset.y, 0);
            letter.transform.localPosition = startPos;

            letter.transform.localScale = Vector3.one * initialScale;

            Color startColor = spriteRenderers[i].color;
            startColor.a = 0f;
            spriteRenderers[i].color = startColor;
        }

        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogError("No se encontró el script PlayerMovement. Asegúrate de asignarlo o de que esté activo en la escena.");
            }
        }

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
        }
    }

    void Start()
    {
        StartCoroutine(AnimateMissionText());
    }

    IEnumerator AnimateMissionText()
    {
        for (int i = 0; i < letterObjects.Count; i++)
        {
            GameObject currentLetter = letterObjects[i];
            SpriteRenderer sr = spriteRenderers[i];

            Vector3 startPosition = currentLetter.transform.localPosition;
            Vector3 targetPosition = targetPositions[i];
            Vector3 startScale = currentLetter.transform.localScale;
            Vector3 targetScale = targetScales[i];

            Color startColor = sr.color;
            Color targetColor = sr.color;
            targetColor.a = 1f;

            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                float t = elapsedTime / animationDuration;
                float curveT = 1f - Mathf.Pow(1f - t, 2);

                currentLetter.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, curveT);
                currentLetter.transform.localScale = Vector3.Lerp(startScale, targetScale, curveT);
                sr.color = Color.Lerp(startColor, targetColor, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentLetter.transform.localPosition = targetPosition;
            currentLetter.transform.localScale = targetScale;
            sr.color = targetColor;

            yield return new WaitForSeconds(delayBetweenLetters);
        }

        Debug.Log("Animación de 'MISIÓN 1' completada.");

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true);
            Debug.Log("Movimiento del jugador habilitado.");
        }

        yield return new WaitForSeconds(visibleDuration);

        float fadeTimer = 0f;

        while (fadeTimer < fadeOutDuration)
        {
            float t = fadeTimer / fadeOutDuration;
            for (int i = 0; i < letterObjects.Count; i++)
            {
                SpriteRenderer sr = spriteRenderers[i];
                Color currentColor = sr.color;
                currentColor.a = Mathf.Lerp(1f, 0f, t);
                sr.color = currentColor;
            }
            fadeTimer += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < letterObjects.Count; i++)
        {
            SpriteRenderer sr = spriteRenderers[i];
            Color finalColor = sr.color;
            finalColor.a = 0f;
            sr.color = finalColor;

            letterObjects[i].SetActive(false);
        }

        Debug.Log("Texto de misión desaparecido.");
    }
}