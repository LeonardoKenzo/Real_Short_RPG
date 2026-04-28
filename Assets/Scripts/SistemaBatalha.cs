using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SistemaBatalha : MonoBehaviour
{
    [SerializeField] private List<GameObject> _spawnsHerois, _spawnsInimigos;
    [SerializeField] private List<Image> _marcacoesEnergia;
    [SerializeField] private GameObject _vitoriaUI, _derrotaUI;

    private List<GameObject> _herois, _inimigos, _heroisSelecionados, _inimigosSelecionados;
    private GameObject _heroiAtacante;
    private GameObject _cartaAtual;
    private HabilidadesSO _habilidadeAtual;

    private int _energiaMaxima = 5, _energiaAtual;

    private enum EstadoBatalha { START, PLAYER_TURN, ENEMY_TURN, WIN, LOSE }
    private enum EstadoAcaojogador { CHOOSE_SKILL, CHOOSE_TARGET, EXECUTE_ACTION }

    private EstadoBatalha _estado;
    private EstadoAcaojogador _estadoJogador;

    void Start()
    {
        _herois = new List<GameObject>();
        _inimigos = new List<GameObject>();
        _heroisSelecionados = new List<GameObject>();
        _inimigosSelecionados = new List<GameObject>();

        EventosGlobais.PersonagemHoverEnter.AddListener(PersonagemHoverEnter);
        EventosGlobais.PersonagemHoverExit.AddListener(PersonagemHoverExit);
        EventosGlobais.PersonagemSelecionado.AddListener(PersonagemSelected);

        EventosGlobais.CartaHoverEnter.AddListener(CartaHoverEnter);
        EventosGlobais.CartaHoverExit.AddListener(CartaHoverExit);
        EventosGlobais.CartaSelecionada.AddListener(CartaSelecionada);

        EventosGlobais.PersonagemMorreu.AddListener(PersonagemMorreu);

        CriarPersonagens();
        foreach (Image marcadorEnergia in _marcacoesEnergia)
            marcadorEnergia.enabled = false;

        _estado = EstadoBatalha.START;
        _estadoJogador = EstadoAcaojogador.CHOOSE_SKILL;
    }

    void Update()
    {
        switch (_estado)
        {
            case EstadoBatalha.START:
            {
                bool todosHeroisStunados = true;
                foreach (GameObject heroi in _herois)
                {
                    if (!heroi.GetComponent<PersonagemController>().IsStunned)
                    {
                        todosHeroisStunados = false;
                        break;
                    }
                }

                if (todosHeroisStunados)
                {
                    _estado = EstadoBatalha.ENEMY_TURN;
                }
                else
                {
                    //Marca o primeiro herói não stunado
                    foreach (GameObject heroi in _herois)
                    {
                        if (!heroi.GetComponent<PersonagemController>().IsStunned)
                        {
                            _heroiAtacante = heroi;
                            break;
                        }
                    }
                    _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarHover();
                    _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarSelecao();

                    _cartaAtual = null;
                    _habilidadeAtual = null;

                    _marcacoesEnergia[_energiaAtual].enabled = false;
                    _energiaAtual = _energiaMaxima;
                    _marcacoesEnergia[_energiaAtual].enabled = true;

                    TrocarCartas();
                    _estado = EstadoBatalha.PLAYER_TURN;
                    _estadoJogador = EstadoAcaojogador.CHOOSE_SKILL;
                }

                break;
            }
            case EstadoBatalha.PLAYER_TURN:
            {
                if (_estadoJogador == EstadoAcaojogador.CHOOSE_TARGET)
                {
                    bool selecionouHerois = _heroisSelecionados.Count == _habilidadeAtual.QtdAlvosAliados ||
                                                _heroisSelecionados.Count == _herois.Count;
                    bool selecionouInimigos = _inimigosSelecionados.Count == _habilidadeAtual.QtdAlvosInimigos ||
                                                _inimigosSelecionados.Count == _inimigos.Count;

                    if (selecionouHerois && selecionouInimigos)
                    {
                        _estadoJogador = EstadoAcaojogador.EXECUTE_ACTION;

                        List<PersonagemController> alvosHerois = new List<PersonagemController>();
                        List<PersonagemController> alvosInimigos = new List<PersonagemController>();

                        foreach (GameObject aliado in _heroisSelecionados)
                            alvosHerois.Add(aliado.GetComponent<PersonagemController>());
                        foreach (GameObject inimigo in _inimigosSelecionados)
                            alvosInimigos.Add(inimigo.GetComponent<PersonagemController>());

                        _heroiAtacante.GetComponent<PersonagemController>().UsarHabilidade(_habilidadeAtual, alvosHerois, alvosInimigos);

                        _marcacoesEnergia[_energiaAtual].enabled = false;
                        _energiaAtual -= _habilidadeAtual.ActionCost;
                        _marcacoesEnergia[_energiaAtual].enabled = true;

                        DesselecionarTodos();
                        _heroiAtacante.GetComponent<PersonagemSelect>().CancelarSelecao();
                        _heroiAtacante.GetComponent<PersonagemSelect>().CancelarHover();

                        if (_herois.Count == 0)
                            _estado = EstadoBatalha.LOSE;
                        else if (_inimigos.Count == 0)
                            _estado = EstadoBatalha.WIN;
                        else if (_energiaAtual == 0)
                            _estado = EstadoBatalha.ENEMY_TURN;
                        else
                        {
                            _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarHover();
                            _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarSelecao();
                            _estadoJogador = EstadoAcaojogador.CHOOSE_SKILL;
                        }
                    }
                }
                break;
            }
            case EstadoBatalha.ENEMY_TURN:
            {
                foreach (GameObject inimigo in _inimigos)
                {
                    if (inimigo.GetComponent<PersonagemController>().IsStunned)
                        continue;

                    List<HabilidadesSO> habilidadesInimigo = inimigo.GetComponent<PersonagemController>().Skills;
                    HabilidadesSO habilidade = habilidadesInimigo[Random.Range(0, habilidadesInimigo.Count)];

                    List<PersonagemController> alvosHerois = new List<PersonagemController>();
                    List<GameObject> copiaHerois = new List<GameObject>(_herois);
                    List<PersonagemController> alvosInimigos = new List<PersonagemController>();
                    List<GameObject> copiaInimigos = new List<GameObject>(_inimigos);

                    for (int i = 0; i < habilidade.QtdAlvosInimigos && i < _herois.Count; i++)
                    {
                        int pos = Random.Range(0, copiaHerois.Count);
                        alvosHerois.Add(copiaHerois[pos].GetComponent<PersonagemController>());
                        copiaHerois.RemoveAt(pos);
                    }
                    for (int i = 0; i < habilidade.QtdAlvosAliados && i < _inimigos.Count; i++)
                    {
                        int pos = Random.Range(0, copiaInimigos.Count);
                        alvosInimigos.Add(copiaInimigos[pos].GetComponent<PersonagemController>());
                        copiaInimigos.RemoveAt(pos);
                    }

                    inimigo.GetComponent<PersonagemController>().UsarHabilidade(habilidade, alvosInimigos, alvosHerois);
                }

                if (_herois.Count == 0)
                    _estado = EstadoBatalha.LOSE;
                else if (_inimigos.Count == 0)
                    _estado = EstadoBatalha.WIN;
                else
                    _estado = EstadoBatalha.START;

                EventosGlobais.FimRodada.Invoke();

                break;
            }
            case EstadoBatalha.WIN:
            {
                _vitoriaUI.SetActive(true);
                //À partir daqui, o código não deve ficar mais ativo
                Destroy(this);
                break;
            }
            case EstadoBatalha.LOSE:
            {
                _derrotaUI.SetActive(true);
                //À partir daqui, o código não deve ficar mais ativo
                Destroy(this);
                break;
            }
        }
    }

    private void PersonagemHoverEnter(GameObject personagem)
    {
        if (_estado != EstadoBatalha.PLAYER_TURN)
            return;

        switch (_estadoJogador)
        {
            case EstadoAcaojogador.CHOOSE_SKILL:
            {
                if (_herois.Contains(personagem))
                    personagem.GetComponent<PersonagemSelect>().ConfirmarHover();
                break;
            }
            case EstadoAcaojogador.CHOOSE_TARGET:
            {
                if ((_herois.Contains(personagem) && _habilidadeAtual.QtdAlvosAliados > 0) ||
                    (_inimigos.Contains(personagem) && _habilidadeAtual.QtdAlvosInimigos > 0))
                {
                    personagem.GetComponent<PersonagemSelect>().ConfirmarHover();
                }
                break;
            }
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

        switch (_estadoJogador)
        {
            case EstadoAcaojogador.CHOOSE_SKILL:
            {
                if (!_herois.Contains(personagem))
                    return;

                if (personagem.GetComponent<PersonagemController>().IsStunned)
                    return;

                if (_heroiAtacante == personagem)
                {
                    _heroiAtacante.GetComponent<PersonagemSelect>().CancelarSelecao();
                    _heroiAtacante = null;
                }
                else
                {
                    if (_heroiAtacante != null)
                    {
                        _heroiAtacante.GetComponent<PersonagemSelect>().CancelarSelecao();
                        _heroiAtacante.GetComponent<PersonagemSelect>().CancelarHover();
                    }
                    personagem.GetComponent<PersonagemSelect>().ConfirmarSelecao();
                    _heroiAtacante = personagem;
                    TrocarCartas();
                }
                break;
            }
            case EstadoAcaojogador.CHOOSE_TARGET:
            {
                if (_heroisSelecionados.Contains(personagem))
                {
                    _heroisSelecionados.Remove(personagem);
                    personagem.GetComponent<PersonagemSelect>().CancelarSelecao();
                }
                else if (_inimigosSelecionados.Contains(personagem))
                {
                    _inimigosSelecionados.Remove(personagem);
                    personagem.GetComponent<PersonagemSelect>().CancelarSelecao();
                }
                else if (_herois.Contains(personagem) && _habilidadeAtual.QtdAlvosAliados > 0)
                {
                    if (_heroisSelecionados.Count == _habilidadeAtual.QtdAlvosAliados)
                    {
                        _heroisSelecionados[0].GetComponent<PersonagemSelect>().CancelarSelecao();
                        _heroisSelecionados[0].GetComponent<PersonagemSelect>().CancelarHover();
                        _heroisSelecionados.RemoveAt(0);
                    }

                    personagem.GetComponent<PersonagemSelect>().ConfirmarSelecao();
                    _heroisSelecionados.Add(personagem);
                }
                else if (_inimigos.Contains(personagem) && _habilidadeAtual.QtdAlvosInimigos > 0)
                {
                    if (_inimigosSelecionados.Count == _habilidadeAtual.QtdAlvosInimigos)
                    {
                        _inimigosSelecionados[0].GetComponent<PersonagemSelect>().CancelarSelecao();
                        _inimigosSelecionados[0].GetComponent<PersonagemSelect>().CancelarHover();
                        _inimigosSelecionados.RemoveAt(0);
                    }

                    personagem.GetComponent<PersonagemSelect>().ConfirmarSelecao();
                    _inimigosSelecionados.Add(personagem);
                }
                break;
            }
        }
    }

    private void CartaHoverEnter(GameObject carta)
    {
        if (_estado != EstadoBatalha.PLAYER_TURN || _estadoJogador == EstadoAcaojogador.EXECUTE_ACTION)
            return;

        carta.GetComponent<HabilidadesSelect>().ConfirmarHover();
    }

    private void CartaHoverExit(GameObject carta)
    {
        carta.GetComponent<HabilidadesSelect>().CancelarHover();
    }

    private void CartaSelecionada(GameObject carta)
    {
        if (_estado != EstadoBatalha.PLAYER_TURN || _estadoJogador == EstadoAcaojogador.EXECUTE_ACTION)
            return;

        if (HabilidadesController.s_ReferenciaControladorHabilidades.GetHabilidadeCarta(carta).ActionCost > _energiaAtual)
            return;

        switch (_estadoJogador)
        {
            case EstadoAcaojogador.CHOOSE_SKILL:
            {
                _cartaAtual = carta;
                _habilidadeAtual = HabilidadesController.s_ReferenciaControladorHabilidades.GetHabilidadeCarta(_cartaAtual);
                carta.GetComponent<HabilidadesSelect>().ConfirmarSelecao();

                _estadoJogador = EstadoAcaojogador.CHOOSE_TARGET;
                break;
            }
            case EstadoAcaojogador.CHOOSE_TARGET:
            {
                if (carta == _cartaAtual)
                {
                    //Como o DesselecionarTodos cancela o Hover, precisa chamar aqui pra neutralizar
                    DesselecionarTodos();
                    carta.GetComponent<HabilidadesSelect>().ConfirmarHover();

                    _estadoJogador = EstadoAcaojogador.CHOOSE_SKILL;
                }
                else
                {
                    DesselecionarTodos();

                    _cartaAtual = carta;
                    _habilidadeAtual = HabilidadesController.s_ReferenciaControladorHabilidades.GetHabilidadeCarta(_cartaAtual);
                    carta.GetComponent<HabilidadesSelect>().ConfirmarSelecao();
                }
                break;
            }
        }
    }

    private void PersonagemMorreu(GameObject personagem)
    {
        if (_herois.Contains(personagem))
        {
            _herois.Remove(personagem);
        }
        else
        {
            _inimigos.Remove(personagem);
        }

        Destroy(personagem);
    }

    private void CriarPersonagens()
    {
        int idxSpawn = 0;
        foreach (PersonagemSO heroi in DadosParty.s_ReferenciaParty.Herois)
        {
            GameObject heroiAtual = Instantiate(heroi.Prefab, _spawnsHerois[idxSpawn].transform.position, Quaternion.identity, this.transform);
            heroiAtual.GetComponent<PersonagemController>().InicializarPersonagem(heroi);
            _herois.Add(heroiAtual);
            idxSpawn++;
        }
        idxSpawn = 0;
        foreach (PersonagemSO inimigo in DadosInimigos.s_ReferenciaInimigos.Inimigos)
        {
            GameObject inimigoAtual = Instantiate(inimigo.Prefab, _spawnsInimigos[idxSpawn].transform.position, Quaternion.identity, this.transform);
            inimigoAtual.GetComponent<PersonagemController>().InicializarPersonagem(inimigo);
            _inimigos.Add(inimigoAtual);
            idxSpawn++;
        }
    }

    private void TrocarCartas()
    {
        HabilidadesController.s_ReferenciaControladorHabilidades.DesenharCartas(_heroiAtacante.GetComponent<PersonagemController>().Skills);
    }

    private void DesselecionarTodos()
    {
        foreach (GameObject heroi in _heroisSelecionados)
        {
            heroi.GetComponent<PersonagemSelect>().CancelarSelecao();
            heroi.GetComponent<PersonagemSelect>().CancelarHover();
        }
        foreach (GameObject inimigo in _inimigosSelecionados)
        {
            inimigo.GetComponent<PersonagemSelect>().CancelarSelecao();
            inimigo.GetComponent<PersonagemSelect>().CancelarHover();
        }

        _heroisSelecionados.Clear();
        _inimigosSelecionados.Clear();

        _cartaAtual.GetComponent<HabilidadesSelect>().CancelarSelecao();
        _cartaAtual.GetComponent<HabilidadesSelect>().CancelarHover();
        _cartaAtual = null;
        _habilidadeAtual = null;
    }
}
