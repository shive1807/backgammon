using UnityEngine;
using Commands;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Command for checking if a turn should end and triggering turn over if needed
/// Now supports manual turn ending when player presses Done button
/// </summary>
public class CheckTurnEndCommand : BaseCommand
{
    private readonly int _playerId;
    private readonly List<int> _remainingDiceValues;
    private readonly bool _isManualTurnEnd;
    
    public CheckTurnEndCommand(int playerId, List<int> remainingDiceValues, bool isManualTurnEnd = false)
        : base($"Check turn end for player {playerId}" + (isManualTurnEnd ? " (manual)" : ""), false) // false = turn management, not game state move
    {
        _playerId = playerId;
        _remainingDiceValues = new List<int>(remainingDiceValues ?? new List<int>());
        _isManualTurnEnd = isManualTurnEnd;
    }
    
    public override bool CanExecute()
    {
        // Can always check if turn should end
        return true;
    }
    
    public override bool Execute()
    {
        try
        {
            // If this is a manual turn end (Done button pressed), validate it's legal
            if (_isManualTurnEnd)
            {
                // Check if no dice values left
                if (_remainingDiceValues.Count == 0)
                {
                    Debug.Log($"Turn ended manually for player {_playerId}: All dice used");
                    MessageBus.Instance.Publish(new CoreGameMessage.TurnOver());
                    return true;
                }
                
                // Check if no valid moves available with remaining dice
                int manualAvailableActions = CountAvailableActions();
                if (manualAvailableActions == 0)
                {
                    Debug.Log($"Turn ended manually for player {_playerId}: No valid moves available with dice [{string.Join(", ", _remainingDiceValues)}]");
                    MessageBus.Instance.Publish(new CoreGameMessage.TurnOver());
                    return true;
                }
                
                // If there are valid moves remaining, don't allow manual turn end
                Debug.Log($"Cannot end turn manually for player {_playerId}: {manualAvailableActions} valid moves available with dice [{string.Join(", ", _remainingDiceValues)}]");
                return false;
            }
            
            // For automatic checking (if still used elsewhere), keep original logic
            // Check if no dice values left
            if (_remainingDiceValues.Count == 0)
            {
                Debug.Log($"Turn ended for player {_playerId}: No dice values remaining");
                MessageBus.Instance.Publish(new CoreGameMessage.TurnOver());
                return true;
            }
            
            // Check if no valid moves available with remaining dice
            int availableActions = CountAvailableActions();
            if (availableActions == 0)
            {
                Debug.Log($"Turn ended for player {_playerId}: No valid moves available with dice [{string.Join(", ", _remainingDiceValues)}]");
                MessageBus.Instance.Publish(new CoreGameMessage.TurnOver());
                return true;
            }
            
            Debug.Log($"Turn continues for player {_playerId}: {availableActions} actions available with dice [{string.Join(", ", _remainingDiceValues)}]");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error executing CheckTurnEndCommand: {e.Message}");
            return false;
        }
    }
    
    public override bool Undo()
    {
        // Cannot undo turn end checking
        return false;
    }
    
    public override bool CanUndo()
    {
        return false;
    }
    
    /// <summary>
    /// Count available actions for the player with remaining dice values
    /// </summary>
    private int CountAvailableActions()
    {
        if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
            return 0;
            
        var gameBoard = GameServices.Instance.GameBoard;
        if (gameBoard == null || gameBoard.towers == null)
            return 0;
            
        int availableActions = 0;
        
        // Check spawn points first (priority moves)
        var spawnTower = GameServices.Instance.GetSpawnTower(_playerId);
        if (spawnTower != null && spawnTower.CoinsCount > 0)
        {
            // If coins are in spawn, they must be moved first
            foreach (var diceValue in _remainingDiceValues)
            {
                int targetIndex = CalculateTargetIndex(-1, diceValue, _playerId); // -1 for spawn
                if (IsValidMove(targetIndex, gameBoard))
                {
                    availableActions++;
                }
            }
            return availableActions;
        }
        
        // Check regular board moves
        foreach (var tower in gameBoard.towers.Where(t => t.IsOwnedBy(_playerId)))
        {
            foreach (var diceValue in _remainingDiceValues)
            {
                int targetIndex = CalculateTargetIndex(tower.TowerIndex, diceValue, _playerId);
                if (IsValidMove(targetIndex, gameBoard))
                {
                    availableActions++;
                }
            }
        }
        
        return availableActions;
    }
    
    /// <summary>
    /// Calculate target index based on player direction and dice value
    /// </summary>
    private int CalculateTargetIndex(int sourceIndex, int diceValue, int playerId)
    {
        // Handle spawn point moves
        if (sourceIndex == -1)
        {
            return playerId == 0 ? 24 - diceValue : diceValue - 1;
        }
        
        // Regular board moves
        return playerId == 0 ? sourceIndex - diceValue : sourceIndex + diceValue;
    }
    
    /// <summary>
    /// Check if a move to the target index is valid
    /// </summary>
    private bool IsValidMove(int targetIndex, GameBoard gameBoard)
    {
        // Check bounds
        if (targetIndex < 0 || targetIndex >= gameBoard.towers.Count)
            return false;
            
        var targetTower = gameBoard.towers[targetIndex];
        if (targetTower == null)
            return false;
            
        // Can move to empty tower
        if (targetTower.IsEmpty())
            return true;
            
        // Can move to own tower
        if (targetTower.IsOwnedBy(_playerId))
            return true;
            
        // Can attack single opponent coin
        if (targetTower.CanAttack() && !targetTower.IsOwnedBy(_playerId))
            return true;
            
        return false;
    }
}