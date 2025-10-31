using Unity.VisualScripting;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private CharacterRuntimeData[] _party;
    [SerializeField] private int _partyMaxSize = 3;

    // Getters -------------------------------------
    public CharacterRuntimeData[] Party => _party;
    public int PartyMaxSize => _partyMaxSize;

    // Functions -----------------------------------
    private void Start()
    {

    }

    public void AddHeroParty(CharacterRuntimeData hero)
    {

    }

}
