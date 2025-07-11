using System;
using TMPro;
using UnityEngine;

public class TextFeedback : MonoBehaviour
{
    private TextMeshProUGUI _feedbackText;

    private void OnEnable()
    {
        _feedbackText = GetComponent<TextMeshProUGUI>();
        ResetText();
        
        MessageBus.Instance.Subscribe<CoreGameMessage.TurnDiceSetupAndRoll>(OnTurnStartDice);
        MessageBus.Instance.Subscribe<CoreGameMessage.DiceRolled>(OnDiceRolled);
        MessageBus.Instance.Subscribe<CoreGameMessage.OnCoinMoved>(OnCoinMoved);
        MessageBus.Instance.Subscribe<CoreGameMessage.SwitchTurn>(OnSwitchTurn);
        MessageBus.Instance.Subscribe<CoreGameMessage.DiceValueRestored>(OnDiceValueRestored);
    }
    
    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.TurnDiceSetupAndRoll>(OnTurnStartDice);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.DiceRolled>(OnDiceRolled);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.OnCoinMoved>(OnCoinMoved);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.SwitchTurn>(OnSwitchTurn);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.DiceValueRestored>(OnDiceValueRestored);
    }

    private void ResetText()
    {
        _feedbackText.text = "";
    }

    private void OnTurnStartDice(CoreGameMessage.TurnDiceSetupAndRoll message)
    {
        ResetText();
        _feedbackText.text = GameManager.Instance.GetTurnManager().GetCurrentTurn == 0 ? "White To Move" : "Black To Move";
    }
    
    private void OnDiceRolled(CoreGameMessage.DiceRolled message)
    {
        string playerName = message.CurrentPlayerIndex == 0 ? "White" : "Black";
        string diceText = string.Join(", ", message.Dice);
        _feedbackText.text = $"{playerName} rolled: {diceText}\nMust use all dice values";
    }
    
    private void OnCoinMoved(CoreGameMessage.OnCoinMoved message)
    {
        // Get remaining dice values
        var remainingDice = GetRemainingDiceValues();
        
        if (remainingDice.Count > 0)
        {
            string playerName = GameManager.Instance.GetTurnManager().GetCurrentTurn == 0 ? "White" : "Black";
            string diceText = string.Join(", ", remainingDice);
            _feedbackText.text = $"{playerName} remaining dice: {diceText}\nMust use all dice values";
        }
        else
        {
            string playerName = GameManager.Instance.GetTurnManager().GetCurrentTurn == 0 ? "White" : "Black";
            _feedbackText.text = $"{playerName} - All dice used!\nPress Done to end turn";
        }
    }
    
    private void OnSwitchTurn(CoreGameMessage.SwitchTurn message)
    {
        ResetText();
    }
    
    private void OnDiceValueRestored(CoreGameMessage.DiceValueRestored message)
    {
        // Update feedback when dice values are restored (e.g., after undo)
        var remainingDice = GetRemainingDiceValues();
        
        if (remainingDice.Count > 0)
        {
            string playerName = message.CurrentPlayerIndex == 0 ? "White" : "Black";
            string diceText = string.Join(", ", remainingDice);
            _feedbackText.text = $"{playerName} dice restored: {diceText}\nMust use all dice values";
        }
    }
    
    private System.Collections.Generic.List<int> GetRemainingDiceValues()
    {
        if (GameServices.Instance?.GameBoard != null)
        {
            return GameServices.Instance.GameBoard.GetRemainingDiceValues();
        }
        return new System.Collections.Generic.List<int>();
    }
}
