using UnityEngine;
using Commands;
using System.Collections.Generic;

/// <summary>
/// Command for rolling dice
/// </summary>
public class RollDiceCommand : BaseCommand
{
    private readonly int _playerId;
    private List<int> _rolledValues;
    private List<int> _previousDiceValues;
    
    public List<int> RolledValues => _rolledValues;
    
    public RollDiceCommand(int playerId) 
        : base($"Roll dice for player {playerId}")
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
            
        // Check if game is in the correct state for dice rolling
        // You might want to add game state validation here
        
        return true;
    }
    
    public override bool Execute()
    {
        if (!CanExecute())
            return false;
            
        try
        {
            // Store previous dice values for undo (if any)
            var gameBoard = GameServices.Instance.GameBoard;
            if (gameBoard != null)
            {
                // You'll need to expose dice values from GameBoard for this to work
                // _previousDiceValues = new List<int>(gameBoard.GetCurrentDiceValues());
            }
            
            // For now, simulate dice roll since the current architecture doesn't expose dice managers properly
            // In a proper implementation, you would have DiceManager references in GameServices
            _rolledValues = new List<int> 
            { 
                Random.Range(1, 7), 
                Random.Range(1, 7) 
            };
            
            // Handle doubles
            if (_rolledValues[0] == _rolledValues[1])
            {
                _rolledValues.Add(_rolledValues[0]);
                _rolledValues.Add(_rolledValues[0]);
            }
            
            // Publish the dice rolled message
            MessageBus.Instance.Publish(new CoreGameMessage.DiceRolled(_rolledValues, _playerId));
            
            Debug.Log($"Player {_playerId} rolled: {string.Join(", ", _rolledValues)}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error executing dice roll command: {e.Message}");
            return false;
        }
    }
    
    public override bool Undo()
    {
        // Dice rolls typically can't be undone in backgammon
        // But you could restore previous dice state if needed for replay purposes
        Debug.LogWarning("Dice roll undo is not typically allowed in backgammon");
        return false;
    }
    
    public override bool CanUndo()
    {
        // Dice rolls are usually not undoable
        return false;
    }
} 