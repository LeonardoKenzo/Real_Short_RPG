using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

public class BattleSystem : MonoBehaviour
{
    private enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WIN, LOSE }
    private enum PlayerActionState { CHOOSE_SKILL, CHOOSE_TARGET, EXECUTE_ACTION }
 
    private BattleState _state;
    private PlayerActionState _playerActionState;

    [SerializeField] private BattleUI _battleUI;

    [Header("Party List")]
    [SerializeField] private List<CharacterRuntimeData> _party;
    [SerializeField] private List<CharacterRuntimeData> _enemies;

    [Header("Prefabs enemy Spawns")]
    [SerializeField] private List<Transform> _heroSpawn;
    [SerializeField] private List<Transform> _enemySpawn;

    [Header("Controle de a��es")]
    [SerializeField] private const int ACTIONS_MAX = 5;
    [SerializeField] private int _indexSkillSelected;
    [SerializeField] private bool _isPlayerTurnEnded = false;
    [SerializeField] private CharacterRuntimeData _targetSelected;
    [SerializeField] private CharacterRuntimeData _activeHero;
    private int _actionsCurrent;

    public event Action OnPassTurn;
    public event Action<int> OnChangeActions;

    // Come�a o fluxo de batalha chamando essa coroutine
    void Start()
    {
        StartCoroutine(BattleFlow());
    }

    // Update vai controlar apenas os inputs do player
    private void Update()
    {
        if (_state != BattleState.PLAYER_TURN)
            return;

        HandlePlayerInput();
    }

    // Fluxo de Batalha ----------------------------------------------------------------------
    IEnumerator BattleFlow()
    {
        _state = BattleState.START;
        InitializeBattle();

        while (true)
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

    // Funcoes e Coroutines para o fluxo de batalha -------------------------------------
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

    IEnumerator StartBattle()
    {
        // Texto de dialogo, efeitos, etc antes da batalha
        Debug.Log("Um inimigo se aproxima!!");

        yield return new WaitForSeconds(2f);

        _state = BattleState.PLAYER_TURN;
    }

    IEnumerator PlayerTurn()
    {
        _isPlayerTurnEnded = false; // Controla se o turno do player foi encerrado
        _playerActionState = PlayerActionState.CHOOSE_SKILL;

        // Recupera todas as acoes dos herois no comeco do turno
        _actionsCurrent = ACTIONS_MAX;
        _activeHero = _party[0];

        // Atualiza a UI
        _activeHero.IsSelected(true);
        _battleUI.UpdateCursorPosition(_actionsCurrent);
        _battleUI.UpdateSkillCards(_activeHero.GetSkillsImages());

        Debug.Log("Seu turno! Escolha uma habilidade (1, 2 ou 3).");

        // Espera at� que o jogador conclua a a��o
        yield return new WaitUntil(() => _isPlayerTurnEnded);

        // Verifica se ganhou
        if (CheckBattleEnd())
            yield break;

        Debug.Log("Jogador encerrou o turno");

        _state = BattleState.ENEMY_TURN;
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Turno do inimigo...");

        foreach (var enemy in _enemies)
        {
            if (enemy == null)
                continue;
            enemy.IsSelected(true);

            // Escolhe her�i vivo aleatoriamente
            var livingHeroes = _party.FindAll(hero => (hero != null && hero.HpCurrent > 0));
            if (livingHeroes.Count == 0)
                break;

            var target = livingHeroes[UnityEngine.Random.Range(0, livingHeroes.Count)];
            target.IsSelected(true);

            Debug.Log($"{enemy.Name} usa a habilidade \"{enemy.Skills[0].name}\" em {target.Name}! Causou {target.Skills[0].Power} de dano!");
            enemy.UseSkill(enemy.Skills[0], target); // Criar diferentes _skills do inimigo

            yield return new WaitForSeconds(1.5f);

            enemy.IsSelected(false);
            target.IsSelected(false);
        }

        // Verifica se ganhou
        if (CheckBattleEnd())
            yield break;

        OnPassTurn?.Invoke();
        _state = BattleState.PLAYER_TURN;
    }

    IEnumerator EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Vit�ria!" : "Derrota...");

        yield return new WaitForSeconds(2f);

        if (playerWon)
            _battleUI.WinScreen();
        else
            _battleUI.LoseScreen();
    }

    // Funcoes de Input do player -------------------------------------------------------

    void HandlePlayerInput()
    {
        switch (_playerActionState)
        {
            case PlayerActionState.CHOOSE_SKILL:
                InputSwitchHero();
                InputFinishTurn();
                ChooseSkillSelection();
                break;

            case PlayerActionState.CHOOSE_TARGET:
                ChooseTarget();
                break;
        }
    }

    private void InputSwitchHero()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Seleciona o proximo heroi por tab
        {
            int currentIndex = _party.IndexOf(_activeHero);
            int nextIndex = (currentIndex + 1) % _party.Count;
            _activeHero.IsSelected(false);

            _activeHero = _party[nextIndex];
            _activeHero.IsSelected(true);
            _battleUI.UpdateSkillCards(_activeHero.GetSkillsImages());
            _battleUI.UpdateCursorPosition(_actionsCurrent);
            Debug.Log($"Her�i ativo trocado para: {_activeHero.Name}");
        }
    }

    private void InputFinishTurn()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Espaco no teclado finaliza o turno
        {
            _isPlayerTurnEnded = true;
        }
    }

    private void ChooseSkillSelection()
    {
        if (_actionsCurrent == 0)
        {
            Debug.Log("N�o h� mais a��es restantes!");
            _isPlayerTurnEnded = true;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SkillChoosed(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SkillChoosed(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SkillChoosed(2);
    }

    private void SkillChoosed(int index)
    {
        var skill = _activeHero.Skills[index];

        if (skill.ActionCost > _actionsCurrent)
        {
            Debug.Log("A��es insuficientes para usar a habilidade.");
            return;
        }

        _battleUI.MoveUpSmooth(index, 112f, 0.5f);
        Debug.Log($"Habilidade {index + 1}: escolhida! Agora selecione o alvo (1, 2, 3).");
        _indexSkillSelected = index;

        if (skill.IsAOE)
        {
            if (IsBuff(skill.Effect))
                StartCoroutine(ExecuteActionAOE(_party));
            else
                StartCoroutine(ExecuteActionAOE(_enemies));
        }
        else
            _playerActionState = PlayerActionState.CHOOSE_TARGET;
    }


    private void ChooseTarget()
    {
        var effects = _activeHero.Skills[_indexSkillSelected].Effect;
        if (IsBuff(effects))
            SelectTarget(_party);
        else
            SelectTarget(_enemies);
    }

    private void SelectTarget(List<CharacterRuntimeData> party)
    {
        for (int i = 0; i < party.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && party[i] != null)
            {
                _targetSelected = party[i];
                Debug.Log($"Alvo {_targetSelected.Name} selecionado!");
                StartCoroutine(ExecuteAction());
            }
        }
    }

    // Coroutines para executar as skills ------------------------------------------------

    private IEnumerator ExecuteActionAOE(List<CharacterRuntimeData> list)
    {
        _playerActionState = PlayerActionState.EXECUTE_ACTION;
        
        // Cast Skill
        foreach (var target in list) 
        {
            if (target == null)
                continue;
            _activeHero.UseSkill(_activeHero.Skills[_indexSkillSelected], target);
            target.IsSelected(true);
        }
        _actionsCurrent -= _activeHero.Skills[_indexSkillSelected].ActionCost;
        OnChangeActions?.Invoke(_actionsCurrent);

        Debug.Log($"{_activeHero.Name} usou a habilidade \"{_activeHero.Skills[_indexSkillSelected].name}\" nos alvos!");

        yield return new WaitForSeconds(2f);

        _battleUI.MoveDownSmooth(_indexSkillSelected, 112f, 0.5f);

        yield return new WaitForSeconds(1f);

        foreach (var target in list)
        {
            if (target == null || target == _activeHero)
                continue;
            target.IsSelected(false);
        }
        
        if (_actionsCurrent > 0)
        {
            _playerActionState = PlayerActionState.CHOOSE_SKILL;
            Debug.Log($"A��es restantes: {_actionsCurrent}. Escolha nova habilidade ou troque de her�i (tab).");
        }
        else
        {
            _isPlayerTurnEnded = true;
        }
    }
    private IEnumerator ExecuteAction()
    {
        _playerActionState = PlayerActionState.EXECUTE_ACTION;
 
        Debug.Log($"{_activeHero.Name} usou a habilidade \"{_activeHero.Skills[_indexSkillSelected].name}\" em {_targetSelected.Name}!");

        // Cast Skill
        _targetSelected.IsSelected(true);
        _activeHero.UseSkill(_activeHero.Skills[_indexSkillSelected], _targetSelected);
        _actionsCurrent -= _activeHero.Skills[_indexSkillSelected].ActionCost;
        OnChangeActions?.Invoke(_actionsCurrent);

        yield return new WaitForSeconds(2f);

        _battleUI.MoveDownSmooth(_indexSkillSelected, 112f, 0.5f);

        yield return new WaitForSeconds(1f);

        // Marca o turno do jogador como finalizado
        _targetSelected.IsSelected(false);

        if (_actionsCurrent > 0)
        {
            _playerActionState = PlayerActionState.CHOOSE_SKILL;
            Debug.Log($"A��es restantes: {_actionsCurrent}. Escolha nova habilidade");
        }
        else
            _isPlayerTurnEnded= true;
    }

    // Funcoes auxiliares --------------------------------------------------------------

    private bool IsBuff(List<SkillsSO.SkillEffects> effects)
    {
        if (effects.Contains(SkillsSO.SkillEffects.ATTACK) ||
           effects.Contains(SkillsSO.SkillEffects.DEBUFF) ||
           effects.Contains(SkillsSO.SkillEffects.STUN))
            return false;
        return true;
    }

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
