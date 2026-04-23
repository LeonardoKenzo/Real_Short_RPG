using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Gerencia todas as batalhas do jogo utilizando uma m�quina
 * de estados para controlar o fluxo de turnos e a��es.
 *
 * Fluxo da batalha:
 * 1) Start
 *    - Inicia a batalha
 *    - Instancia cen�rio, her�is e inimigos
 *    - Exibe mensagem inicial
 * 2) PlayerTurn
 *    - Jogador escolhe a��es (ataque, habilidade, etc)
 * 3) EnemyTurn
 *    - Inimigos executam suas a��es automaticamente
 * 4) Loop de batalha
 *    - Alterna entre PlayerTurn e EnemyTurn
 *    - Verifica condi��o de vit�ria ou derrota
 * 5) Win / Lose
 *    - Finaliza a batalha
 *    - Troca para tela de resultado
 *
 * -------------------------------------------------------
 * Como usar:
 * 1) Criar um GameObject chamado "BattleSystem"
 * 2) Adicionar filhos com Transform para posi��es de
 *    her�is e inimigos
 * 3) Anexar este script ao GameObject BattleSystem
 * 4) Definir o BattleUI na vari�vel _battleUI
 *
 * -------------------------------------------------------
 * Depend�ncias:
 * - BattleUI
 * - CharacterRuntimeData
 * - Transforms para instanciar her�is e inimigos
 */

public class SistemaBatalha : MonoBehaviour
{
    private enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WIN, LOSE }
    private enum PlayerActionState { CHOOSE_SKILL, CHOOSE_TARGET, EXECUTE_ACTION }

    [SerializeField] private BatalhaUI _battleUI;
    [SerializeField] private PartyGerenciador _partyGerenciador;
    [SerializeField] private List<Transform> _heroSpawn, _enemySpawn;

    private BattleState _state;
    private PlayerActionState _playerActionState;

    private List<PersonagemRuntime> _party, _enemies;

    private const int ACTIONS_MAX = 5;
    private int _actionsCurrent;
    private int _indexSkillSelected;
    private bool _isPlayerTurnEnded = false;
    private PersonagemRuntime _targetSelected;
    private PersonagemRuntime _activeHero;

    public event Action OnPassTurn;
    public event Action<int> OnChangeActions;

    void Start()
    {
        _state = BattleState.START;
        InitializeBattle();
    }

    private void InitializeBattle()
    {
        _battleUI.Initialize(this);
        _actionsCurrent = ACTIONS_MAX;

        var battleData = GameManager.Instance._battleDataManager;

        // Instancia todos os herois da cena
        _party = _partyGerenciador.CriarHerois(_heroSpawn);

        // Instancia todos os viloes da cena
        _enemies = _partyGerenciador.CriarInimigos(_enemySpawn);
    }

    void Update()
    {
        switch (_state)
        {
            case BattleState.START:
                StartBattle();
                break;
            case BattleState.PLAYER_TURN:
                PlayerTurn();
                break;
            case BattleState.ENEMY_TURN:
                EnemyTurn();
                break;
            case BattleState.WIN:
            case BattleState.LOSE:
                EndBattle(_state == BattleState.WIN);
                break;
        }
    }

    private void StartBattle()
    {
        // Texto de dialogo, efeitos, etc antes da batalha
        Debug.Log("Um inimigo se aproxima!!");

        _state = BattleState.PLAYER_TURN;
    }

    private void PlayerTurn()
    {
        _isPlayerTurnEnded = false; // Controla se o turno do player foi encerrado
        _playerActionState = PlayerActionState.CHOOSE_SKILL;

        // Recupera todas as acoes dos herois no comeco do turno
        _actionsCurrent = ACTIONS_MAX;
        _activeHero = _party[0];

        // Atualiza a UI
        _battleUI.UpdateCursorPosition(_actionsCurrent);
        _battleUI.UpdateSkillCards(_activeHero.GetSkillsImages());

        Debug.Log("Seu turno! Escolha uma habilidade (1, 2 ou 3).");

        // Verifica se ganhou
        if (CheckBattleEnd())
            return;

        Debug.Log("Jogador encerrou o turno");

        _state = BattleState.ENEMY_TURN;
    }

    private void EnemyTurn()
    {
        Debug.Log("Turno do inimigo...");

        foreach (var enemy in _enemies)
        {
            if (enemy == null)
                continue;

            // Escolhe her�i vivo aleatoriamente
            var livingHeroes = _party.FindAll(hero => (hero != null && hero.HpCurrent > 0));
            if (livingHeroes.Count == 0)
                break;

            var target = livingHeroes[UnityEngine.Random.Range(0, livingHeroes.Count)];

            Debug.Log($"{enemy.Name} usa a habilidade \"{enemy.Skills[0].name}\" em {target.Name}! Causou {target.Skills[0].Power} de dano!");
            enemy.UseSkill(enemy.Skills[0], target, false); // Criar diferentes _skills do inimigo
        }

        // Verifica se ganhou
        if (CheckBattleEnd())
            return;

        OnPassTurn?.Invoke();
        _state = BattleState.PLAYER_TURN;
    }

    private void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Vit�ria!" : "Derrota...");

        if (playerWon)
            _battleUI.WinScreen();
        else
            _battleUI.LoseScreen();
    }

    // Funcoes auxiliares ----------------------------------
    private bool CheckBattleEnd()
    {
        // Remove mortos
        _party.RemoveAll(hero => (hero == null || hero.HpCurrent <= 0));
        var enemiesDead = _enemies.FindAll(enemy => (enemy == null || enemy.HpCurrent <= 0));

        if (enemiesDead.Count == _enemies.Count)
        {
            _enemies.RemoveAll(enemy => (enemy == null || enemy.HpCurrent <= 0));
            _state = BattleState.WIN;
            return true;
        }

        if (_party.Count == 0)
        {
            _state = BattleState.LOSE;
            return true;
        }

        return false;
    }
}
