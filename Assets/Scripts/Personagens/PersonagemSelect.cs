using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonagemSelect : MonoBehaviour
{
    [SerializeField] private Image _setaMarcacao;
    [SerializeField] private float _offsetY, _tempoSubida, _tempoDescida;
    private bool _selecionado;
    private Vector2 _posicaoSetaInicial;

    public void OnHoverEnter(BaseEventData data)
    {
        EventosGlobais.PersonagemHoverEnter.Invoke(this.gameObject);
    }

    public void OnHoverExit(BaseEventData data)
    {
        EventosGlobais.PersonagemHoverExit.Invoke(this.gameObject);
    }

    public void OnSelect(BaseEventData data)
    {
        EventosGlobais.PersonagemSelecionado.Invoke(this.gameObject);
    }

    public void ConfirmarHover()
    {
        if (!_selecionado)
            _setaMarcacao.enabled = true;
    }

    public void CancelarHover()
    {
        if (!_selecionado)
            _setaMarcacao.enabled = false;
    }

    public void ConfirmarSelecao()
    {
        _selecionado = true;
        StartCoroutine(MoverSeta());
    }

    public void CancelarSelecao()
    {
        _selecionado = false;
    }

    void Start()
    {
        _selecionado = false;
        _posicaoSetaInicial = _setaMarcacao.rectTransform.anchoredPosition;
        _setaMarcacao.enabled = false;
    }

    private IEnumerator Animar(Vector2 posicaoInicial, Vector2 posicaoFinal, float tempo)
    {
        float progressao = 0f;
        while (progressao < 1.0f && _selecionado)
        {
            progressao += 1f/tempo * Time.deltaTime;
            _setaMarcacao.rectTransform.anchoredPosition = Vector3.Lerp(posicaoInicial, posicaoFinal, Mathf.SmoothStep(0f, 1f, progressao));
            yield return null;
        }
        _setaMarcacao.rectTransform.anchoredPosition = posicaoFinal;
    }

    private IEnumerator MoverSeta()
    {
        Vector2 posicaoAlta = _posicaoSetaInicial + new Vector2(0, _offsetY);
        while (_selecionado)
        {
            if (_selecionado)
                yield return Animar(_posicaoSetaInicial, posicaoAlta, _tempoSubida);
            if (_selecionado)
                yield return Animar(posicaoAlta, _posicaoSetaInicial, _tempoDescida);
        }
        _setaMarcacao.rectTransform.anchoredPosition = _posicaoSetaInicial;
    }
}
