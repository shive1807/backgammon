using UnityEngine;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    private readonly List<Dice> _diceList = new ();
    private int _dicesOwner = -1;

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Subscribe<CoreGameMessage.TurnStartDice>(OnTurnStartDice);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.TurnStartDice>(OnTurnStartDice);
    }

    public void SetOwner(int owner)
    {
        _dicesOwner = owner;
    }

    private void OnGameSetup(CoreGameMessage.GameSetup message)
    {
        _diceList.Clear();

        foreach (Transform child in transform)
        {
            var dice = child.GetComponent<Dice>();
            if (dice != null)
            {
                _diceList.Add(dice);
            }
        }
    }

    private void OnTurnStartDice(CoreGameMessage.TurnStartDice message)
    {
        DisableAllDice();
        
        if (message.PlayerIndex == _dicesOwner)
        {
            ShowDice();
        }
    }

    private void DisableAllDice()
    {
        foreach (var dice in _diceList)
        {
            dice.gameObject.SetActive(false);
        }
    }

    private void ShowDice()
    {
        foreach (var dice in _diceList)
        {
            dice.gameObject.SetActive(true);
        }

        ShuffleDiceAndReturnValues();
    }
    
    private void ShuffleDiceAndReturnValues()
    {
        MessageBus.Instance.Publish(new CoreGameMessage.DiceShuffled(new List<int>() { _diceList[0].Roll(), _diceList[1].Roll()}));
    }
}
