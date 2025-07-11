using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    public List<Tower> towers = new (); // 24 towers on the board

    private int _currentTurn;
    
    private List<int> _diceValues;
    
    /// <summary>
    /// Get remaining dice values for UI and validation purposes
    /// </summary>
    public List<int> GetRemainingDiceValues()
    {
        return _diceValues ?? new List<int>();
    }

    // _diceValuesUsed removed - Command Pattern handles this
    
    [SerializeField]
    private Tower whiteSpawnPoint;
    
    [SerializeField]
    private Tower blackSpawnPoint;
    
    private void OnGameSetup(CoreGameMessage.GameSetup message)
    {
        _diceValues = new List<int>();
        
        // Use a command pattern for game setup
        var setupCommand = GameCommandFactory.CreateGameSetupCommand();
        if (setupCommand != null && setupCommand.CanExecute())
        {
            CommandManager.Instance.ExecuteCommand(setupCommand);
        }
        else
        {
            Debug.LogError("Failed to create or execute game setup command");
        }
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



    private void OnSwitchTurn(CoreGameMessage.SwitchTurn message)
    {
        // Dice value tracking removed - handled by Command Pattern
    }

    private void OnDiceRolled(CoreGameMessage.DiceRolled message)
    {
        _diceValues  = message.Dice;
        _currentTurn = message.CurrentPlayerIndex;
        
        // Use command pattern to highlight available coins
        var highlightCommand = GameCommandFactory.CreateHighlightAvailableCoinsCommand(_currentTurn, _diceValues);
        if (highlightCommand != null && highlightCommand.CanExecute())
        {
            CommandManager.Instance.ExecuteCommand(highlightCommand);
        }
        
        // Remove automatic turn end checking - let player decide when to end turn
        // Turn will only end when Done button is pressed
    }

    private void OnDonePressed(CoreGameMessage.OnDonePressed message)
    {
        // Use command pattern to validate and end the turn only if legal
        var manualTurnEndCommand = GameCommandFactory.CreateManualTurnEndCommand(_currentTurn, _diceValues);
        if (manualTurnEndCommand != null && manualTurnEndCommand.CanExecute())
        {
            bool success = CommandManager.Instance.ExecuteCommand(manualTurnEndCommand);
            if (!success)
            {
                Debug.Log($"Player {_currentTurn} cannot end turn yet - must use all available dice moves");
            }
        }
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
        
        Debug.Log($"Dice value {message.CheckerMovedByDiceValue} used. Remaining: [{string.Join(", ", _diceValues)}]");
        
        // Use command pattern to highlight coins for remaining dice values
        if (_diceValues.Count > 0)
        {
            var highlightCommand = GameCommandFactory.CreateHighlightAvailableCoinsCommand(_currentTurn, _diceValues);
            if (highlightCommand != null && highlightCommand.CanExecute())
            {
                CommandManager.Instance.ExecuteCommand(highlightCommand);
            }
        }
        else
        {
            // All dice used - remove highlights but don't end turn automatically
            // Player must press Done to end turn
            Debug.Log("All dice values used. Player can press Done to end turn.");
        }
        
        // Remove automatic turn end checking - let player decide when to end turn
        // Turn will only end when Done button is pressed
    }

    // OnRingClicked and OnResetPressed removed - now handled by Command Pattern
}