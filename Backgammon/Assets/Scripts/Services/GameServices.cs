using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Service locator for game dependencies to avoid FindObjectOfType calls
/// This should be initialized before other game components
/// </summary>
[DefaultExecutionOrder(-100)] // Execute before other scripts
public class GameServices : MonoBehaviour
{
    public static GameServices Instance { get; private set; }
    
    [Header("Core Game Services")]
    [SerializeField] private GameBoard gameBoard;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CommandManager commandManager;
    
    [Header("Spawn Points")]
    [SerializeField] private Tower whiteSpawnPoint;
    [SerializeField] private Tower blackSpawnPoint;
    
    [Header("Dice Managers")]
    [SerializeField] private DiceManager[] diceManagers;
    
    public GameBoard GameBoard => gameBoard;
    public TurnManager TurnManager => turnManager;
    public GameManager GameManager => gameManager;
    public CommandManager CommandManager => commandManager;
    public Tower WhiteSpawnPoint => whiteSpawnPoint;
    public Tower BlackSpawnPoint => blackSpawnPoint;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple GameServices instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Debug.Log("GameServices: Initializing...");
        
        // Auto-find components if not assigned
        if (gameBoard == null)
            gameBoard = GetComponent<GameBoard>();
        if (turnManager == null)
            turnManager = GetComponent<TurnManager>();
        if (gameManager == null)
            gameManager = GetComponent<GameManager>();
        if (commandManager == null)
            commandManager = GetComponent<CommandManager>();
            
        Debug.Log("GameServices: Component auto-detection completed.");
    }
    
    private void Start()
    {
        ValidateServices();
    }
    
    private void ValidateServices()
    {
        Debug.Log("GameServices: Validating services...");
        
        bool allValid = true;
        
        if (gameBoard == null)
        {
            Debug.LogError("GameBoard not assigned to GameServices!");
            allValid = false;
        }
        if (turnManager == null)
        {
            Debug.LogError("TurnManager not assigned to GameServices!");
            allValid = false;
        }
        if (gameManager == null)
        {
            Debug.LogError("GameManager not assigned to GameServices!");
            allValid = false;
        }
        if (commandManager == null)
        {
            Debug.LogError("CommandManager not assigned to GameServices!");
            allValid = false;
        }
        if (whiteSpawnPoint == null)
        {
            Debug.LogError("WhiteSpawnPoint not assigned to GameServices!");
            allValid = false;
        }
        if (blackSpawnPoint == null)
        {
            Debug.LogError("BlackSpawnPoint not assigned to GameServices!");
            allValid = false;
        }
        
        if (allValid)
        {
            Debug.Log("GameServices: All services validated successfully!");
        }
        else
        {
            Debug.LogError("GameServices: Some services are missing. Please check the inspector assignments.");
        }
    }
    
    /// <summary>
    /// Get spawn tower for a specific player
    /// </summary>
    public Tower GetSpawnTower(int playerId)
    {
        return playerId == 0 ? whiteSpawnPoint : blackSpawnPoint;
    }
    
    /// <summary>
    /// Get tower by index with proper validation
    /// </summary>
    public Tower GetTowerByIndex(int index)
    {
        if (index == -1)
        {
            // Return appropriate spawn point based on current turn
            if (turnManager != null)
                return GetSpawnTower(turnManager.GetCurrentTurn);
            else
                return GetSpawnTower(0); // Default to player 0 if turnManager not ready
        }
        
        if (gameBoard == null || index < 0 || index >= gameBoard.towers.Count)
            return null;
            
        return gameBoard.towers[index];
    }
    
    /// <summary>
    /// Get all towers owned by a specific player
    /// </summary>
    public List<Tower> GetTowersOwnedBy(int playerId)
    {
        var ownedTowers = new List<Tower>();
        
        if (gameBoard == null)
            return ownedTowers;
            
        foreach (var tower in gameBoard.towers)
        {
            if (tower.IsOwnedBy(playerId))
                ownedTowers.Add(tower);
        }
        
        return ownedTowers;
    }
    
    /// <summary>
    /// Get dice manager for a specific player
    /// </summary>
    public DiceManager GetDiceManager(int playerId)
    {
        if (diceManagers == null || playerId < 0 || playerId >= diceManagers.Length)
            return null;
            
        return diceManagers[playerId];
    }
    
    /// <summary>
    /// Check if all required services are available
    /// </summary>
    public bool AreServicesReady()
    {
        return gameBoard != null && 
               turnManager != null && 
               gameManager != null && 
               commandManager != null &&
               whiteSpawnPoint != null &&
               blackSpawnPoint != null;
    }
    
    /// <summary>
    /// Get the current game state
    /// </summary>
    public GameState GetCurrentGameState()
    {
        // You'll need to expose this from GameManager
        // For now, return a default state
        return GameState.Idle;
    }
    
    /// <summary>
    /// Validate a move without executing it
    /// </summary>
    public bool IsValidMove(int fromIndex, int toIndex, int playerId, int diceValue)
    {
        var moveCommand = new MoveCoinCommand(fromIndex, toIndex, playerId, diceValue);
        return moveCommand.CanExecute();
    }
    
    /// <summary>
    /// Get all valid moves for a player with given dice values
    /// </summary>
    public List<(int from, int to, int dice)> GetValidMoves(int playerId, List<int> diceValues)
    {
        var validMoves = new List<(int, int, int)>();
        
        if (!AreServicesReady())
            return validMoves;
            
        var ownedTowers = GetTowersOwnedBy(playerId);
        
        foreach (var tower in ownedTowers)
        {
            foreach (var diceValue in diceValues)
            {
                int targetIndex = playerId == 0 ? tower.TowerIndex - diceValue : tower.TowerIndex + diceValue;
                
                if (IsValidMove(tower.TowerIndex, targetIndex, playerId, diceValue))
                {
                    validMoves.Add((tower.TowerIndex, targetIndex, diceValue));
                }
            }
        }
        
        return validMoves;
    }
} 