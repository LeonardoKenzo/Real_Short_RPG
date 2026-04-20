using System;
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
    [SerializeField] private List<Transform> _heroSpawn, _enemySpawn;

    private BattleState _state;
    private PlayerActionState _playerActionState;

    private List<CharacterRuntimeData> _party, _enemies;

    private const int ACTIONS_MAX = 5;
    private int _actionsCurrent;
    private int _indexSkillSelected;
    private bool _isPlayerTurnEnded = false;
    private CharacterRuntimeData _targetSelected;
    private CharacterRuntimeData _activeHero;

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
        int heroIndex = 0;
        foreach (var hero in battleData._partyData)
        {
            GameObject heroObject = Instantiate(hero.Prefab, _heroSpawn[heroIndex]);
            CharacterRuntimeData runtimeHero = heroObject.GetComponent<CharacterRuntimeData>();
            runtimeHero.InitializeStats(hero, this);
            _party.Add(runtimeHero);
            heroIndex++;
        }

        // Instancia todos os viloes da cena
        int enemyIndex = 0;
        foreach (var enemy in battleData._enemiesData)
        {
            GameObject enemyObject = Instantiate(enemy.Prefab, _enemySpawn[enemyIndex]);
            CharacterRuntimeData runtimeEnemy = enemyObject.GetComponent<CharacterRuntimeData>();
            runtimeEnemy.InitializeStats(enemy, this);
            _enemies.Add(runtimeEnemy);
            enemyIndex++;
        }
    }

    void Update()
    {
        switch (_state)
        {
            case BattleState.START:
                yield return StartCoroutine(StartBattle());
                break;
            case BattleState.PLAYER_TURN:
                yield return StartCoroutine(PlayerTurn());
                break;
            case BattleState.ENEMY_TURN:
                yield return StartCoroutine(EnemyTurn());
                break;
            case BattleState.WIN:
            case BattleState.LOSE:
                yield return StartCoroutine(EndBattle(_state == BattleState.WIN));
                yield break;
        }
    }
}
