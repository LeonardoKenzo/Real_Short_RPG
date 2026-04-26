using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DadosParty : MonoBehaviour
{
    public static DadosParty S_ReferenciaParty;

    [SerializeField] private List<PersonagemSO> _herois;

    public List<PersonagemSO> Herois => _herois;

    void Awake()
    {
        if (S_ReferenciaParty != null)
        {
            Debug.Log("Destruindo duplicata da party...");
            Destroy(this.gameObject);
            return;
        }

        S_ReferenciaParty = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void RemoverHeroi(PersonagemController heroi)
    {
        int idxHeroi = _herois.FindIndex(item => item.name == heroi.name);
        _herois.RemoveAt(idxHeroi);
    }
}
