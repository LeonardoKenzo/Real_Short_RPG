using System.Collections.Generic;
using UnityEngine;

public class SistemaBatalha : MonoBehaviour
{
    [SerializeField] private List<GameObject> _spawnsHerois, _spawnsInimigos;

    private List<GameObject> _herois, _inimigos, _heroisSelecionados, _inimigosSelecionados;
    private GameObject _heroiAtacacnte;
    private HabilidadesSO _habilidadeAtual;

    private enum EstadoBatalha { START, PLAYER_TURN, ENEMY_TURN, WIN, LOSE }
    private enum EstadoAcaojogador { CHOOSE_SKILL, CHOOSE_TARGET, EXECUTE_ACTION }

    private EstadoBatalha _estado;
    private EstadoAcaojogador _estadoJogador;

    void Start()
    {
        _herois = new List<GameObject>();
        _inimigos = new List<GameObject>();

        EventosGlobais.PersonagemHoverEnter.AddListener(PersonagemHoverEnter);
        EventosGlobais.PersonagemHoverExit.AddListener(PersonagemHoverExit);
        EventosGlobais.PersonagemSelecionado.AddListener(PersonagemSelected);

        CriarPersonagens();
        _estado = EstadoBatalha.PLAYER_TURN;
        _estadoJogador = EstadoAcaojogador.CHOOSE_TARGET;
        _habilidadeAtual = _herois[0].GetComponent<PersonagemController>().Skills[0];
    }

    private void PersonagemHoverEnter(GameObject personagem)
    {
        if (_estado != EstadoBatalha.PLAYER_TURN)
            return;

        if (_herois.Contains(personagem))
        {
            personagem.GetComponent<PersonagemSelect>().ConfirmarHover();
        }
        else if (_estadoJogador == EstadoAcaojogador.CHOOSE_TARGET && _habilidadeAtual.QtdAlvosInimigos > 0)
        {
            personagem.GetComponent<PersonagemSelect>().ConfirmarHover();
        }
    }

    private void PersonagemHoverExit(GameObject personagem)
    {
        personagem.GetComponent<PersonagemSelect>().CancelarHover();
    }

    private void PersonagemSelected(GameObject personagem)
    {
        if (_estado != EstadoBatalha.PLAYER_TURN)
            return;

        if (_herois.Contains(personagem))
        {
            if (_heroiAtacacnte == personagem)
            {
                personagem.GetComponent<PersonagemSelect>().CancelarSelecao();
                _heroiAtacacnte = null;
            }
            personagem.GetComponent<PersonagemSelect>().ConfirmarSelecao();
            //TODO: interações
        }
        else if (_estadoJogador == EstadoAcaojogador.CHOOSE_TARGET && _habilidadeAtual.QtdAlvosInimigos > 0)
        {
            personagem.GetComponent<PersonagemSelect>().ConfirmarHover();
        }
    }

    private void CriarPersonagens()
    {
        int idxSpawn = 0;
        foreach (PersonagemSO heroi in DadosParty.S_ReferenciaParty.Herois)
        {
            GameObject heroiAtual = Instantiate(heroi.Prefab, _spawnsHerois[idxSpawn].transform.position, Quaternion.identity, this.transform);
            heroiAtual.GetComponent<PersonagemController>().InicializarPersonagem(heroi);
            _herois.Add(heroiAtual);
            idxSpawn++;
        }
        idxSpawn = 0;
        foreach (PersonagemSO inimigo in DadosInimigos.S_ReferenciaInimigos.Inimigos)
        {
            GameObject inimigoAtual = Instantiate(inimigo.Prefab, _spawnsInimigos[idxSpawn].transform.position, Quaternion.identity, this.transform);
            inimigoAtual.GetComponent<PersonagemController>().InicializarPersonagem(inimigo);
            _inimigos.Add(inimigoAtual);
            idxSpawn++;
        }
    }
}
