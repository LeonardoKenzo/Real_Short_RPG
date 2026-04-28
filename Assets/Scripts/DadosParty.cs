using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DadosParty : MonoBehaviour
{
    public static DadosParty s_ReferenciaParty;

    [SerializeField] private List<PersonagemSO> _herois;

    public List<PersonagemSO> Herois => _herois;

    void Awake()
    {
        if (s_ReferenciaParty != null)
        {
            Debug.Log("Destruindo duplicata da party...");
            Destroy(this.gameObject);
            return;
        }

        s_ReferenciaParty = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void RemoverHeroi(PersonagemController heroi)
    {
        int idxHeroi = _herois.FindIndex(item => item.name == heroi.name);
        _herois.RemoveAt(idxHeroi);
    }
}
