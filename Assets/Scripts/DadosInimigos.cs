using System.Collections.Generic;
using UnityEngine;

public class DadosInimigos : MonoBehaviour
{
    public static DadosInimigos S_ReferenciaInimigos;

    [SerializeField] private List<PersonagemSO> _inimigos;

    public List<PersonagemSO> Inimigos => _inimigos;

    void Awake()
    {
        if (S_ReferenciaInimigos != null)
        {
            Debug.LogError("DUPLICADA DE DADOS_INIMIGOS!!!!");
            return;
        }

        S_ReferenciaInimigos = this;
    }
}
