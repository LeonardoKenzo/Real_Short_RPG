using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TelaVitoriaDerrota : MonoBehaviour
{
    [SerializeField] private InputActionReference _acaoPausar;
    [SerializeField] private GameObject _telaPause;

    public void Vencer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void Reiniciar()
    {
        Destroy(DadosParty.s_ReferenciaParty);
        SceneManager.LoadScene(1);
    }

    public void IrMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Pausar()
    {
        if (_telaPause.activeSelf)
        {
            _telaPause.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            _telaPause.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void Start()
    {
        _acaoPausar.action.performed += PausarJogo;
    }

    void OnDestroy()
    {
        _acaoPausar.action.performed -= PausarJogo;
        Time.timeScale = 1f;
    }

    void OnDisable()
    {
        _acaoPausar.action.performed -= PausarJogo;
        Time.timeScale = 1f;
    }

    private void PausarJogo(InputAction.CallbackContext context)
    {
        Pausar();
    }
}
