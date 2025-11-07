using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class BattleSystem : MonoBehaviour
{
    // Estados da batalha
    private enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WIN, LOSE }
    private enum PlayerActionState { CHOOSE_SKILL, CHOOSE_TARGET, EXECUTE_ACTION }
 
    [SerializeField] private BattleState _state;
    [SerializeField] private PlayerActionState _playerActionState;

    [SerializeField] private List<CharacterRuntimeData> _party;
    [SerializeField] private List<CharacterRuntimeData> _enemies;

    [Header("Prefabs enemy Spawns")]
    [SerializeField] private GameObject _heroPrefab;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private List<Transform> _heroSpawn;
    [SerializeField] private List<Transform> _enemySpawn;

    [Header("Controle de ações")]
    [SerializeField] private int _indexSkillSelected;
    [SerializeField] private bool _isPlayerSelectionFinished = false;
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
                SwitchHero();
                ChooseSkillSelection();
                break;
            case PlayerActionState.CHOOSE_TARGET:
                ChooseTargetSelection();
                break;
        }
    }

    // Fluxo de Batalha ----------------------------------------------------------------------

    // SO TEM 1 HEROI E 1 INIMIGO MUDAR ISSO DEPOIS PARA SER VARIOS TIPOS DIFERENTES
    private void InitializeBattle()
    {
        _state = BattleState.START;
        
        var battleData = GameManager.Instance._battleDataManager;

        // Instancia todos os herois da cena
        int heroIndex = 0;
        foreach (var hero in battleData._partyData)
        {
            GameObject heroObject = Instantiate(_heroPrefab, _heroSpawn[heroIndex]);
            CharacterRuntimeData runtime = heroObject.GetComponent<CharacterRuntimeData>();
            runtime.InitializeStats(hero);
            _party.Add(runtime);
            heroIndex++;
        }

        // Instancia todos os viloes da cena
        int enemyIndex = 0;
        foreach (var enemy in battleData._enemiesData)
        {
            GameObject enemyObject = Instantiate(_enemyPrefab, _enemySpawn[enemyIndex]);
            CharacterRuntimeData runtime = enemyObject.GetComponent<CharacterRuntimeData>();
            runtime.InitializeStats(enemy);
            _enemies.Add(runtime);
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
        _isPlayerSelectionFinished = false;
        _playerActionState = PlayerActionState.CHOOSE_SKILL;
        _activeHero = _party[0];
        _activeHero.RecoverActions();

        Debug.Log("Seu turno! Escolha uma habilidade (1, 2 ou 3).");

        // Espera até que o jogador conclua a ação
        yield return new WaitUntil(() => _isPlayerSelectionFinished);

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

            // Escolhe herói vivo aleatoriamente
            var livingHeroes = _party.FindAll(hero => (hero != null && hero.HpCurrent > 0));
            if (livingHeroes.Count == 0)
                break;

            var target = livingHeroes[Random.Range(0, livingHeroes.Count)];

            Debug.Log($"{enemy.Name} usa a habilidade \"{enemy.Skills[0].name}\" em {target.Name}! Causou {target.Skills[0].Power} de dano!");
            enemy.UseSkill(0, target);
            enemy.RecoverActions();

            yield return new WaitForSeconds(1.5f);
        }

        // Verifica se ganhou
        if (CheckBattleEnd())
            yield break;

        _state = BattleState.PLAYER_TURN;
        StartCoroutine(PlayerTurn());
    }

    IEnumerator EndBattle(bool isPlayerWon)
    {
        _state = isPlayerWon ? BattleState.WIN : BattleState.LOSE;
        Debug.Log(isPlayerWon ? "Vitória!" : "Derrota...");

        yield return new WaitForSeconds(2f);
    }

    // Funcoes Auxilares para as Batalhas -------------------------------------------------------

    // Selecao de Habilidades e ataque do player -----------------------------------------------
    private void ChooseSkillSelection()
    {
        if(_activeHero.ActionsCurrent <= 0)
        {
            Debug.Log("Não há mais ações restantes!");
            _isPlayerSelectionFinished = true;
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

        _indexSkillSelected = index;
        Debug.Log($"Habilidade {index + 1}: escolhida! Agora selecione o alvo (1, 2, 3...).");
        _playerActionState = PlayerActionState.CHOOSE_TARGET;
    }

    private void ChooseTargetSelection()
    {
        for(int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i] == null)
                return;

            if(Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _targetSelected = _enemies[i];
                Debug.Log($"Alvo {_targetSelected.Name} selecionado!");
                StartCoroutine(ExecuteAction());
            }
        }
    }

    private IEnumerator ExecuteAction()
    {
        _playerActionState = PlayerActionState.EXECUTE_ACTION;
 
        Debug.Log($"{_activeHero.Name} usa habilidade \"{_activeHero.Skills[_indexSkillSelected].name}\" em {_targetSelected.Name}! Causou {_activeHero.Skills[_indexSkillSelected].Power} de dano!");

        // Usa a habilidade
        _activeHero.UseSkill(_indexSkillSelected, _targetSelected);

        yield return new WaitForSeconds(2f);

        // Marca o turno do jogador como finalizado
        if(_activeHero.ActionsCurrent > 0)
        {
            _playerActionState = PlayerActionState.CHOOSE_SKILL;
            Debug.Log($"Ações restantes: {_activeHero.ActionsCurrent}. Escolha nova habilidade ou troque de herói (tab).");
        }
        else
            _isPlayerSelectionFinished = true;
    }

    // Verificar fim de batalha -------------------------------------------------
    private bool CheckBattleEnd()
    {
        // Remove mortos
        _party.RemoveAll(hero => (hero == null || hero.HpCurrent <= 0));
        _enemies.RemoveAll((enemy => enemy == null || enemy.HpCurrent <= 0));

        if (_enemies.Count == 0)
        {
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

    // Troca de heroi ------------------------------------------------------
    private void SwitchHero()
    {
        // Seleciona o proximo heroi por tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            int currentIndex = _party.IndexOf(_activeHero);
            int nextIndex = (currentIndex + 1) % _party.Count;
            _activeHero = _party[nextIndex];
            Debug.Log($"Herói ativo trocado para: {_activeHero.Name}");
        }
        
        /* Ou seleciona os herois por 1, 2 ou 3
        for(int i = 0; i < _party.Count; i++) 
        {
            if(Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _activeHero = _party[i];
                Debug.Log($"Herói ativo trocado para: {_activeHero.Name}");
            }
        }
        */
    }
}
