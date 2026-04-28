using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HabilidadesSelect : MonoBehaviour
{
    [SerializeField] private float _offsetY, _tempoSubida, _tempoDescida;
    private bool _selecionado, _levantado;
    private Image _imagemCarta;
    private Vector2 _posicaoInicial, _posicaoFinal;
    private float _progressao;

    void Start()
    {
        _selecionado = false;
        _levantado = false;

        _imagemCarta = this.gameObject.GetComponent<Image>();
        _posicaoInicial = _imagemCarta.rectTransform.anchoredPosition;
        _posicaoFinal = _posicaoInicial + new Vector2(0, _offsetY);
    }

    void Update()
    {
        _progressao += Time.deltaTime * (_levantado ? 1f/_tempoSubida : -1f/_tempoDescida);
        _progressao = Mathf.Clamp(_progressao, 0f, 1f);

        _imagemCarta.rectTransform.anchoredPosition = Vector2.Lerp(_posicaoInicial, _posicaoFinal, _progressao);
    }

    public void OnHoverEnter(BaseEventData data)
    {
        EventosGlobais.CartaHoverEnter.Invoke(this.gameObject);
    }

    public void OnHoverExit(BaseEventData data)
    {
        EventosGlobais.CartaHoverExit.Invoke(this.gameObject);
    }

    public void OnSelect(BaseEventData data)
    {
        EventosGlobais.CartaSelecionada.Invoke(this.gameObject);
    }

    public void ConfirmarHover()
    {
        if (!_selecionado)
            _levantado = true;
    }

    public void CancelarHover()
    {
        if (!_selecionado)
            _levantado = false;
    }

    public void ConfirmarSelecao()
    {
        _selecionado = true;
    }

    public void CancelarSelecao()
    {
        _selecionado = false;
    }
}
