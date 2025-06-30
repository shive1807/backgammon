using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    private List<int> _diceValues;
    private List<int> _usedStack;
    public int GetCurrentTurn { get; private set; }

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.DiceShuffled>(OnDiceShuffled);
        MessageBus.Instance.Subscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Subscribe<CoreGameMessage.SwitchTurn>(OnSwitchTurn);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.DiceShuffled>(OnDiceShuffled);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.SwitchTurn>(OnSwitchTurn);
    }

    private void OnDiceShuffled(CoreGameMessage.DiceShuffled message)
    {
        _diceValues = message.Dice;
    }
    

    private void OnGameSetup(CoreGameMessage.GameSetup message)
    {
        GetCurrentTurn = 0;
    }

    private void OnSwitchTurn(CoreGameMessage.SwitchTurn message)
    {
        GetCurrentTurn++;
        GetCurrentTurn %= GameSettings.NumberOfPlayers;
    }

    public List<int> GetDiceValues()
    {
        return _diceValues;
    }
}