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
}
