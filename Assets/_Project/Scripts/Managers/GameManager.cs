using System.Collections.Generic;
using UnityEngine;

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
        StartBattle("BattleScene");
    }

    // Functions ---------------------------------------------------
    public void StartBattle(string sceneName)
    {
        _battleDataManager = new BattleDataManager
        {
            _partyData = _partyManager.HeroPartyData,
            _enemiesData = _partyManager.EnemiesPartyData,
            _nextScene = sceneName
        };
    }
}
