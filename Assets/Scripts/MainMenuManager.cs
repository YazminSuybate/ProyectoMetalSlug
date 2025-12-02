using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string nombreEscenaJuego = "Mision1";

    public void IniciarJuego()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
    }

}