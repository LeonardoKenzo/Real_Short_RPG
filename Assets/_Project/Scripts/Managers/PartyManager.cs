using System.Collections.Generic;
using UnityEngine;

/*
 * Gerenciador os grupos que irao para a luta, tanto herois quanto inimigos
 * 
 * ---------------------------------------------------------------------------
 * Como usar:
 * 1) Se este script não estiver anexado ao filho do GameManager, anexe-o
 * 2) Adicione manualmente (vamos mudar isso posteriormente) os 
 *    CharacterRuntimeData.cs de todos os personagens da party
 * 
 * ----------------------------------------------------------------------------
 * Dependencias:
 * - CharacterRuntimeData (dos inimigos e herois)
*/
public class PartyManager : MonoBehaviour
{

    [Header("Hero Party")]
    [SerializeField] private List<CharacterRuntimeData> _heroParty;
    [SerializeField] private List<UnitsSO> _heroPartyData; 
    private int _heroPartySize = 0;
    private const int HEROPARTYMAXSIZE = 3;

    [Header("Enemies Party")]
    [SerializeField] private List<CharacterRuntimeData> _enemiesParty;
    [SerializeField] private List<UnitsSO> _enemiesPartyData;
    private int _enemiesPartySize = 0;
    private const int ENEMIESPARTYMAXSIZE = 5;

    // Getters -------------------------------------
    public List<CharacterRuntimeData> HeroParty => _heroParty;
    public List<UnitsSO> HeroPartyData => _heroPartyData;
    public List<CharacterRuntimeData> EnemiesParty => _enemiesParty;
    public List<UnitsSO> EnemiesPartyData => _enemiesPartyData;

    // Initialize the stats of the characters
    private void Awake()
    {
        for (int i = 0; i < _heroParty.Count; i++)
        {
            if (_heroParty[i] != null)
            {
                _heroPartyData.Add(_heroParty[i].Stats);
                _heroPartySize++;
            }
        }
        for(int i = 0; i < _enemiesParty.Count; i++)
        {
            if (_enemiesParty[i] != null)
            {
                _enemiesPartyData.Add(_enemiesParty[i].Stats);
                _enemiesPartySize++;
            }
        }
    }

    // Functions -----------------------------------
    public bool AddHero(CharacterRuntimeData hero)
    {
        if (hero != null && _heroPartySize < HEROPARTYMAXSIZE)
        {
            _heroParty.Add(hero);
            _heroPartySize++;
            _heroPartyData.Add(hero.Stats);
            return true;
        }
        return false;
    }

    public bool RemoveHero(CharacterRuntimeData hero)
    {
        if (hero != null && _heroPartySize > 0)
        {
            _heroParty.Remove(hero);
            _heroPartySize--;
            _heroPartyData.Remove(hero.Stats);
            return true;
        }
        return false;
    }

    public bool AddEnemy(CharacterRuntimeData enemy)
    {
        if (enemy != null && _enemiesPartySize < ENEMIESPARTYMAXSIZE)
        {
            _enemiesParty.Add(enemy);
            _enemiesPartySize++;
            _enemiesPartyData.Add(enemy.Stats);
            return true;
        }
        return false;
    }

    public bool RemoveEnemy(CharacterRuntimeData enemy)
    {
        if (enemy != null && _enemiesPartySize > 0)
        {
            _enemiesParty.Remove(enemy);
            _enemiesPartySize--;
            _enemiesPartyData.Remove(enemy.Stats);
            return true;
        }
        return false;
    }

}
