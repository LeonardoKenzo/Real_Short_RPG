using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum TiposSelecao { Select, SelectAlvo, Todos };

public class PersonagemSelect : MonoBehaviour
{
    [SerializeField] private Image _setaMarcacao, _setaAlvo;
    //[SerializeField] private Animator _animacaoControllerMarcacao, _animacaoControllerAlvo;
    private bool _selecionado, _selecionadoAlvo;

    void Start()
    {
        _selecionado = false;
        _selecionadoAlvo = false;

        _setaMarcacao.enabled = false;
        _setaAlvo.enabled = false;

        _setaMarcacao.GetComponent<Animator>().SetBool("Selected", false);
        _setaAlvo.GetComponent<Animator>().SetBool("Selected", false);
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

    public void ConfirmarHover(TiposSelecao tipo)
    {
        switch (tipo)
        {
            case TiposSelecao.Select:
                _setaMarcacao.enabled = true;
                break;
            case TiposSelecao.SelectAlvo:
                _setaMarcacao.enabled = false;
                _setaAlvo.enabled = true;
                break;
            case TiposSelecao.Todos:
                Debug.LogError("Estado absurdo de " + this.gameObject.name + "!");
                break;
        }
    }

    public void CancelarHover(TiposSelecao tipo)
    {
        switch (tipo)
        {
            case TiposSelecao.Select:
                if (!_selecionado)
                    _setaMarcacao.enabled = false;
                break;
            case TiposSelecao.SelectAlvo:
                if (!_selecionadoAlvo)
                {
                    _setaAlvo.enabled = false;
                    _setaMarcacao.enabled = _selecionado;
                }
                break;
            case TiposSelecao.Todos:
                if (!_selecionado)
                    _setaMarcacao.enabled = false;
                if (!_selecionadoAlvo)
                    _setaAlvo.enabled = false;
                break;
        }
    }

    public void ConfirmarSelecao(TiposSelecao tipo)
    {
        switch (tipo)
        {
            case TiposSelecao.Select:
                _selecionado = true;
                _setaMarcacao.GetComponent<Animator>().SetBool("Selected", true);
                break;
            case TiposSelecao.SelectAlvo:
                _selecionadoAlvo = true;
                _setaAlvo.GetComponent<Animator>().SetBool("Selected", true);
                break;
            case TiposSelecao.Todos:
                Debug.LogError("Estado absurdo de " + this.gameObject.name + "!");
                break;
        }
    }

    public void CancelarSelecao(TiposSelecao tipo)
    {
        switch (tipo)
        {
            case TiposSelecao.Select:
                _selecionado = false;
                _setaMarcacao.GetComponent<Animator>().SetBool("Selected", false);
                break;
            case TiposSelecao.SelectAlvo:
                _selecionadoAlvo = false;
                _setaAlvo.GetComponent<Animator>().SetBool("Selected", false);
                break;
            case TiposSelecao.Todos:
                _selecionado = false;
                _selecionadoAlvo = false;
                _setaMarcacao.GetComponent<Animator>().SetBool("Selected", false);
                _setaAlvo.GetComponent<Animator>().SetBool("Selected", false);
                break;
        }
    }
}
