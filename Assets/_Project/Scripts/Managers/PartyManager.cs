using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private List<CharacterRuntimeData> _party;
    [SerializeField] private List<UnitsSO> _partyData;
    [SerializeField] private int _partySize = 0;
    private const int PARTYMAXSIZE = 3;

    // Getters -------------------------------------
    public List<CharacterRuntimeData> Party => _party;

    // Functions -----------------------------------
    public bool AddHero(CharacterRuntimeData hero)
    {
        if (hero != null && _partySize < PARTYMAXSIZE)
        {
            _party.Add(hero);
            _partySize++;
            _partyData.Add(hero.Stats);
            return true;
        }
        return false;
    }

    public bool RemoveHero(CharacterRuntimeData hero)
    {
        if (hero != null && _partySize > 0)
        {
            _party.Remove(hero);
            _partySize--;
            _partyData.Remove(hero.Stats);
            return true;
        }
        return false;
    }

    public List<UnitsSO> GetPartyData()
    {
        return _partyData;
    }

}
