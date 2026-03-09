using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class BattleSystem : MonoBehaviour
{
    private enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WIN, LOSE }
    private enum PlayerActionState { CHOOSE_SKILL, CHOOSE_TARGET, EXECUTE_ACTION }
 
    [SerializeField] private BattleState _state;
    [SerializeField] private PlayerActionState _playerActionState;
    
    [SerializeField] private BattleUI _battleUI;

    [SerializeField] private List<CharacterRuntimeData> _party;
    [SerializeField] private List<CharacterRuntimeData> _enemies;

    [Header("Prefabs enemy Spawns")]
    [SerializeField] private GameObject _heroPrefab;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private List<Transform> _heroSpawn;
    [SerializeField] private List<Transform> _enemySpawn;

    [Header("Controle de ações")]
    [SerializeField] private int _indexSkillSelected;
    [SerializeField] private bool _isPlayerTurnEnded = false;
    [SerializeField] private CharacterRuntimeData _targetSelected;
    [SerializeField] private CharacterRuntimeData _activeHero;

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
                ChooseTargetSelection();
                break;
        }
    }

    // Fluxo de Batalha ----------------------------------------------------------------------

    private void InitializeBattle()
    {
        _state = BattleState.START;
        
        var battleData = GameManager.Instance._battleDataManager;

        // Instancia todos os herois da cena
        int heroIndex = 0;
        foreach (var hero in battleData._partyData)
        {
            GameObject heroObject = Instantiate(_heroPrefab, _heroSpawn[heroIndex]);
            CharacterRuntimeData runtimeHero = heroObject.GetComponent<CharacterRuntimeData>();
            runtimeHero.InitializeStats(hero);
            _party.Add(runtimeHero);
            heroIndex++;
        }

        // Instancia todos os viloes da cena
        int enemyIndex = 0;
        foreach (var enemy in battleData._enemiesData)
        {
            GameObject enemyObject = Instantiate(_enemyPrefab, _enemySpawn[enemyIndex]);
            CharacterRuntimeData runtimeEnemy = enemyObject.GetComponent<CharacterRuntimeData>();
            runtimeEnemy.InitializeStats(enemy);
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
        _isPlayerTurnEnded = false;
        _playerActionState = PlayerActionState.CHOOSE_SKILL;
        for (int i = 0; i < _party.Count; i++)
        {
            _party[i].RecoverActions();
        }
        _activeHero = _party[0];

        // Atualiza a UI
        _activeHero.IsSelected(true);
        _battleUI.UpdateCursorPosition(_activeHero.ActionsCurrent);
        _battleUI.UpdateSkillCards(_activeHero.GetSkillsImages());

        Debug.Log("Seu turno! Escolha uma habilidade (1, 2 ou 3).");

        // Espera até que o jogador conclua a ação
        yield return new WaitUntil(() => _isPlayerTurnEnded);

        // Verifica se ganhou
        if (CheckBattleEnd())
            yield break;

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

            var target = livingHeroes[Random.Range(0, livingHeroes.Count)];
            target.IsSelected(true);

            Debug.Log($"{enemy.Name} usa a habilidade \"{enemy.Skills[0].name}\" em {target.Name}! Causou {target.Skills[0].Power} de dano!");
            enemy.UseSkill(0, target); // Fazer mecânica de diferentes skills do inimigo
            enemy.RecoverActions();

            yield return new WaitForSeconds(1.5f);

            enemy.IsSelected(false);
            target.IsSelected(false);
        }

        // Verifica se ganhou
        if (CheckBattleEnd())
            yield break;

        _state = BattleState.PLAYER_TURN;
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

    // Selecao de Habilidades e ataque do player -----------------------------------------------
    private void ChooseSkillSelection()
    {
        var _actionsRemaining = _party.FindAll(hero => (hero.ActionsCurrent > 0));
        if(_actionsRemaining.Count == 0)
        {
            Debug.Log("Não há mais ações restantes!");
            _isPlayerTurnEnded = true;
            return;
        }
  
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChooseSkill(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChooseSkill(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChooseSkill(2);
    }

    private void ChooseSkill(int index)
    {
        if (_activeHero.Skills[index].ActionCost > _activeHero.ActionsCurrent)
        {
            Debug.Log("Ações insuficientes para usar a habilidade.");
            return;
        }
        _battleUI.MoveDownSmooth(_indexSkillSelected, 112, 0.5f);

        _indexSkillSelected = index;
        _battleUI.MoveUpSmooth(index, 112f, 0.5f);
        Debug.Log($"Habilidade {index + 1}: escolhida! Agora selecione o alvo (1, 2, 3).");
        _playerActionState = PlayerActionState.CHOOSE_TARGET;
    }

    private void ChooseTargetSelection()
    {
        var effects = _activeHero.Skills[_indexSkillSelected].Effect;
        if (effects.Contains(SkillsSO.SkillEffects.ATTACK) ||
            effects.Contains(SkillsSO.SkillEffects.DEBUFF) ||
            effects.Contains(SkillsSO.SkillEffects.STUN))
            SelectEnemy();
        else if (effects.Contains(SkillsSO.SkillEffects.BUFF) ||
            effects.Contains(SkillsSO.SkillEffects.HEAL) ||
            effects.Contains(SkillsSO.SkillEffects.SHIELD))
            SelectHero();
    }

    private IEnumerator ExecuteAction()
    {
        _playerActionState = PlayerActionState.EXECUTE_ACTION;
 
        Debug.Log($"{_activeHero.Name} usa habilidade \"{_activeHero.Skills[_indexSkillSelected].name}\" em {_targetSelected.Name}!");

        // Usa a habilidade
        _activeHero.UseSkill(_indexSkillSelected, _targetSelected);
        _battleUI.UpdateCursorPosition(_activeHero.ActionsCurrent);

        yield return new WaitForSeconds(2f);

        _battleUI.MoveDownSmooth(_indexSkillSelected, 112f, 0.5f);

        yield return new WaitForSeconds(1f);

        // Marca o turno do jogador como finalizado
        _targetSelected.IsSelected(false);
        CheckBattleEnd();
        if (_activeHero.ActionsCurrent > 0)
        {
            _playerActionState = PlayerActionState.CHOOSE_SKILL;
            Debug.Log($"Ações restantes: {_activeHero.ActionsCurrent}. Escolha nova habilidade ou troque de herói (tab).");
        }
        else
        {
            ChangeHero();
            _playerActionState = PlayerActionState.CHOOSE_SKILL;
        }
    }

    private void SelectEnemy()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && _enemies[i] != null)
            {
                _targetSelected = _enemies[i];
                Debug.Log($"Alvo {_targetSelected.Name} selecionado!");
                _targetSelected.IsSelected(true);
                StartCoroutine(ExecuteAction());
            }
        }
    }

    private void SelectHero()
    {
        for (int i = 0; i < _party.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _targetSelected = _party[i];
                Debug.Log($"Alvo {_targetSelected.Name} selecionado!");
                _targetSelected.IsSelected(true);
                StartCoroutine(ExecuteAction());
            }
        }
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
        // Seleciona o proximo heroi por tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            int currentIndex = _party.IndexOf(_activeHero);
            int nextIndex = (currentIndex + 1) % _party.Count;
            _activeHero.IsSelected(false);

            _activeHero = _party[nextIndex];
            _activeHero.IsSelected(true);
            _battleUI.UpdateCursorPosition(_activeHero.ActionsCurrent);
            Debug.Log($"Herói ativo trocado para: {_activeHero.Name}");
        }
    }

    private void InputFinishTurn()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _state = BattleState.ENEMY_TURN;
            Debug.Log("O jogador encerrou o turno");
            StartCoroutine(EnemyTurn());
        }
    }

    // Troca para o proximo heroi
    private void ChangeHero()
    {
        int currentIndex = _party.IndexOf(_activeHero);
        int nextIndex = (currentIndex + 1) % _party.Count;
        _activeHero.IsSelected(false);

        // Realiza a troca
        _activeHero = _party[nextIndex];
        _activeHero.IsSelected(true);

        // Atualiza toda a UI
        _battleUI.UpdateCursorPosition(_activeHero.ActionsCurrent);
        _battleUI.UpdateSkillCards(_activeHero.GetSkillsImages());
        Debug.Log($"Herói ativo trocado para: {_activeHero.Name}");
    }
}
