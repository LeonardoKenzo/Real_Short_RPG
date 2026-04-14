using System.Collections.Generic;
using UnityEngine;

/*
 * "Struct" que contem as informacoes da batalha, varios 
 * podem ser criados para cada batalha, usado pelo GameManager
 * 
 * ------------------------------------------------------------
 * Como usar:
 * 1) Crie uma variavel do tipo BattleDataManager {get, private set} 
 *    em qualquer script e comece a usa-lo
 * 2) Precisa receber duas listas de UnitsSO
 */
public class BattleDataManager
{
    public List<UnitsSO> _partyData;
    public List<UnitsSO> _enemiesData;
    public string _nextScene;
}
