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
                // Remove from spawn and put back to target
                var coinFromSpawn = _spawnTower.RemoveTopChecker();
                if (coinFromSpawn == _attackedCoin)
                {
                    _targetTower.AddCoin(_attackedCoin);
                    _attackedCoin.SetCoinState(CoinState.InGame);
                }
            }
            
            // Put moved coin back to source tower
            _sourceTower.AddCoin(_movedCoin);
            
            Debug.Log($"Undone move from tower {_sourceIndex} to tower {_targetIndex}");
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
    
    // These methods are no longer needed since we use GameServices
} 