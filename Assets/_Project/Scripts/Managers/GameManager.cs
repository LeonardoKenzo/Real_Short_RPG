using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BattleDataManager _battleDataManager { get; private set; }
    public PartyManager _partyManager { get; private set; }

    [SerializeField] private List<UnitsSO> _enemies;

    // Singleton Pattern -------------------------------------------
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Inicializacao dos managers ---------------------------------
    void Start()
    {
        _partyManager = gameObject.GetComponentInChildren<PartyManager>();
        StartBattle(_enemies, "BattleScene"); // Teste
    }

    // Functions ---------------------------------------------------
    public void StartBattle(List<UnitsSO> enemies, string sceneName)
    {
        _battleDataManager = new BattleDataManager
        {
            _partyData = _partyManager.GetPartyData(),
            _enemiesData = enemies,
            _nextScene = sceneName
        };
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }
}
