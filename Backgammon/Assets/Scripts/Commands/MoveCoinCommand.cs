using UnityEngine;
using Commands;

/// <summary>
/// Command for moving a coin from one tower to another
/// </summary>
public class MoveCoinCommand : BaseCommand
{
    private readonly int _sourceIndex;
    private readonly int _targetIndex;
    private readonly int _playerId;
    private readonly int _diceValue;
    
    // State for undo
    private Coin _movedCoin;
    private Coin _attackedCoin;
    private bool _wasAttack;
    private Tower _sourceTower;
    private Tower _targetTower;
    private Tower _spawnTower;
    
    public MoveCoinCommand(int sourceIndex, int targetIndex, int playerId, int diceValue)
        : base($"Move coin from tower {sourceIndex} to tower {targetIndex} (dice: {diceValue})")
    {
        _sourceIndex = sourceIndex;
        _targetIndex = targetIndex;
        _playerId = playerId;
        _diceValue = diceValue;
    }
    
    public override bool CanExecute()
    {
        // Validate basic parameters
        if (_playerId < 0 || _playerId >= GameSettings.NumberOfPlayers)
            return false;
            
        // Check if services are available
        if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
            return false;
            
        // Check if it's the player's turn
        if (GameServices.Instance.TurnManager.GetCurrentTurn != _playerId)
            return false;
        
        // Get towers using service locator
        _sourceTower = GameServices.Instance.GetTowerByIndex(_sourceIndex);
        _targetTower = GameServices.Instance.GetTowerByIndex(_targetIndex);
        
        if (_sourceTower == null || _targetTower == null)
            return false;
            
        // Check if source tower has coins owned by current player
        if (_sourceTower.CoinsCount == 0 || !_sourceTower.IsOwnedBy(_playerId))
            return false;
            
        // Check if target tower allows this move
        if (!CanMoveToTarget(_targetTower))
            return false;
            
        return true;
    }
    
    public override bool Execute()
    {
        if (!CanExecute())
            return false;
            
        try
        {
            // Remove coin from source tower
            _movedCoin = _sourceTower.RemoveTopChecker();
            if (_movedCoin == null)
                return false;
            
            // Handle attack if target tower can be attacked
            _wasAttack = _targetTower.CanAttack() && _targetTower.GetOwnerPlayerId() != _playerId;
            if (_wasAttack)
            {
                _attackedCoin = _targetTower.RemoveTopChecker();
                if (_attackedCoin != null)
                {
                    // Move attacked coin to spawn
                    _spawnTower = GameServices.Instance.GetSpawnTower(_attackedCoin.GetOwnerId());
                    _spawnTower.AddCoin(_attackedCoin);
                    _attackedCoin.SetCoinState(CoinState.AtSpawn);
                }
            }
            
            // Add coin to target tower
            _targetTower.AddCoin(_movedCoin);
            
            // Hide all possible move rings since a move has been made
            MessageBus.Instance.Publish(new CoreGameMessage.CleanTowerRings());
            
            // Publish move completed message
            MessageBus.Instance.Publish(new CoreGameMessage.OnCoinMoved(_diceValue));
            
            Debug.Log($"Moved coin from tower {_sourceIndex} to tower {_targetIndex}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error executing move command: {e.Message}");
            return false;
        }
    }
    
    public override bool Undo()
    {
        if (_movedCoin == null || _targetTower == null || _sourceTower == null)
            return false;
            
        try
        {
            // Remove coin from target tower and put back to source
            var coinFromTarget = _targetTower.RemoveTopChecker();
            if (coinFromTarget != _movedCoin)
            {
                Debug.LogError("Undo failed: wrong coin on target tower");
                if (coinFromTarget != null)
                    _targetTower.AddCoin(coinFromTarget); // Put it back
                return false;
            }
            
            // If there was an attack, restore the attacked coin
            if (_wasAttack && _attackedCoin != null && _spawnTower != null)
            {
                // Verify the attacked coin is actually in the spawn tower
                if (_attackedCoin.GetCurrentTower() != -1 && _attackedCoin.GetCurrentTower() != _spawnTower.TowerIndex)
                {
                    Debug.LogError($"Undo failed: Attacked coin is not in expected spawn tower. Expected: {_spawnTower.TowerIndex}, Found: {_attackedCoin.GetCurrentTower()}");
                    return false;
                }
                
                // Remove the specific attacked coin from spawn and put back to target
                bool removed = _spawnTower.RemoveSpecificCoin(_attackedCoin);
                if (removed)
                {
                    _targetTower.AddCoin(_attackedCoin);
                    _attackedCoin.SetCoinState(CoinState.InGame);
                    Debug.Log($"Restored attacked coin (Player {_attackedCoin.GetOwnerId()}) to tower {_targetIndex}");
                }
                else
                {
                    Debug.LogError($"Failed to find and remove attacked coin from spawn tower during undo. Spawn tower has {_spawnTower.CoinsCount} coins.");
                    return false;
                }
            }
            
            // Put moved coin back to source tower
            _sourceTower.AddCoin(_movedCoin);
            
            // Restore the dice value that was used for this move
            // We need to publish a message to restore the dice value to the GameBoard
            RestoreDiceValue();
            
            Debug.Log($"Undone move from tower {_sourceIndex} to tower {_targetIndex}. Dice value {_diceValue} restored. Click on a coin to see possible moves again.");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error undoing move command: {e.Message}");
            return false;
        }
    }
    
    private bool CanMoveToTarget(Tower targetTower)
    {
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
    
    /// <summary>
    /// Restores the dice value that was used for this move
    /// </summary>
    private void RestoreDiceValue()
    {
        if (GameServices.Instance?.GameBoard != null)
        {
            // Add the dice value back to the GameBoard's dice list
            var remainingDice = GameServices.Instance.GameBoard.GetRemainingDiceValues();
            remainingDice.Add(_diceValue);
            
            // Publish message to notify other components about dice restoration
            var currentPlayer = GameServices.Instance.TurnManager.GetCurrentTurn;
            MessageBus.Instance.Publish(new CoreGameMessage.DiceValueRestored(_diceValue, currentPlayer));
            
            Debug.Log($"Restored dice value {_diceValue} to available dice. Total dice: [{string.Join(", ", remainingDice)}]");
        }
    }
    
    // These methods are no longer needed since we use GameServices
} 