using UnityEngine;
using UnityEngine.UI;

public class FiltroEscuridao : MonoBehaviour
{
    public static FiltroEscuridao S_ReferenciaFiltro;

    [SerializeField] private Image _filtro;
    [SerializeField] private float _escuridaoMaxima, _iteracoesParaMaximizar;

    private float _passoEscuridao, _escuridaoMaximaUnitaria;

    void Awake()
    {
        if (S_ReferenciaFiltro != null)
        {
            Debug.LogError("DUPLICATA DE FILTRO DE ESCURIDÃO!!!!");
            return;
        }

        S_ReferenciaFiltro = this;
    }

    void Start()
    {
        _filtro.color = new Color(_filtro.color.r, _filtro.color.g, _filtro.color.b, 0);
        _escuridaoMaximaUnitaria = _escuridaoMaxima/255;
        _passoEscuridao = _escuridaoMaximaUnitaria/_iteracoesParaMaximizar;
    }

    public void Escurecer()
    {
        float proximaEscuridao = Mathf.Clamp(_filtro.color.a + _passoEscuridao, 0f, _escuridaoMaximaUnitaria);
        _filtro.color = new Color(_filtro.color.r, _filtro.color.g, _filtro.color.b, proximaEscuridao);
    }
}
