using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    public List<Tower> towers = new (); // 24 towers on the board

    private int _availableActions;

    private int _currentTurn;
    
    private List<int> _diceValues;

    // _diceValuesUsed removed - Command Pattern handles this
    
    [SerializeField]
    private Tower whiteSpawnPoint;
    
    [SerializeField]
    private Tower blackSpawnPoint;
    
    private void OnGameSetup(CoreGameMessage.GameSetup message)
    {
        _diceValues       = new List<int>();
        _availableActions = 0;
        //initialize tower index.
        var i = 0;
        foreach (var tower in towers)
        {
            tower.Initialize(i);
            i++;
        }
        
        blackSpawnPoint.Initialize(-1);
        whiteSpawnPoint.Initialize(-1);
        
        AddWhiteCoins();
        AddBlackCoins();
    }

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Subscribe<CoreGameMessage.TurnDiceSetupAndRoll>(TurnDiceSetupAndRoll);
        MessageBus.Instance.Subscribe<CoreGameMessage.DiceRolled>(OnDiceRolled);
        MessageBus.Instance.Subscribe<CoreGameMessage.CoinClicked>(OnCoinClicked);
        MessageBus.Instance.Subscribe<CoreGameMessage.OnCoinMoved>(OnCheckerMoved);
        MessageBus.Instance.Subscribe<CoreGameMessage.OnDonePressed>(OnDonePressed);
        MessageBus.Instance.Subscribe<CoreGameMessage.SwitchTurn>(OnSwitchTurn);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.TurnDiceSetupAndRoll>(TurnDiceSetupAndRoll);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.DiceRolled>(OnDiceRolled);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.CoinClicked>(OnCoinClicked);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.OnCoinMoved>(OnCheckerMoved);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.OnDonePressed>(OnDonePressed);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.SwitchTurn>(OnSwitchTurn);
    }

    private void AddWhiteCoins()
    {
        AddCoinsToTower(GameSettings.TowerIndex_TopRight,    GameSettings.Coins_TopRight,    GameSettings.Player0);
        AddCoinsToTower(GameSettings.TowerIndex_MiddleLeft,  GameSettings.Coins_MiddleLeft,  GameSettings.Player0);
        AddCoinsToTower(GameSettings.TowerIndex_Center,      GameSettings.Coins_Center,      GameSettings.Player0);
        AddCoinsToTower(GameSettings.TowerIndex_BottomLeft,  GameSettings.Coins_BottomLeft,  GameSettings.Player0);
    }

    private void AddBlackCoins()
    {
        AddCoinsToTower(GameSettings.TowerIndex_TopLeft,      GameSettings.Coins_TopLeft,      GameSettings.Player1);  // Tower 1 (index 0)
        AddCoinsToTower(GameSettings.TowerIndex_MiddleRight,  GameSettings.Coins_MiddleRight,  GameSettings.Player1); // Tower 12 (index 11)
        AddCoinsToTower(GameSettings.TowerIndex_CenterRight,  GameSettings.Coins_CenterRight,  GameSettings.Player1); // Tower 17 (index 16)
        AddCoinsToTower(GameSettings.TowerIndex_BottomRight,  GameSettings.Coins_BottomRight,  GameSettings.Player1); // Tower 19 (index 18)
    }

    /// <summary>
    /// Adds coins to a specific tower and sets their color.
    /// </summary>
    /// <param name="towerIndex">Index of the tower to place checkers on (0–23).</param>
    /// <param name="count">Number of checkers to add.</param>
    /// <param name="playerId">0 = White, 1 = Black</param>
    private void AddCoinsToTower(int towerIndex, int count, int playerId)
    {
        for (var i = 0; i < count; i++)
        {
            towers[towerIndex].AddInitCoins(playerId);
        }
    }

    private void OnSwitchTurn(CoreGameMessage.SwitchTurn message)
    {
        // Dice value tracking removed - handled by Command Pattern
    }

    private void OnDiceRolled(CoreGameMessage.DiceRolled message)
    {
        _diceValues  = message.Dice;
        _currentTurn = message.CurrentPlayerIndex;
        if (GetAvailableActions() == 0)
        {
            MessageBus.Instance.Publish(new CoreGameMessage.TurnOver());
        }
    }

    private void OnDonePressed(CoreGameMessage.OnDonePressed message)
    {
        //Check if no available dive moves left to play
        if(_diceValues.Count == 0)
        {
            MessageBus.Instance.Publish(new CoreGameMessage.TurnOver());
            return;
        }
        
        //Check if no available actions left to play
        if (GetAvailableActions() == 0)
        {
            MessageBus.Instance.Publish(new CoreGameMessage.TurnOver());
            return;
        }
    }
    

    private int GetAvailableActions()
    {
        _availableActions = 0;
        var highlightedTowers = new HashSet<Tower>(); // Track highlighted towers to avoid duplicates
        
        //check if we have any pending actions to remove dead coin.
        if (_currentTurn == 0)
        {
            if (whiteSpawnPoint.CoinsCount > 0)
            {
                whiteSpawnPoint.HighlightTopCoin();
                for (var i = 0; i < whiteSpawnPoint.CoinsCount; i++)
                {
                    _availableActions++;
                }

                return _availableActions;
            }
        }
        else
        {
            if (blackSpawnPoint.CoinsCount > 0)
            {
                blackSpawnPoint.HighlightTopCoin();
                for (var i = 0; i < blackSpawnPoint.CoinsCount; i++)
                {
                    _availableActions++;
                }
                return _availableActions;
            }
        }
        
        foreach (var tower in towers.Where(tower => tower.IsOwnedBy(_currentTurn)))
        {
            bool canMoveWithAnyDice = false;
            
            foreach (var diceValue in _diceValues)
            {
                var targetValue = tower.TowerIndex;

                if (_currentTurn == 0)
                {
                    targetValue -= diceValue;
                }
                else
                {
                    targetValue += diceValue;
                }
                
                if (targetValue >= 0 && targetValue < towers.Count)
                {
                    canMoveWithAnyDice = true;
                    _availableActions++;
                }
            }
            
            // Only highlight each tower once, even if it can move with multiple dice
            if (canMoveWithAnyDice && !highlightedTowers.Contains(tower))
            {
                tower.HighlightTopCoin();
                highlightedTowers.Add(tower);
            }
        }
        
        Debug.Log($"Highlighted {highlightedTowers.Count} towers with {_availableActions} available actions for player {_currentTurn}");
        return _availableActions;
    }

    //update current player turn.
    //turn started and dice is rolled.
    private void TurnDiceSetupAndRoll(CoreGameMessage.TurnDiceSetupAndRoll message)
    {
        _currentTurn = message.PlayerIndex;
    }

    //listens to event raised by coin - creates rings for possible moves using command pattern
    private void OnCoinClicked(CoreGameMessage.CoinClicked message)
    {
        // Get current player ID - use services if available, fallback to local _currentTurn
        var currentPlayerId = GameServices.Instance != null && GameServices.Instance.AreServicesReady() 
            ? GameServices.Instance.TurnManager.GetCurrentTurn 
            : _currentTurn;
            
        // Only show moves for coins owned by the current player
        if (message.OwnerId != currentPlayerId)
        {
            Debug.Log($"Cannot show moves for coin owned by player {message.OwnerId} when it's player {currentPlayerId}'s turn");
            return;
        }
        
        // Create and execute the show possible moves command
        var showMovesCommand = GameCommandFactory.CreateShowPossibleMovesCommand(
            message.TowerIndex, 
            message.OwnerId, 
            _diceValues);
            
        if (showMovesCommand != null && showMovesCommand.CanExecute())
        {
            CommandManager.Instance.ExecuteCommand(showMovesCommand);
        }
        else
        {
            Debug.Log($"Cannot show possible moves from tower {message.TowerIndex} for player {message.OwnerId}");
        }
    }
    
    private void OnCheckerMoved(CoreGameMessage.OnCoinMoved message)
    {
        _diceValues.Remove(message.CheckerMovedByDiceValue);
        
        // Re-highlight coins that can be moved with remaining dice values
        if (_diceValues.Count > 0)
        {
            Debug.Log($"Re-highlighting coins for remaining dice: [{string.Join(", ", _diceValues)}]");
            GetAvailableActions();
        }
        else
        {
            // No more dice values, turn is over
            Debug.Log("All dice values used, ending turn");
            MessageBus.Instance.Publish(new CoreGameMessage.TurnOver());
        }
    }

    // OnRingClicked and OnResetPressed removed - now handled by Command Pattern
}