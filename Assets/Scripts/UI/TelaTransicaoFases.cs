using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaTransicaoFases : MonoBehaviour
{
    [SerializeField] private TMP_Text _caixaTextoAviso;
    [SerializeField] private string _msgAvisoPreNome, _msgAvisoPosNome;

    void Start()
    {
        string nomePersonagem = DadosParty.s_ReferenciaParty.RemoverHeroiAleatorio();

        _caixaTextoAviso.text = _msgAvisoPreNome + " " + nomePersonagem + " " + _msgAvisoPosNome;
    }

    public void AvancarFase()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
