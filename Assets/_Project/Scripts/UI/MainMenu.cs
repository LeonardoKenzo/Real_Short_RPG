using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Chamado pelo botão "Start Game"
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    // Chamado pelo botão "Quit Game"
    public void QuitGame()
    {
        Application.Quit();
    }
}
