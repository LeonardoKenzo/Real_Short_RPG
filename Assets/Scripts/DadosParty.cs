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

        //Cria os espaços para todas as habilidades
        _contagemHabilidades = new Dictionary<HabilidadesSO, int>();
        //Aleatoriza a ordem que os heróis serão analisados
        //Isso é útil para cancelar um viés que existe na função PegarHabilidadesMaisUsadas()
        foreach (PersonagemSO dadosPersonagem in _herois.OrderBy(_ => Random.value))
        {
            foreach (HabilidadesSO habilidade in dadosPersonagem.Skills)
            {
                _contagemHabilidades[habilidade] = 0;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void HabilidadeUsada(HabilidadesSO habilidade)
    {
        _contagemHabilidades[habilidade]++;
    }

    public List<HabilidadesSO> PegarHabilidadesMaisUsadas()
    {
        HabilidadesSO habilidadeMaisUsadaCusto1 = null;
        HabilidadesSO habilidadeMaisUsadaCusto2 = null;
        HabilidadesSO habilidadeMaisUsadaCusto3 = null;

        int usosCusto1 = -1;
        int usosCusto2 = -1;
        int usosCusto3 = -1;

        //Quando as habilidades tiverem o mesmo número de usos, aleatoriza entre elas qual deve ser
        //Isso cria um viés, onde a habilidade que aparecer por último tem maior probabilidade, pois basta vencer 1 vez
        //Isso foi "cancelado" no Awake(), onde os heróis são analisados em ordem aleatória
        //"cancelado" pois o viés ainda existe, mas ele vai beneficiar heróis diferentes em runs diferentes
        //Então cada herói vai ter uma probabilidade maior por run, mas é negligenciável para a escala do jogo
        foreach (var (habilidade, usos) in _contagemHabilidades)
        {
            switch (habilidade.ActionCost)
            {
                case 1:
                    if (usos > usosCusto1)
                    {
                        usosCusto1 = usos;
                        habilidadeMaisUsadaCusto1 = habilidade;
                    }
                    //Se tiverem o mesmo número de usos, aleatoriza entre elas qual deve manter
                    else if (usos == usosCusto1 && Random.Range(0f, 1f) < 0.5f)
                    {
                        habilidadeMaisUsadaCusto1 = habilidade;
                    }
                    break;

                case 2:
                    if (usos > usosCusto2)
                    {
                        usosCusto2 = usos;
                        habilidadeMaisUsadaCusto2 = habilidade;
                    }
                    else if (usos == usosCusto2 && Random.Range(0f, 1f) < 0.5f)
                    {
                        habilidadeMaisUsadaCusto2 = habilidade;
                    }
                    break;

                case 3:
                    if (usos > usosCusto3)
                    {
                        usosCusto3 = usos;
                        habilidadeMaisUsadaCusto3 = habilidade;
                    }
                    else if (usos == usosCusto3 && Random.Range(0f, 1f) < 0.5f)
                    {
                        habilidadeMaisUsadaCusto3 = habilidade;
                    }
                    break;
            }
        }

        return new List<HabilidadesSO> {
            habilidadeMaisUsadaCusto1,
            habilidadeMaisUsadaCusto2,
            habilidadeMaisUsadaCusto3
        };
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
