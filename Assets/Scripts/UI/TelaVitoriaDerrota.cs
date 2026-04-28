using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaVitoriaDerrota : MonoBehaviour
{
    public void Vencer()
    {
        //Quando tiver próximas fases, trocar para +1
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(1);
    }

    public void IrMenu()
    {
        SceneManager.LoadScene(0);
    }
}
