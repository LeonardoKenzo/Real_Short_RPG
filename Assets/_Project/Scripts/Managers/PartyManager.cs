using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [Header("Hero Party")]
    [SerializeField] private List<CharacterRuntimeData> _heroParty;
    [SerializeField] private List<UnitsSO> _heroPartyData;
    [SerializeField] private int _heroPartySize = 0;
    private const int HEROPARTYMAXSIZE = 3;

    [Header("Enemies Party")]
    [SerializeField] private List<CharacterRuntimeData> _enemiesParty;
    [SerializeField] private List<UnitsSO> _enemiesPartyData;
    [SerializeField] private int _enemiesPartySize = 0;
    private const int ENEMIESPARTYMAXSIZE = 5;

    // Getters -------------------------------------
    public List<CharacterRuntimeData> HeroParty => _heroParty;
    public List<UnitsSO> HeroPartyData => _heroPartyData;
    public List<CharacterRuntimeData> EnemiesParty => _enemiesParty;
    public List<UnitsSO> EnemiesPartyData => _enemiesPartyData;

    private void Awake()
    {
        for (int i = 0; i < ENEMIESPARTYMAXSIZE; i++)
        {
            if (_heroParty[i] != null)
                _heroPartyData[i] = _heroParty[i].GetStatsSO();
            if (_enemiesParty[i] != null)
                _enemiesPartyData[i] = _enemiesParty[i].GetStatsSO();
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
