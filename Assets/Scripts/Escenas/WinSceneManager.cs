using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI; 

public class WinSceneManager : MonoBehaviour
{
    public Image panelFade;

    public string nombreEscenaJuego = "SeleccionMision";

    public float duracionTransicion = 1.0f;

    public void IniciarJuego()
    {
        StartCoroutine(FadeToLevel(nombreEscenaJuego));
    }

    private IEnumerator FadeToLevel(string sceneName)
    {
        float tiempo = 0f;

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