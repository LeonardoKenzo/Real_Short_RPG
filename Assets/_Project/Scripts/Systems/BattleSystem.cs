using System.Collections;
using UnityEngine;


public class BattleSystem : MonoBehaviour
{
    // Estados da batalha
    [SerializeField] private enum BattleState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    
    [SerializeField] private BattleState _state;

    void Start()
    {
        _state = BattleState.START;

        // Algum texto ou animacao para comecar a batalha (textos de dialogo etc), use a coroutine abaixo;
        StartCoroutine(startBattle());

    }

    IEnumerator startBattle()
    {
        // Texto de dialogo, efeitos, etc antes da batalha

        yield return new WaitForSeconds(2f);

        _state = BattleState.PLAYERTURN;
    }
}
