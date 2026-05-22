using System.Collections.Generic;
using UnityEngine;

public class DadosInimigos : MonoBehaviour
{
    public static DadosInimigos s_ReferenciaInimigos;

    [SerializeField] private List<PersonagemSO> _inimigos;

    public List<PersonagemSO> Inimigos => _inimigos;

    void Awake()
    {
        if (s_ReferenciaInimigos != null)
        {
            Debug.LogError("DUPLICADA DE DADOS_INIMIGOS!!!!");
            return;
        }

        s_ReferenciaInimigos = this;
    }
}
