using UnityEngine;

public class TurnManager
{
    private int _turnCount;

    // Construtor do Manager -------------
    public TurnManager()
    {
        _turnCount = 1;
    }

    // Funcoes ---------------------------
    public void PassTurn()
    {
        _turnCount += 1;
    }

    public void ResetTurn()
    {
        _turnCount = 1;
    }

    public int GetTurnCount()
    {
        return _turnCount;
    }
}
