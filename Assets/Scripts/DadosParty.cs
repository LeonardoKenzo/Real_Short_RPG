using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DadosParty : MonoBehaviour
{
    public static DadosParty s_ReferenciaParty;

    [SerializeField] private List<PersonagemSO> _herois;
    [SerializeField] private PersonagemSO _luz;

    private Dictionary<HabilidadesSO, int> _contagemHabilidades;

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
        _contagemHabilidades = new Dictionary<HabilidadesSO, int>();
        DontDestroyOnLoad(this.gameObject);
    }

    public void HabilidadeUsada(HabilidadesSO habilidade)
    {
        if (_contagemHabilidades.ContainsKey(habilidade))
            _contagemHabilidades[habilidade]++;
        else
            _contagemHabilidades[habilidade] = 1;
    }

    public List<HabilidadesSO> PegarHabilidadesMaisUsadas()
    {
        return _contagemHabilidades
                    .OrderByDescending(habilidade => habilidade.Value)
                    .Take(3)
                    .Select(habilidade => habilidade.Key)
                    .ToList();
    }

    public void RemoverHeroi(PersonagemController heroi)
    {
        int idxHeroi = _herois.FindIndex(item => item.UnitName == heroi.UnitName);
        _herois.RemoveAt(idxHeroi);
    }

    public string RemoverHeroiAleatorio()
    {
        PersonagemSO removido = _herois[UnityEngine.Random.Range(0, _herois.Count)];
        _herois.Remove(removido);

        return removido.UnitName;
    }

    public void ConfigurarLuz()
    {
        _herois.Clear();

        _luz.Skills = PegarHabilidadesMaisUsadas();

        _herois.Add(_luz);
    }
}
