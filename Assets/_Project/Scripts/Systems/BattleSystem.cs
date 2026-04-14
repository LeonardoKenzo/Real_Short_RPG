using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Gerencia todas as batalhas do jogo utilizando uma máquina
 * de estados para controlar o fluxo de turnos e ações.
 *
 * Fluxo da batalha:
 * 1) Start
 *    - Inicia a batalha
 *    - Instancia cenário, heróis e inimigos
 *    - Exibe mensagem inicial
 * 2) PlayerTurn
 *    - Jogador escolhe ações (ataque, habilidade, etc)
 * 3) EnemyTurn
 *    - Inimigos executam suas ações automaticamente
 * 4) Loop de batalha
 *    - Alterna entre PlayerTurn e EnemyTurn
 *    - Verifica condição de vitória ou derrota
 * 5) Win / Lose
 *    - Finaliza a batalha
 *    - Troca para tela de resultado
 *
 * -------------------------------------------------------
 * Como usar:
 * 1) Criar um GameObject chamado "BattleSystem"
 * 2) Adicionar filhos com Transform para posições de
 *    heróis e inimigos
 * 3) Anexar este script ao GameObject BattleSystem
 * 4) Definir o BattleUI na variável _battleUI
 *
 * -------------------------------------------------------
 * Dependências:
 * - BattleUI
 * - CharacterRuntimeData
 * - Transforms para instanciar heróis e inimigos
 */

public class BattleSystem : MonoBehaviour
{
    private enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WIN, LOSE }
    private enum PlayerActionState { CHOOSE_SKILL, CHOOSE_TARGET, EXECUTE_ACTION }
 
    [SerializeField] private BattleState _state;
    [SerializeField] private PlayerActionState _playerActionState;

    [SerializeField] private BattleUI _battleUI;

    [Header("Party List")]
    [SerializeField] private List<CharacterRuntimeData> _party;
    [SerializeField] private List<CharacterRuntimeData> _enemies;

    [Header("Prefabs enemy Spawns")]
    [SerializeField] private List<Transform> _heroSpawn;
    [SerializeField] private List<Transform> _enemySpawn;

    [Header("Controle de ações")]
    [SerializeField] private const int ACTIONS_MAX = 5;
    [SerializeField] private int _indexSkillSelected;
    [SerializeField] private bool _isPlayerTurnEnded = false;
    [SerializeField] private CharacterRuntimeData _targetSelected;
    [SerializeField] private CharacterRuntimeData _activeHero;
    public int _actionsCurrent { get; private set; }

    public event Action OnPassTurn;
    public event Action<int> OnChangeActions;

    void Start()
    {
        InitializeBattle();
    }

    private void Update()
    {
        if (_state != BattleState.PLAYER_TURN)
            return;

        switch (_playerActionState)
        {
            case PlayerActionState.CHOOSE_SKILL:
                InputSwitchHero();
                ChooseSkillSelection();
                InputFinishTurn();
                break;
            case PlayerActionState.CHOOSE_TARGET:
                ChooseTarget();
                break;
        }
    }

    // Fluxo de Batalha ----------------------------------------------------------------------
    private void InitializeBattle()
    {
        _state = BattleState.START;
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

        // Comeca a batalha
        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        // Texto de dialogo, efeitos, etc antes da batalha
        Debug.Log("Um inimigo se aproxima!!");

        yield return new WaitForSeconds(2f);

        _state = BattleState.PLAYER_TURN;
        StartCoroutine(PlayerTurn());
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

        // Espera até que o jogador conclua a ação
        yield return new WaitUntil(() => _isPlayerTurnEnded);

        // Verifica se ganhou
        if (CheckBattleEnd())
            yield break;

        Debug.Log("Jogador encerrou o turno");

        _state = BattleState.ENEMY_TURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Turno do inimigo...");

        foreach (var enemy in _enemies)
        {
            if (enemy == null)
                continue;
            enemy.IsSelected(true);

            // Escolhe herói vivo aleatoriamente
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

        _state = BattleState.PLAYER_TURN;
        OnPassTurn?.Invoke();
        StartCoroutine(PlayerTurn());
    }

    IEnumerator EndBattle(bool playerWon)
    {
        _state = playerWon ? BattleState.WIN : BattleState.LOSE;
        Debug.Log(playerWon ? "Vitória!" : "Derrota...");

        yield return new WaitForSeconds(2f);

        if (_state == BattleState.WIN)
            _battleUI.WinScreen();
        else
            _battleUI.LoseScreen();
    }

    // Funcoes Auxilares para as Batalhas -------------------------------------------------------
    
    private void ChooseSkillSelection()
    {
        if(_actionsCurrent == 0)
        {
            Debug.Log("Não há mais ações restantes!");
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
            Debug.Log("Ações insuficientes para usar a habilidade.");
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
        
        CheckBattleEnd();
        if (_actionsCurrent > 0)
        {
            _playerActionState = PlayerActionState.CHOOSE_SKILL;
            Debug.Log($"Ações restantes: {_actionsCurrent}. Escolha nova habilidade ou troque de herói (tab).");
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
        CheckBattleEnd();
        if (_actionsCurrent > 0)
        {
            _playerActionState = PlayerActionState.CHOOSE_SKILL;
            Debug.Log($"Ações restantes: {_actionsCurrent}. Escolha nova habilidade ou troque de herói (tab).");
        }
        else
        {
            _isPlayerTurnEnded= true;
        }
    }

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
            StartCoroutine(EndBattle(true));
            return true;
        }

        if (_party.Count == 0)
        {
            StartCoroutine(EndBattle(false));
            return true;
        }

        return false;
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
            Debug.Log($"Herói ativo trocado para: {_activeHero.Name}");
        }
    }

    private void InputFinishTurn()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Espaco no teclado finaliza o turno
        {
            _isPlayerTurnEnded = true;
        }
    }
}
