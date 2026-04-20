using System;
using UnityEngine;

public class PersonagemSelect : MonoBehaviour
{
    private bool _selecionado, _estaEscolhendoAlvo;
    
    public event Action<bool> OnSelected;

    void Start()
    {
        _selecionado = false;

        EventosGlobais.AtivarEscolhaAlvo.AddListener((bool escolha) =>
        {
           _estaEscolhendoAlvo =  escolha;
        });
    }

    public void OnHover()
    {
        
    }

    public void OnExitHover()
    {
        
    }

    public void AliadoSelected()
    {
        EventosGlobais.AliadoSelecionado.Invoke(this.gameObject);

        if(!_estaEscolhendoAlvo)
        {
            _selecionado = !_selecionado;
            OnSelected?.Invoke(_selecionado);
        }
    }

    public void InimigoSelected()
    {
        EventosGlobais.InimigoSelecionado.Invoke(this.gameObject);

        if(!_estaEscolhendoAlvo)
        {
            _selecionado = !_selecionado;
            OnSelected?.Invoke(_selecionado);
        }
    }
}
