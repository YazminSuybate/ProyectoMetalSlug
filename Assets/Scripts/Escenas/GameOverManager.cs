using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public Image panelFade;

    public string nombreEscenaRetry = "Mision1";

    public string nombreEscenaMenuPrincipal = "SeleccionMision";

    public float duracionTransicion = 1.0f;

    void Start()
    {
        if (panelFade != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void Retry()
    {
        StartCoroutine(FadeToLevel(nombreEscenaRetry));
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(FadeToLevel(nombreEscenaMenuPrincipal));
    }

    private IEnumerator FadeIn()
    {
        float tiempo = 0f;

        panelFade.color = new Color(panelFade.color.r, panelFade.color.g, panelFade.color.b, 1f);

        while (tiempo < duracionTransicion)
        {
            tiempo += Time.deltaTime;

            float alpha = 1f - Mathf.Clamp01(tiempo / duracionTransicion);

            panelFade.color = new Color(panelFade.color.r, panelFade.color.g, panelFade.color.b, alpha);

            yield return null;
        }

        panelFade.color = new Color(panelFade.color.r, panelFade.color.g, panelFade.color.b, 0f);
    }

    private IEnumerator FadeToLevel(string sceneName)
    {
        float tiempo = 0f;

        panelFade.gameObject.SetActive(true);

        while (tiempo < duracionTransicion)
        {
            tiempo += Time.deltaTime;

            float alpha = Mathf.Clamp01(tiempo / duracionTransicion);

            panelFade.color = new Color(panelFade.color.r, panelFade.color.g, panelFade.color.b, alpha);

            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}