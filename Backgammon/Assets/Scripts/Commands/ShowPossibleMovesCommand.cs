using UnityEngine;
using Commands;
using System.Collections.Generic;
using System.Linq;

namespace Commands
{
    /// <summary>
    /// Command for showing rings that indicate possible moves for a selected coin
    /// </summary>
    public class ShowPossibleMovesCommand : BaseCommand
{
    private readonly int _sourceTowerIndex;
    private readonly int _playerId;
    private readonly List<int> _diceValues;
    
    // State for undo - track which towers had rings added
    private readonly List<int> _targetTowersWithRings;
    private bool _ringsCurrentlyShown;
    
    public ShowPossibleMovesCommand(int sourceTowerIndex, int playerId, List<int> diceValues)
        : base($"Show possible moves from tower {sourceTowerIndex} for player {playerId}", false) // false = visual command, not game state
    {
        _sourceTowerIndex = sourceTowerIndex;
        _playerId = playerId;
        _diceValues = new List<int>(diceValues ?? new List<int>());
        _targetTowersWithRings = new List<int>();
        _ringsCurrentlyShown = false;
    }
    
    /// <summary>
    /// Create a command that cleans existing rings and shows new possible moves
    /// This is the recommended way to show moves to avoid ring conflicts
    /// </summary>
    public static ShowPossibleMovesCommand CreateCleanAndShow(int sourceTowerIndex, int playerId, List<int> diceValues)
    {
        return new ShowPossibleMovesCommand(sourceTowerIndex, playerId, diceValues);
    }
    
    public override bool CanExecute()
    {
        // Validate basic parameters
        if (_playerId < 0 || _playerId >= GameSettings.NumberOfPlayers)
            return false;
            
        // Check if services are available
        if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
            return false;
            
        // Check if source tower exists
        var sourceTower = GameServices.Instance.GetTowerByIndex(_sourceTowerIndex);
        if (sourceTower == null)
            return false;
            
        // Check if source tower is owned by the player and has coins
        if (sourceTower.CoinsCount == 0 || !sourceTower.IsOwnedBy(_playerId))
            return false;
            
        // Must have dice values to show moves
        if (_diceValues == null || _diceValues.Count == 0)
            return false;
            
        return true;
    }
    
    public override bool Execute()
    {
        if (!CanExecute())
            return false;
            
        try
        {
            // Clear any existing rings first
            MessageBus.Instance.Publish(new CoreGameMessage.CleanTowerRings());
            _targetTowersWithRings.Clear();
            
            var gameBoard = GameServices.Instance.GameBoard;
            if (gameBoard == null || gameBoard.towers == null)
                return false;
                
            // Calculate and show rings for possible moves
            var runOnce = _diceValues.Distinct().Count() != _diceValues.Count();
            
            foreach (var diceValue in _diceValues)
            {
                var targetTowerIndex = CalculateTargetTowerIndex(_sourceTowerIndex, diceValue, _playerId);
                
                // Check if target is within board bounds
                if (targetTowerIndex < 0 || targetTowerIndex >= gameBoard.towers.Count)
                    continue;
                
                var targetTower = gameBoard.towers[targetTowerIndex];
                if (targetTower == null)
                    continue;
                
                // Validate the move using the same logic as MoveCoinCommand
                if (IsValidMove(targetTower))
                {
                    targetTower.AddRing(_playerId, _sourceTowerIndex, targetTowerIndex);
                    
                    // Track this tower for undo functionality
                    if (!_targetTowersWithRings.Contains(targetTowerIndex))
                        _targetTowersWithRings.Add(targetTowerIndex);
                }
                
                // If we have duplicate dice values, only show rings once
                if (runOnce) 
                    break;
            }
            
            _ringsCurrentlyShown = true;
            Debug.Log($"Showed possible moves from tower {_sourceTowerIndex} for player {_playerId}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error executing ShowPossibleMovesCommand: {e.Message}");
            return false;
        }
    }
    
    public override bool Undo()
    {
        if (!_ringsCurrentlyShown)
            return false;
            
        try
        {
            // Clean all rings (this will remove rings from all towers)
            MessageBus.Instance.Publish(new CoreGameMessage.CleanTowerRings());
            
            _targetTowersWithRings.Clear();
            _ringsCurrentlyShown = false;
            
            Debug.Log($"Hidden possible moves from tower {_sourceTowerIndex} for player {_playerId}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error undoing ShowPossibleMovesCommand: {e.Message}");
            return false;
        }
    }
    
    public override bool CanUndo()
    {
        return _ringsCurrentlyShown;
    }
    
    /// <summary>
    /// Calculate the target tower index based on source, dice value, and player direction
    /// </summary>
    private int CalculateTargetTowerIndex(int sourceTowerIndex, int diceValue, int playerId)
    {
        // Player 0 (white) moves in decreasing direction, Player 1 (black) moves in increasing direction
        return playerId == 0 ? sourceTowerIndex - diceValue : sourceTowerIndex + diceValue;
    }
    
    /// <summary>
    /// Check if a move to the target tower is valid (same logic as MoveCoinCommand)
    /// </summary>
    private bool IsValidMove(Tower targetTower)
    {
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

/// <summary>
/// Command for hiding all visible rings (possible move indicators)
/// </summary>
public class HidePossibleMovesCommand : BaseCommand
{
    public HidePossibleMovesCommand() : base("Hide all possible move rings", false) // false = visual command, not game state
    {
    }
    
    public override bool CanExecute()
    {
        return true; // Can always try to clean rings
    }
    
    public override bool Execute()
    {
        try
        {
            MessageBus.Instance.Publish(new CoreGameMessage.CleanTowerRings());
            Debug.Log("Hidden all possible move rings");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error hiding rings: {e.Message}");
            return false;
        }
    }
    
    public override bool Undo()
    {
        // Cannot undo hiding rings - we don't know what was there before
        return false;
    }
    
    public override bool CanUndo()
    {
        return false;
    }
}
} 