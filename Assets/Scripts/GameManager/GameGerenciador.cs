using System.Collections.Generic;
using UnityEngine;

public class GameGerenciador : MonoBehaviour
{
    public static GameGerenciador Instance { get; private set; }
    public List<PersonagemRuntime> HeroPartyData { get; private set; }

    // Singleton Pattern -------------------------------------------
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Guarda os dados dos herois entre cenas
    public void InitializeParty(List<UnitsSO> SO)
    {
        if (HeroPartyData == null || HeroPartyData.Count <= 0)
        {
            HeroPartyData = new List<PersonagemRuntime>();
            foreach (UnitsSO unit in SO)
            {
                HeroPartyData.Add(new PersonagemRuntime(unit));
            }
        }
    }
}
