using UnityEngine;
using Commands;
using System.Collections.Generic;

/// <summary>
/// Command that handles starting a new turn (dice roll + turn setup)
/// </summary>
public class StartTurnCommand : BaseCommand
{
    private readonly int _playerId;
    private RollDiceCommand _diceCommand;
    
    public StartTurnCommand(int playerId) 
        : base($"Start turn for player {playerId}")
    {
        _playerId = playerId;
    }
    
    public override bool CanExecute()
    {
        // Check if services are available
        if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
            return false;
            
        // Check if it's the correct player's turn
        if (GameServices.Instance.TurnManager.GetCurrentTurn != _playerId)
            return false;
            
        return true;
    }
    
    public override bool Execute()
    {
        if (!CanExecute())
            return false;
            
        try
        {
            // Create and execute dice roll command
            _diceCommand = new RollDiceCommand(_playerId);
            if (!_diceCommand.Execute())
            {
                Debug.LogError("Failed to roll dice for turn start");
                return false;
            }
            
            // Publish turn setup message
            MessageBus.Instance.Publish(new CoreGameMessage.TurnDiceSetupAndRoll(_playerId));
            
            Debug.Log($"Turn started for player {_playerId}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error starting turn: {e.Message}");
            return false;
        }
    }
    
    public override bool Undo()
    {
        // Starting a turn typically can't be undone in backgammon
        // But we could reset the dice state if needed
        return false;
    }
    
    public override bool CanUndo()
    {
        return false;
    }
    
    public List<int> GetDiceValues()
    {
        return _diceCommand?.RolledValues ?? new List<int>();
    }
} 