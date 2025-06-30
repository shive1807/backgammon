using UnityEngine;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    private readonly List<Dice> _diceList = new ();
    
    [SerializeField, Tooltip("ID of the player who owns these dice. Use -1 for unassigned.")]
    private int dicesOwner = -1;

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Subscribe<CoreGameMessage.TurnStartDiceRolled>(OnTurnStartDice);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.TurnStartDiceRolled>(OnTurnStartDice);
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

    private void OnTurnStartDice(CoreGameMessage.TurnStartDiceRolled message)
    {
        DisableAllDice();
        
        if (message.PlayerIndex == dicesOwner)
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
        MessageBus.Instance.Publish(new CoreGameMessage.DiceShuffled(new List<int>() { _diceList[0].Roll(), _diceList[1].Roll()}, dicesOwner));
    }
}
