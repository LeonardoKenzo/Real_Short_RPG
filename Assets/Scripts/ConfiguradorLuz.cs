using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfiguradorLuz : MonoBehaviour
{
    public void Avancar()
    {
        DadosParty.s_ReferenciaParty.ConfigurarLuz();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
