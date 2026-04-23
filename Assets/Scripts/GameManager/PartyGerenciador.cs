using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PartyGerenciador : MonoBehaviour
{
    [SerializeField] private List<UnitsSO> enemiesData;

    public List<PersonagemRuntime> CriarHerois(List<Transform> heroSpawn)
    {
        int index = 0;
        foreach (PersonagemRuntime hero in GameGerenciador.Instance.HeroPartyData)
        {
            GameObject heroObj = Instantiate(hero.Prefab, heroSpawn[index]);
            GameGerenciador.Instance.HeroPartyData[index] = heroObj.GetComponent<PersonagemRuntime>();
            index++;
        }
        return GameGerenciador.Instance.HeroPartyData;
    }

    public List<PersonagemRuntime> CriarInimigos(List<Transform> enemiesSpawn)
    {
        int index = 0;
        List<PersonagemRuntime> enemies = new List<PersonagemRuntime>();
        foreach (UnitsSO enemy in enemiesData)
        {
            GameObject enemyObj = Instantiate(enemy.Prefab, enemiesSpawn[index]);
            enemies.Add(enemyObj.GetComponent<PersonagemRuntime>());
            index++;
        }

        return enemies;
    }

}
