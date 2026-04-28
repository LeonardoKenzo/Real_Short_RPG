using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonagemSelect : MonoBehaviour
{
    [SerializeField] private Image _setaMarcacao;
    [SerializeField] private Animator _animacaoController;
    private bool _selecionado;

    void Start()
    {
        _selecionado = false;
        _setaMarcacao.enabled = false;
    }

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
        _animacaoController.SetBool("Selected", true);
    }

    public void CancelarSelecao()
    {
        _selecionado = false;
        _animacaoController.SetBool("Selected", false);
    }
}
