using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CanvasManager : MonoBehaviour
{ 
    [SerializeField] private Button doneButton;
    [SerializeField] private Button resetButton;

    private void Start()
    {
        doneButton.onClick.AddListener(() =>
        {
            Debug.Log("Done button pressed - attempting to end turn");
            MessageBus.Instance.Publish(new CoreGameMessage.OnDonePressed());
        });
        
        resetButton.onClick.AddListener(() =>
        {
            // Reset all moves made in the current turn
            bool success = CommandManager.Instance.UndoCurrentTurn();
            if (!success)
            {
                Debug.Log("No moves to reset in current turn");
            }
        });
        
        // Set initial button states
        UpdateButtons();
    }
    
    private void OnEnable()
    {
        if (CommandManager.Instance != null)
        {
            CommandManager.Instance.OnCommandExecuted += OnCommandExecuted;
            CommandManager.Instance.OnCommandUndone += OnCommandUndone;
            CommandManager.Instance.OnTurnStarted += OnTurnStarted;
        }
        
        // Subscribe to game events that affect button states
        MessageBus.Instance.Subscribe<CoreGameMessage.DiceRolled>(OnDiceRolled);
        MessageBus.Instance.Subscribe<CoreGameMessage.OnCoinMoved>(OnCoinMoved);
    }
    
    private void OnDisable()
    {
        if (CommandManager.Instance != null)
        {
            CommandManager.Instance.OnCommandExecuted -= OnCommandExecuted;
            CommandManager.Instance.OnCommandUndone -= OnCommandUndone;
            CommandManager.Instance.OnTurnStarted -= OnTurnStarted;
        }
        
        // Unsubscribe from game events
        MessageBus.Instance.Unsubscribe<CoreGameMessage.DiceRolled>(OnDiceRolled);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.OnCoinMoved>(OnCoinMoved);
    }
    
    private void OnCommandExecuted(Commands.ICommand command)
    {
        UpdateButtons();
    }
    
    private void OnCommandUndone(Commands.ICommand command)
    {
        UpdateButtons();
    }
    
    private void OnTurnStarted()
    {
        UpdateButtons();
    }
    
    private void OnDiceRolled(CoreGameMessage.DiceRolled message)
    {
        UpdateButtons();
    }
    
    private void OnCoinMoved(CoreGameMessage.OnCoinMoved message)
    {
        UpdateButtons();
    }
    
    private void UpdateButtons()
    {
        // Update Reset button - only enable if there are moves to reset
        if (resetButton != null && CommandManager.Instance != null)
        {
            resetButton.interactable = CommandManager.Instance.CanResetCurrentTurn();
        }
        
        // Done button should only be enabled when all dice are used OR no valid moves are available
        if (doneButton != null)
        {
            bool canEndTurn = CanEndTurn();
            doneButton.interactable = canEndTurn;
        }
    }
    
    /// <summary>
    /// Check if the current player can legally end their turn
    /// </summary>
    private bool CanEndTurn()
    {
        // Check if GameServices is available
        if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
            return false;
            
        var gameBoard = GameServices.Instance.GameBoard;
        var turnManager = GameServices.Instance.TurnManager;
        
        if (gameBoard == null || turnManager == null)
            return false;
            
        // Get current player and remaining dice values
        int currentPlayer = turnManager.GetCurrentTurn;
        var remainingDice = GetRemainingDiceValues(gameBoard);
        
        // If no dice values left, can always end turn
        if (remainingDice.Count == 0)
            return true;
            
        // Check if any valid moves are available with remaining dice
        return !HasValidMoves(currentPlayer, remainingDice);
    }
    
    /// <summary>
    /// Get remaining dice values from GameBoard
    /// </summary>
    private System.Collections.Generic.List<int> GetRemainingDiceValues(GameBoard gameBoard)
    {
        return gameBoard.GetRemainingDiceValues();
    }
    
    /// <summary>
    /// Check if player has any valid moves with remaining dice
    /// </summary>
    private bool HasValidMoves(int playerId, System.Collections.Generic.List<int> diceValues)
    {
        if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
            return false;
            
        var gameBoard = GameServices.Instance.GameBoard;
        if (gameBoard == null || gameBoard.towers == null)
            return false;
            
        // Check spawn points first (priority moves)
        var spawnTower = GameServices.Instance.GetSpawnTower(playerId);
        if (spawnTower != null && spawnTower.CoinsCount > 0)
        {
            // If coins are in spawn, they must be moved first
            foreach (var diceValue in diceValues)
            {
                int targetIndex = CalculateTargetIndex(-1, diceValue, playerId); // -1 for spawn
                if (IsValidMove(targetIndex, gameBoard, playerId))
                {
                    return true; // Found at least one valid move
                }
            }
            return false; // No valid moves from spawn
        }
        
        // Check regular board moves
        foreach (var tower in gameBoard.towers.Where(t => t.IsOwnedBy(playerId)))
        {
            foreach (var diceValue in diceValues)
            {
                int targetIndex = CalculateTargetIndex(tower.TowerIndex, diceValue, playerId);
                if (IsValidMove(targetIndex, gameBoard, playerId))
                {
                    return true; // Found at least one valid move
                }
            }
        }
        
        return false; // No valid moves available
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
    private bool IsValidMove(int targetIndex, GameBoard gameBoard, int playerId)
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
        if (targetTower.IsOwnedBy(playerId))
            return true;
            
        // Can attack single opponent coin
        if (targetTower.CanAttack() && !targetTower.IsOwnedBy(playerId))
            return true;
            
        return false;
    }
}
