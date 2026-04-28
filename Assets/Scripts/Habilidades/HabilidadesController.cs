using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HabilidadesController : MonoBehaviour
{
    public static HabilidadesController s_ReferenciaControladorHabilidades;

    [SerializeField] private GameObject _prefabCarta;

    private List<GameObject> _cartas;
    private List<HabilidadesSO> _habilidades;

    //Porcentagem
    private const float _distanciaEntreCartas = 0.1f;

    void Awake()
    {
        if (s_ReferenciaControladorHabilidades != null)
        {
            Debug.LogError("DUPLICADA DE CONTROLADOR_HABILIDADES!!!!");
            return;
        }

        s_ReferenciaControladorHabilidades = this;

        _cartas = new List<GameObject>();
        _habilidades = new List<HabilidadesSO>();
    }

    public void DesenharCartas(List<HabilidadesSO> habilidades)
    {
        foreach (Transform filho in this.gameObject.transform)
        {
            Destroy(filho.gameObject);
        }
        _cartas.Clear();

        Image imagemCarta = _prefabCarta.GetComponent<Image>();
        float larguraCarta = imagemCarta.rectTransform.sizeDelta.x;

        float meioTela = ((RectTransform)this.transform).anchoredPosition.x;
        float separacao = _distanciaEntreCartas * larguraCarta;
        float posInicial = meioTela;

        if (habilidades.Count > 1)
        {
            posInicial -= larguraCarta/2f;
            if (habilidades.Count % 2 == 1)
            {
                posInicial -= larguraCarta/2f + separacao + (((int)habilidades.Count/2) - 1) * (larguraCarta + separacao);
            }
            else
            {
                posInicial -= separacao/2 + (((int)habilidades.Count/2) - 1) * (larguraCarta + separacao);
            }
        }

        foreach (HabilidadesSO habil in habilidades)
        {
            GameObject carta = Instantiate(_prefabCarta, this.transform);
            ((RectTransform)carta.transform).anchoredPosition = new Vector3(posInicial, 0, 0);
            carta.GetComponent<Image>().sprite = habil.SkillImage;

            _cartas.Add(carta);
            posInicial += separacao + larguraCarta;
        }
        _habilidades = habilidades;
    }

    public HabilidadesSO GetHabilidadeCarta(GameObject carta)
    {
        int index = _cartas.FindIndex(x => x == carta);
        return _habilidades[index];
    }
}
