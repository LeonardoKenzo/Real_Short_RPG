using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SistemaBatalha : MonoBehaviour
{
    public static SistemaBatalha s_SistemaBatalha;

    [SerializeField] private List<GameObject> _spawnsHerois, _spawnsInimigos;
    [SerializeField] private List<Image> _marcacoesEnergia;
    [SerializeField] private GameObject _vitoriaUI, _derrotaUI;

    private List<GameObject> _herois, _inimigos, _heroisSelecionados, _inimigosSelecionados, _inimigosComecoAtaque, _objetosHoverDuranteAnim;
    private List<Vector3> _posicoesLivresHerois, _posicoesLivresInimigos;
    private GameObject _heroiAtacante;
    private GameObject _cartaAtual;
    private HabilidadesSO _habilidadeAtual;

    private int _energiaMaxima = 5, _energiaAtual, _inimigoAtual, _acaoInimigoAtual;

    //WAIT -> os personagens são criados no Start(), isso faz com que o Start() deles ocorra no próximo frame
    //Isso cria uma condição de corrida com o Update(), então o WAIT serve pra forçar à esperar
    private enum EstadoBatalha { WAIT, START, PLAYER_TURN, PLAYER_ANIM, ENEMY_TURN, ENEMY_ANIM, ENEMY_END, WIN, LOSE }
    private enum EstadoAcaojogador { CHOOSE_SKILL, CHOOSE_TARGET, EXECUTE_ACTION }

    private EstadoBatalha _estado;
    private EstadoAcaojogador _estadoJogador;

    void Awake()
    {
        if (s_SistemaBatalha != null)
        {
            Debug.LogError("DUPLICATA DO SISTEMA DE BATALHA!!!!");
            return;
        }

        s_SistemaBatalha = this;
    }

    void Start()
    {
        _herois = new List<GameObject>();
        _inimigos = new List<GameObject>();
        _heroisSelecionados = new List<GameObject>();
        _inimigosSelecionados = new List<GameObject>();
        _objetosHoverDuranteAnim = new List<GameObject>();

        _posicoesLivresHerois = new List<Vector3>();
        _posicoesLivresInimigos = new List<Vector3>();
        foreach (GameObject spawnHeroi in _spawnsHerois)
            _posicoesLivresHerois.Add(spawnHeroi.transform.position);
        foreach (GameObject spawnInimigo in _spawnsInimigos)
            _posicoesLivresInimigos.Add(spawnInimigo.transform.position);

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

        _estado = EstadoBatalha.WAIT;
        _estadoJogador = EstadoAcaojogador.CHOOSE_SKILL;
    }

    void Update()
    {
        switch (_estado)
        {
            //Força o jogo à esperar 1 frame pros personagens terminarem de instanciar
            case EstadoBatalha.WAIT:
            {
                _estado = EstadoBatalha.START;
                break;
            }
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
                    _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarHover(TiposSelecao.Select);
                    _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarSelecao(TiposSelecao.Select);

                    _cartaAtual = null;
                    _habilidadeAtual = null;

                    _marcacoesEnergia[_energiaAtual].enabled = false;
                    _energiaAtual = _energiaMaxima;
                    _marcacoesEnergia[_energiaAtual].enabled = true;

                    TrocarCartas();
                    _estado = EstadoBatalha.PLAYER_TURN;
                    _estadoJogador = EstadoAcaojogador.CHOOSE_SKILL;

                    HoverPosAnimacao();
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

                        DadosParty.s_ReferenciaParty.HabilidadeUsada(_habilidadeAtual);
                        _heroiAtacante.GetComponent<PersonagemController>().UsarHabilidade(_habilidadeAtual, alvosHerois, alvosInimigos);

                        _marcacoesEnergia[_energiaAtual].enabled = false;
                        _energiaAtual -= _habilidadeAtual.ActionCost;
                        _marcacoesEnergia[_energiaAtual].enabled = true;

                        _estado = EstadoBatalha.PLAYER_ANIM;
                    }
                }
                break;
            }
            case EstadoBatalha.PLAYER_ANIM:
            {
                if (_heroiAtacante.GetComponent<PersonagemController>().IsAnimating)
                    break;

                DesselecionarTodos();
                _heroiAtacante.GetComponent<PersonagemSelect>().CancelarSelecao(TiposSelecao.Todos);
                _heroiAtacante.GetComponent<PersonagemSelect>().CancelarHover(TiposSelecao.Todos);

                if (_herois.Count == 0)
                    _estado = EstadoBatalha.LOSE;
                else if (_inimigos.Count == 0)
                    _estado = EstadoBatalha.WIN;
                else if (_energiaAtual == 0)
                {
                    _inimigoAtual = 0;
                    _acaoInimigoAtual = 0;
                    _inimigosComecoAtaque = new List<GameObject>(_inimigos);
                    _estado = EstadoBatalha.ENEMY_TURN;
                }
                else
                {
                    _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarHover(TiposSelecao.Select);
                    _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarSelecao(TiposSelecao.Select);
                    _estado = EstadoBatalha.PLAYER_TURN;
                    _estadoJogador = EstadoAcaojogador.CHOOSE_SKILL;

                    HoverPosAnimacao();
                }

                break;
            }
            case EstadoBatalha.ENEMY_TURN:
            {
                while (_inimigosComecoAtaque[_inimigoAtual].GetComponent<PersonagemController>().IsStunned)
                {
                    _inimigoAtual++;

                    if (_inimigoAtual == _inimigosComecoAtaque.Count)
                    {
                        break;
                    }
                }

                if (_inimigoAtual == _inimigosComecoAtaque.Count)
                {
                    _estado = EstadoBatalha.ENEMY_END;
                    break;
                }

                GameObject inimigo = _inimigosComecoAtaque[_inimigoAtual];
                inimigo.GetComponent<PersonagemSelect>().ConfirmarHover(TiposSelecao.Select);
                inimigo.GetComponent<PersonagemSelect>().ConfirmarSelecao(TiposSelecao.Select);

                List<HabilidadesSO> habilidadesInimigo = inimigo.GetComponent<PersonagemController>().Skills;

                HabilidadesSO habilidade;
                do
                {
                    habilidade = habilidadesInimigo[Random.Range(0, habilidadesInimigo.Count)];
                }
                //Enquanto ele tentar invocar um oponente sem ter espaço para invocar, aleatoriza novamente
                while (habilidade.Effects.Contains(SkillEffects.SUMMON) && _posicoesLivresInimigos.Count == 0);

                List<PersonagemController> alvosHerois = new List<PersonagemController>();
                List<GameObject> copiaHerois = new List<GameObject>(_herois);
                List<PersonagemController> alvosInimigos = new List<PersonagemController>();
                List<GameObject> copiaInimigos = new List<GameObject>(_inimigos);

                for (int i = 0; i < habilidade.QtdAlvosInimigos && i < _herois.Count; i++)
                {
                    int pos = Random.Range(0, copiaHerois.Count);
                    alvosHerois.Add(copiaHerois[pos].GetComponent<PersonagemController>());

                    copiaHerois[pos].GetComponent<PersonagemSelect>().ConfirmarHover(TiposSelecao.SelectAlvo);
                    copiaHerois[pos].GetComponent<PersonagemSelect>().ConfirmarSelecao(TiposSelecao.SelectAlvo);
                    copiaHerois.RemoveAt(pos);
                }
                for (int i = 0; i < habilidade.QtdAlvosAliados && i < _inimigos.Count; i++)
                {
                    int pos = Random.Range(0, copiaInimigos.Count);
                    alvosInimigos.Add(copiaInimigos[pos].GetComponent<PersonagemController>());
                    copiaInimigos.RemoveAt(pos);
                }

                inimigo.GetComponent<PersonagemController>().UsarHabilidade(habilidade, alvosInimigos, alvosHerois);

                _acaoInimigoAtual++;
                _estado = EstadoBatalha.ENEMY_ANIM;
                break;
            }
            case EstadoBatalha.ENEMY_ANIM:
            {
                GameObject inimigo = _inimigosComecoAtaque[_inimigoAtual];
                if (inimigo.GetComponent<PersonagemController>().IsAnimating)
                    break;

                if (_herois.Count == 0)
                    _estado = EstadoBatalha.LOSE;
                else if (_inimigos.Count == 0)
                    _estado = EstadoBatalha.WIN;
                else if (_acaoInimigoAtual >= 
                            AtributosPorClasse.S_ValoresPorClasse[inimigo.GetComponent<PersonagemController>().Classe].QuantidadeAcoes)
                {
                    _acaoInimigoAtual = 0;
                    _inimigoAtual++;
                }

                DesselecionarTodos();

                if (_inimigoAtual == _inimigosComecoAtaque.Count)
                    _estado = EstadoBatalha.ENEMY_END;
                else
                    _estado = EstadoBatalha.ENEMY_TURN;

                break;
            }
            case EstadoBatalha.ENEMY_END:
            {
                EventosGlobais.FimRodada.Invoke();

                if (_herois.Count == 0)
                    _estado = EstadoBatalha.LOSE;
                else if (_inimigos.Count == 0)
                    _estado = EstadoBatalha.WIN;
                else
                    _estado = EstadoBatalha.START;

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
        _objetosHoverDuranteAnim.Add(personagem);

        ValidarPersonagemHoverEnter(personagem);
    }

    private void ValidarPersonagemHoverEnter(GameObject personagem)
    {
        if (_estado != EstadoBatalha.PLAYER_TURN)
            return;

        switch (_estadoJogador)
        {
            case EstadoAcaojogador.CHOOSE_SKILL:
            {
                if (_herois.Contains(personagem))
                    personagem.GetComponent<PersonagemSelect>().ConfirmarHover(TiposSelecao.Select);
                break;
            }
            case EstadoAcaojogador.CHOOSE_TARGET:
            {
                if ((_herois.Contains(personagem) && _habilidadeAtual.QtdAlvosAliados > 0) ||
                    (_inimigos.Contains(personagem) && _habilidadeAtual.QtdAlvosInimigos > 0))
                {
                    personagem.GetComponent<PersonagemSelect>().ConfirmarHover(TiposSelecao.SelectAlvo);
                }
                break;
            }
        }
    }

    private void PersonagemHoverExit(GameObject personagem)
    {
        _objetosHoverDuranteAnim.Remove(personagem);

        switch (_estadoJogador)
        {
            case EstadoAcaojogador.CHOOSE_SKILL:
            {
                personagem.GetComponent<PersonagemSelect>().CancelarHover(TiposSelecao.Select);
                break;
            }
            case EstadoAcaojogador.CHOOSE_TARGET:
            {
                personagem.GetComponent<PersonagemSelect>().CancelarHover(TiposSelecao.SelectAlvo);
                break;
            }
        }
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
                    _heroiAtacante.GetComponent<PersonagemSelect>().CancelarSelecao(TiposSelecao.Select);
                    _heroiAtacante = null;
                    TrocarCartas();
                }
                else
                {
                    if (_heroiAtacante != null)
                    {
                        _heroiAtacante.GetComponent<PersonagemSelect>().CancelarSelecao(TiposSelecao.Select);
                        _heroiAtacante.GetComponent<PersonagemSelect>().CancelarHover(TiposSelecao.Select);
                    }
                    personagem.GetComponent<PersonagemSelect>().ConfirmarSelecao(TiposSelecao.Select);
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
                    personagem.GetComponent<PersonagemSelect>().CancelarSelecao(TiposSelecao.SelectAlvo);
                }
                else if (_inimigosSelecionados.Contains(personagem))
                {
                    _inimigosSelecionados.Remove(personagem);
                    personagem.GetComponent<PersonagemSelect>().CancelarSelecao(TiposSelecao.SelectAlvo);
                }
                else if (_herois.Contains(personagem) && _habilidadeAtual.QtdAlvosAliados > 0)
                {
                    if (_heroisSelecionados.Count == _habilidadeAtual.QtdAlvosAliados)
                    {
                        _heroisSelecionados[0].GetComponent<PersonagemSelect>().CancelarSelecao(TiposSelecao.SelectAlvo);
                        _heroisSelecionados[0].GetComponent<PersonagemSelect>().CancelarHover(TiposSelecao.SelectAlvo);
                        _heroisSelecionados.RemoveAt(0);
                    }

                    personagem.GetComponent<PersonagemSelect>().ConfirmarSelecao(TiposSelecao.SelectAlvo);
                    _heroisSelecionados.Add(personagem);
                }
                else if (_inimigos.Contains(personagem) && _habilidadeAtual.QtdAlvosInimigos > 0)
                {
                    if (_inimigosSelecionados.Count == _habilidadeAtual.QtdAlvosInimigos)
                    {
                        _inimigosSelecionados[0].GetComponent<PersonagemSelect>().CancelarSelecao(TiposSelecao.SelectAlvo);
                        _inimigosSelecionados[0].GetComponent<PersonagemSelect>().CancelarHover(TiposSelecao.SelectAlvo);
                        _inimigosSelecionados.RemoveAt(0);
                    }

                    personagem.GetComponent<PersonagemSelect>().ConfirmarSelecao(TiposSelecao.SelectAlvo);
                    _inimigosSelecionados.Add(personagem);
                }
                break;
            }
        }
    }

    private void CartaHoverEnter(GameObject carta)
    {
        _objetosHoverDuranteAnim.Add(carta);

        ValidarCartaHoverEnter(carta);
    }

    private void ValidarCartaHoverEnter(GameObject carta)
    {
        if (_estado != EstadoBatalha.PLAYER_TURN || _estadoJogador == EstadoAcaojogador.EXECUTE_ACTION)
            return;

        carta.GetComponent<HabilidadesSelect>().ConfirmarHover();
    }

    private void CartaHoverExit(GameObject carta)
    {
        _objetosHoverDuranteAnim.Remove(carta);

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
                    _estadoJogador = EstadoAcaojogador.CHOOSE_SKILL;

                    DesselecionarTodos();

                    //Como o DesselecionarTodos cancela o Hover, precisa chamar aqui pra neutralizar
                    _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarHover(TiposSelecao.Select);
                    _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarSelecao(TiposSelecao.Select);
                }
                else
                {
                    DesselecionarTodos();

                    //Como o DesselecionarTodos cancela o Hover, precisa chamar aqui pra neutralizar
                    _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarHover(TiposSelecao.Select);
                    _heroiAtacante.GetComponent<PersonagemSelect>().ConfirmarSelecao(TiposSelecao.Select);

                    _cartaAtual = carta;
                    _habilidadeAtual = HabilidadesController.s_ReferenciaControladorHabilidades.GetHabilidadeCarta(_cartaAtual);
                    _cartaAtual.GetComponent<HabilidadesSelect>().ConfirmarHover();
                    _cartaAtual.GetComponent<HabilidadesSelect>().ConfirmarSelecao();
                }
                
                break;
            }
        }
    }

    private void PersonagemMorreu(GameObject personagem)
    {
        _objetosHoverDuranteAnim.Remove(personagem);

        if (_herois.Contains(personagem))
        {
            _herois.Remove(personagem);
            _posicoesLivresHerois.Add(personagem.transform.position);

            if(_heroisSelecionados.Contains(personagem))
                _heroisSelecionados.Remove(personagem);
        }
        else
        {
            _inimigos.Remove(personagem);
            _posicoesLivresInimigos.Add(personagem.transform.position);

            if(_inimigosSelecionados.Contains(personagem))
                _inimigosSelecionados.Remove(personagem);
        }

        Destroy(personagem);

        if (_estado != EstadoBatalha.PLAYER_ANIM && _estado != EstadoBatalha.ENEMY_ANIM)
        {
            if (_herois.Count == 0)
                _estado = EstadoBatalha.LOSE;
            else if (_inimigos.Count == 0)
                _estado = EstadoBatalha.WIN;
        }
    }

    private Vector3 InstanciarPersonagem(PersonagemSO personagem, Transform parente, 
                                        List<GameObject> listaPersonagens, List<Vector3> listaPosicoes, bool colocarNoFundo)
    {
        //Como listaPosicoes vai remover a posição ocupada, sempre posiciona o personagem na primeira posição
        Vector3 posicao = listaPosicoes[0];

        GameObject instancia = Instantiate(personagem.Prefab, posicao, Quaternion.identity, parente);
        instancia.GetComponent<PersonagemController>().InicializarPersonagem(personagem);
        listaPersonagens.Add(instancia);

        if (colocarNoFundo)
            instancia.transform.SetAsFirstSibling();

        //Como está ocupado, não pode ser mais usado
        listaPosicoes.RemoveAt(0);

        return posicao;
    }

    //Vector3? == pode retornar nulo
    public Vector3? TentarInvocarPersonagem(GameObject invocador, PersonagemSO invocado)
    {
        if (_herois.Contains(invocador))
        {
            if (_posicoesLivresHerois.Count > 0)
                //true -> invocados ficam no fundo da tela, para não sobreporem o invocador
                return InstanciarPersonagem(invocado, this.transform, _herois, _posicoesLivresHerois, true);
        }
        else if (_inimigos.Contains(invocador))
        {
            if (_posicoesLivresInimigos.Count > 0)
                //true -> invocados ficam no fundo da tela, para não sobreporem o invocador
                return InstanciarPersonagem(invocado, this.transform, _inimigos, _posicoesLivresInimigos, true);
        }

        return null;
    }

    private void CriarPersonagens()
    {
        foreach (PersonagemSO heroi in DadosParty.s_ReferenciaParty.Herois)
        {
            //false -> crie conforme a ordem pré-definida
            InstanciarPersonagem(heroi, this.transform, _herois, _posicoesLivresHerois, false);
        }
        foreach (PersonagemSO inimigo in DadosInimigos.s_ReferenciaInimigos.Inimigos)
        {
            //false -> crie conforme a ordem pré-definida
            InstanciarPersonagem(inimigo, this.transform, _inimigos, _posicoesLivresInimigos, false);
        }
    }

    private void HoverPosAnimacao()
    {
        foreach (GameObject objHover in _objetosHoverDuranteAnim)
        {
            if (_herois.Contains(objHover))
                ValidarPersonagemHoverEnter(objHover);
            else if (!_inimigos.Contains(objHover))
                ValidarCartaHoverEnter(objHover);
        }
        _objetosHoverDuranteAnim.Clear();
    }

    private void TrocarCartas()
    {
        if (_heroiAtacante != null)
            HabilidadesController.s_ReferenciaControladorHabilidades.DesenharCartas(_heroiAtacante.GetComponent<PersonagemController>().Skills);
        else
            HabilidadesController.s_ReferenciaControladorHabilidades.LimparTela();
    }

    private void DesselecionarTodos()
    {
        foreach (GameObject heroi in _herois)
        {
            heroi.GetComponent<PersonagemSelect>().CancelarSelecao(TiposSelecao.Todos);
            heroi.GetComponent<PersonagemSelect>().CancelarHover(TiposSelecao.Todos);
        }
        foreach (GameObject inimigo in _inimigos)
        {
            inimigo.GetComponent<PersonagemSelect>().CancelarSelecao(TiposSelecao.Todos);
            inimigo.GetComponent<PersonagemSelect>().CancelarHover(TiposSelecao.Todos);
        }

        _heroisSelecionados.Clear();
        _inimigosSelecionados.Clear();

        if (_cartaAtual != null)
        {
            _cartaAtual.GetComponent<HabilidadesSelect>().CancelarSelecao();
            _cartaAtual.GetComponent<HabilidadesSelect>().CancelarHover();
            _cartaAtual = null;
            _habilidadeAtual = null;
        }
    }
}
