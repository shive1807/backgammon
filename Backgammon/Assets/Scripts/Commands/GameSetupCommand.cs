using UnityEngine;
using Commands;
using System.Collections.Generic;

/// <summary>
/// Command for setting up the initial game state
/// </summary>
public class GameSetupCommand : BaseCommand
{
    private bool _gameSetupCompleted;
    
    public GameSetupCommand()
        : base("Initialize game setup", false) // false = setup command, not game state move
    {
        _gameSetupCompleted = false;
    }
    
    public override bool CanExecute()
    {
        // Check if services are available
        if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
            return false;
            
        return true;
    }
    
    public override bool Execute()
    {
        if (!CanExecute())
            return false;
            
        try
        {
            var gameBoard = GameServices.Instance.GameBoard;
            if (gameBoard == null || gameBoard.towers == null)
                return false;
            
            // Initialize tower indices
            InitializeTowers(gameBoard);
            
            // Add coins to starting positions
            AddInitialCoins(gameBoard);
            
            _gameSetupCompleted = true;
            Debug.Log("Game setup completed successfully");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error executing GameSetupCommand: {e.Message}");
            return false;
        }
    }
    
    public override bool Undo()
    {
        // Game setup typically cannot be undone
        Debug.LogWarning("Game setup undo is not supported");
        return false;
    }
    
    public override bool CanUndo()
    {
        return false;
    }
    
    /// <summary>
    /// Initialize all towers with their indices
    /// </summary>
    private void InitializeTowers(GameBoard gameBoard)
    {
        // Initialize main board towers
        for (int i = 0; i < gameBoard.towers.Count; i++)
        {
            gameBoard.towers[i].Initialize(i);
        }
        
        // Initialize spawn points
        var whiteSpawn = GameServices.Instance.GetSpawnTower(GameSettings.Player0);
        var blackSpawn = GameServices.Instance.GetSpawnTower(GameSettings.Player1);
        
        whiteSpawn?.Initialize(-1);
        blackSpawn?.Initialize(-1);
        
        Debug.Log($"Initialized {gameBoard.towers.Count} board towers and spawn points");
    }
    
    /// <summary>
    /// Add coins to their starting positions according to backgammon rules
    /// </summary>
    private void AddInitialCoins(GameBoard gameBoard)
    {
        // Add white coins to starting positions
        AddCoinsToTower(gameBoard, GameSettings.TowerIndex_TopRight, GameSettings.Coins_TopRight, GameSettings.Player0);
        AddCoinsToTower(gameBoard, GameSettings.TowerIndex_MiddleLeft, GameSettings.Coins_MiddleLeft, GameSettings.Player0);
        AddCoinsToTower(gameBoard, GameSettings.TowerIndex_Center, GameSettings.Coins_Center, GameSettings.Player0);
        AddCoinsToTower(gameBoard, GameSettings.TowerIndex_BottomLeft, GameSettings.Coins_BottomLeft, GameSettings.Player0);
        
        // Add black coins to starting positions
        AddCoinsToTower(gameBoard, GameSettings.TowerIndex_TopLeft, GameSettings.Coins_TopLeft, GameSettings.Player1);
        AddCoinsToTower(gameBoard, GameSettings.TowerIndex_MiddleRight, GameSettings.Coins_MiddleRight, GameSettings.Player1);
        AddCoinsToTower(gameBoard, GameSettings.TowerIndex_CenterRight, GameSettings.Coins_CenterRight, GameSettings.Player1);
        AddCoinsToTower(gameBoard, GameSettings.TowerIndex_BottomRight, GameSettings.Coins_BottomRight, GameSettings.Player1);
        
        Debug.Log("Added initial coins to starting positions");
    }
    
    /// <summary>
    /// Add coins to a specific tower
    /// </summary>
    private void AddCoinsToTower(GameBoard gameBoard, int towerIndex, int count, int playerId)
    {
        if (towerIndex < 0 || towerIndex >= gameBoard.towers.Count)
        {
            Debug.LogError($"Invalid tower index: {towerIndex}");
            return;
        }
        
        for (int i = 0; i < count; i++)
        {
            gameBoard.towers[towerIndex].AddInitCoins(playerId);
        }
        
        Debug.Log($"Added {count} coins for player {playerId} to tower {towerIndex}");
    }
}