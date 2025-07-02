using UnityEngine;
using Commands;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Factory for creating common game commands and command combinations
/// </summary>
public static class GameCommandFactory
{
    /// <summary>
    /// Create a move command with validation
    /// </summary>
    public static MoveCoinCommand CreateMoveCommand(int fromTower, int toTower, int playerId, int diceValue)
    {
        return new MoveCoinCommand(fromTower, toTower, playerId, diceValue);
    }
    
    /// <summary>
    /// Create a command for multiple moves using multiple dice values
    /// </summary>
    public static CompositeCommand CreateMultiMoveCommand(int playerId, List<(int from, int to, int dice)> moves)
    {
        var composite = new CompositeCommand($"Multi-move for player {playerId}");
        
        foreach (var (from, to, dice) in moves)
        {
            var moveCommand = new MoveCoinCommand(from, to, playerId, dice);
            composite.AddCommand(moveCommand);
        }
        
        return composite;
    }
    
    /// <summary>
    /// Create a command for bearing off coins (moving them off the board)
    /// </summary>
    public static MoveCoinCommand CreateBearOffCommand(int fromTower, int playerId, int diceValue)
    {
        // In backgammon, bearing off means moving to position -1 (off board)
        return new MoveCoinCommand(fromTower, -1, playerId, diceValue);
    }
    
    /// <summary>
    /// Create commands for entering coins from the bar (spawn point)
    /// </summary>
    public static MoveCoinCommand CreateEnterFromBarCommand(int toTower, int playerId, int diceValue)
    {
        // From spawn point (-1) to board
        return new MoveCoinCommand(-1, toTower, playerId, diceValue);
    }
    
    /// <summary>
    /// Create a command that validates and executes the best possible moves for given dice
    /// </summary>
    public static ICommand CreateOptimalMoveCommand(int playerId, List<int> diceValues)
    {
        if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
            return null;
            
        // This is a simplified example - in a real implementation, you'd have 
        // sophisticated move generation and evaluation logic
        var possibleMoves = GeneratePossibleMoves(playerId, diceValues);
        
        if (possibleMoves.Count == 0)
            return null;
            
        if (possibleMoves.Count == 1)
            return possibleMoves[0];
            
        // Return composite command for multiple moves
        var composite = new CompositeCommand($"Optimal moves for player {playerId}");
        possibleMoves.ForEach(composite.AddCommand);
        return composite;
    }
    
    /// <summary>
    /// Create a command for AI move execution
    /// </summary>
    public static ICommand CreateAICommand(int aiPlayerId, List<int> diceValues)
    {
        // This would integrate with an AI system
        // For now, return a simple random move
        if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
            return null;
            
        var validMoves = GetValidMoves(aiPlayerId, diceValues);
        if (validMoves.Count == 0)
            return null;
            
        // Select random move for demonstration
        var randomMove = validMoves[Random.Range(0, validMoves.Count)];
        return randomMove;
    }
    
    /// <summary>
    /// Create a command for showing possible moves from a source tower
    /// </summary>
    public static ShowPossibleMovesCommand CreateShowPossibleMovesCommand(int sourceTowerIndex, int playerId, List<int> diceValues)
    {
        return new ShowPossibleMovesCommand(sourceTowerIndex, playerId, diceValues);
    }
    
    /// <summary>
    /// Create a command for hiding all possible move rings
    /// </summary>
    public static HidePossibleMovesCommand CreateHidePossibleMovesCommand()
    {
        return new HidePossibleMovesCommand();
    }
    
    // CreateUndoMultipleCommand removed - use CommandManager.UndoLastCommand() directly
    
    private static List<MoveCoinCommand> GeneratePossibleMoves(int playerId, List<int> diceValues)
    {
        var moves = new List<MoveCoinCommand>();
        
        if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
            return moves;
        
        var gameBoard = GameServices.Instance.GameBoard;
        
        // Simplified move generation - in reality this would be much more complex
        foreach (var tower in gameBoard.towers.Where(t => t.IsOwnedBy(playerId)))
        {
            foreach (var diceValue in diceValues)
            {
                int targetIndex = playerId == 0 ? tower.TowerIndex - diceValue : tower.TowerIndex + diceValue;
                
                if (targetIndex >= 0 && targetIndex < gameBoard.towers.Count)
                {
                    var moveCommand = new MoveCoinCommand(tower.TowerIndex, targetIndex, playerId, diceValue);
                    if (moveCommand.CanExecute())
                    {
                        moves.Add(moveCommand);
                    }
                }
            }
        }
        
        return moves;
    }
    
    private static List<MoveCoinCommand> GetValidMoves(int playerId, List<int> diceValues)
    {
        return GeneratePossibleMoves(playerId, diceValues);
    }
} 