using System.Collections.Generic;

public class TurnManager
{
    private List<int> _diceValues;
    private List<int> _usedStack;
    private int _currentTurn;

    public int IncrementTurn()
    {
        _currentTurn++;
        _currentTurn %= GameSettings.NumberOfPlayers;
        return _currentTurn;
    }

    public int GetCurrentTurn => _currentTurn;
    
    public List<int> GetDiceValues()
    {
        return _diceValues;
    }
}