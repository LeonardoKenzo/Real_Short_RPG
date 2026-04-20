using System.Collections.Generic;
using UnityEngine;

/*
 * O Script eh uma instancia global que nao eh destruido entre as scenes, ele armazena
 * as informacoes da party de herois e inimigos alem de ser responsavel por gerenciar todas
 * as batalhas durante o jogo
 * 
 * -------------------------------------------------------------------------------------
 * Como usar:
 * 1) Use "GameManager.Instance" para referenciar o GameManager em qualquer codigo
 * 
 * Aviso!!
 * NAO CRIE 2 GAME MANAGERS!! JA EXISTE UM NA CENA INICIAL, PARA TESTAR CRIE O OBJETO E APAGUE DEPOIS!!!
 */

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BattleDataManager _battleDataManager { get; private set; }
    public PartyManager _partyManager { get; private set; }

    // Singleton Pattern -------------------------------------------
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Inicializacao dos managers ---------------------------------
        _partyManager = gameObject.GetComponentInChildren<PartyManager>();

        StartBattle("BattleScene"); // Usar essa linha somente para testes dentro das cenas de batalha
    }

    // Functions ---------------------------------------------------
    public void StartBattle(string sceneName)
    {
        Debug.Log("AAAAAAAA");
        _battleDataManager = new BattleDataManager
        {
            _partyData = _partyManager.HeroPartyData,
            _enemiesData = _partyManager.EnemiesPartyData,
            _nextScene = sceneName
        };
    }
}
