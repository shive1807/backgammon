using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private List<int> _diceValues;
    private List<int> _usedStack;
    private int _currentTurn;

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.DiceShuffled>(OnDiceShuffled);
        MessageBus.Instance.Subscribe<CoreGameMessage.OnCheckerMoved>(OnCheckerMoved);
        MessageBus.Instance.Subscribe<CoreGameMessage.GameSetup>(OnGameSetup);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.DiceShuffled>(OnDiceShuffled);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.OnCheckerMoved>(OnCheckerMoved);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.GameSetup>(OnGameSetup);
    }

    private void OnDiceShuffled(CoreGameMessage.DiceShuffled message)
    {
        _diceValues = message.Dice;
    }
    
    private void OnCheckerMoved(CoreGameMessage.OnCheckerMoved message)
    {
        _diceValues.Remove(message.CheckerMovedByDiceValue);
    }

    private void OnGameSetup(CoreGameMessage.GameSetup message)
    {
        _currentTurn = 0;
    }


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