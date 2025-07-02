using UnityEngine;
using Commands;
using System.Collections.Generic;
using System.Linq;

namespace Commands
{
    /// <summary>
    /// Command for highlighting coins that can be moved with available dice values
    /// </summary>
    public class HighlightAvailableCoinsCommand : BaseCommand
    {
        private readonly int _playerId;
        private readonly List<int> _diceValues;
        
        // State for undo - track which coins were highlighted
        private readonly List<Coin> _highlightedCoins;
        private bool _coinsCurrentlyHighlighted;
        
        public HighlightAvailableCoinsCommand(int playerId, List<int> diceValues)
            : base($"Highlight available coins for player {playerId}", false) // false = visual command, not game state
        {
            _playerId = playerId;
            _diceValues = new List<int>(diceValues ?? new List<int>());
            _highlightedCoins = new List<Coin>();
            _coinsCurrentlyHighlighted = false;
        }
        
        public override bool CanExecute()
        {
            // Validate basic parameters
            if (_playerId < 0 || _playerId >= GameSettings.NumberOfPlayers)
                return false;
                
            // Check if services are available
            if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
                return false;
                
            // Must have dice values to highlight for
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
                // Clear any existing highlights first
                ClearExistingHighlights();
                
                var gameBoard = GameServices.Instance.GameBoard;
                if (gameBoard == null || gameBoard.towers == null)
                    return false;
                
                var highlightedTowers = new HashSet<Tower>();
                int availableActions = 0;
                
                // Check spawn points first
                if (HasCoinsInSpawn(_playerId, out Tower spawnTower))
                {
                    spawnTower.HighlightTopCoin();
                    _highlightedCoins.Add(spawnTower.GetTopCoin());
                    availableActions += spawnTower.CoinsCount;
                    _coinsCurrentlyHighlighted = true;
                    Debug.Log($"Highlighted spawn point for player {_playerId} with {spawnTower.CoinsCount} coins");
                    return true;
                }
                
                // Highlight coins on board that can move
                foreach (var tower in gameBoard.towers.Where(t => t.IsOwnedBy(_playerId)))
                {
                    bool canMoveWithAnyDice = false;
                    
                    foreach (var diceValue in _diceValues)
                    {
                        var targetIndex = CalculateTargetIndex(tower.TowerIndex, diceValue, _playerId);
                        
                        if (IsValidTargetIndex(targetIndex, gameBoard.towers.Count))
                        {
                            canMoveWithAnyDice = true;
                            availableActions++;
                        }
                    }
                    
                    // Only highlight each tower once, even if it can move with multiple dice
                    if (canMoveWithAnyDice && !highlightedTowers.Contains(tower))
                    {
                        tower.HighlightTopCoin();
                        var topCoin = tower.GetTopCoin();
                        if (topCoin != null)
                        {
                            _highlightedCoins.Add(topCoin);
                        }
                        highlightedTowers.Add(tower);
                    }
                }
                
                _coinsCurrentlyHighlighted = _highlightedCoins.Count > 0;
                Debug.Log($"Highlighted {highlightedTowers.Count} towers with {availableActions} available actions for player {_playerId}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error executing HighlightAvailableCoinsCommand: {e.Message}");
                return false;
            }
        }
        
        public override bool Undo()
        {
            if (!_coinsCurrentlyHighlighted)
                return false;
                
            try
            {
                ClearExistingHighlights();
                _coinsCurrentlyHighlighted = false;
                
                Debug.Log($"Cleared highlights for player {_playerId}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error undoing HighlightAvailableCoinsCommand: {e.Message}");
                return false;
            }
        }
        
        public override bool CanUndo()
        {
            return _coinsCurrentlyHighlighted;
        }
        
        /// <summary>
        /// Clear all existing highlights and reset tracking
        /// </summary>
        private void ClearExistingHighlights()
        {
            foreach (var coin in _highlightedCoins)
            {
                if (coin != null)
                {
                    coin.Highlight(false);
                }
            }
            _highlightedCoins.Clear();
        }
        
        /// <summary>
        /// Check if player has coins in spawn that need to be moved first
        /// </summary>
        private bool HasCoinsInSpawn(int playerId, out Tower spawnTower)
        {
            spawnTower = GameServices.Instance.GetSpawnTower(playerId);
            return spawnTower != null && spawnTower.CoinsCount > 0;
        }
        
        /// <summary>
        /// Calculate target index based on player direction and dice value
        /// </summary>
        private int CalculateTargetIndex(int sourceIndex, int diceValue, int playerId)
        {
            return playerId == 0 ? sourceIndex - diceValue : sourceIndex + diceValue;
        }
        
        /// <summary>
        /// Check if target index is within valid bounds
        /// </summary>
        private bool IsValidTargetIndex(int targetIndex, int boardSize)
        {
            return targetIndex >= 0 && targetIndex < boardSize;
        }
    }
} 