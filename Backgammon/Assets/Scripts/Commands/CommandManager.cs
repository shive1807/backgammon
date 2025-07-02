using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Commands;

/// <summary>
/// Manages command execution, undo/redo functionality, and command history
/// </summary>
public class CommandManager : MonoBehaviour
{
    public static CommandManager Instance { get; private set; }
    
    [Header("Command History Settings")]
    [SerializeField] private int maxHistorySize = 50;
    
    private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();
    
    // Track turn boundaries for reset functionality
    private int _commandCountAtTurnStart = 0;
    
    // Events for UI updates
    public event Action<ICommand> OnCommandExecuted;
    public event Action<ICommand> OnCommandUndone;
    public event Action OnTurnStarted;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    /// <summary>
    /// Execute a command and add it to history
    /// </summary>
    public bool ExecuteCommand(ICommand command)
    {
        if (command == null)
        {
            Debug.LogError("Cannot execute null command");
            return false;
        }
        
        if (!command.CanExecute())
        {
            Debug.LogWarning($"Command cannot be executed: {command.Description}");
            return false;
        }
        
        try
        {
            bool success = command.Execute();
            if (success)
            {
                // Add to undo stack
                _undoStack.Push(command);
                
                // Maintain history size limit
                if (_undoStack.Count > maxHistorySize)
                {
                    var commands = _undoStack.ToArray();
                    _undoStack.Clear();
                    // Keep only the most recent commands
                    for (int i = commands.Length - maxHistorySize; i < commands.Length; i++)
                    {
                        _undoStack.Push(commands[i]);
                    }
                }
                
                OnCommandExecuted?.Invoke(command);
                
                Debug.Log($"Command executed: {command.Description}");
            }
            else
            {
                Debug.LogError($"Command execution failed: {command.Description}");
            }
            
            return success;
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception during command execution: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Undo the last command
    /// </summary>
    public bool UndoLastCommand()
    {
        if (!CanUndo())
        {
            Debug.LogWarning("No commands to undo");
            return false;
        }
        
        var command = _undoStack.Pop();
        
        try
        {
            bool success = command.Undo();
            if (success)
            {
                OnCommandUndone?.Invoke(command);
                
                Debug.Log($"Command undone: {command.Description}");
            }
            else
            {
                // Put it back if undo failed
                _undoStack.Push(command);
                Debug.LogError($"Command undo failed: {command.Description}");
            }
            
            return success;
        }
        catch (Exception e)
        {
            // Put it back if exception occurred
            _undoStack.Push(command);
            Debug.LogError($"Exception during command undo: {e.Message}");
            return false;
        }
    }
    

    
    /// <summary>
    /// Check if undo is possible
    /// </summary>
    public bool CanUndo()
    {
        return _undoStack.Count > 0 && _undoStack.Peek().CanUndo();
    }
    

    
    /// <summary>
    /// Clear all command history
    /// </summary>
    public void ClearHistory()
    {
        _undoStack.Clear();
        _commandCountAtTurnStart = 0;
        Debug.Log("Command history cleared");
    }
    
    /// <summary>
    /// Mark the current point as the start of a turn (for reset functionality)
    /// Call this when dice are rolled to track turn boundaries
    /// </summary>
    public void MarkTurnStart()
    {
        _commandCountAtTurnStart = _undoStack.Count;
        OnTurnStarted?.Invoke();
        Debug.Log($"Turn started - commands at start: {_commandCountAtTurnStart}");
    }
    
    /// <summary>
    /// Undo all moves made since the current turn started
    /// This is what the Reset button should call
    /// Only undoes game state commands, skips visual commands
    /// </summary>
    public bool UndoCurrentTurn()
    {
        if (_undoStack.Count <= _commandCountAtTurnStart)
        {
            Debug.Log("No moves to undo in current turn");
            return false;
        }
        
        // Get all commands since turn start
        var commandsToProcess = new List<ICommand>();
        var tempStack = new Stack<ICommand>();
        
        // Pop commands until we reach the turn start point
        while (_undoStack.Count > _commandCountAtTurnStart)
        {
            var command = _undoStack.Pop();
            commandsToProcess.Add(command);
            tempStack.Push(command);
        }
        
        // Separate game state commands from visual commands
        var gameStateCommands = commandsToProcess.Where(cmd => cmd.IsGameStateCommand).ToList();
        var visualCommands = commandsToProcess.Where(cmd => !cmd.IsGameStateCommand).ToList();
        
        Debug.Log($"Turn reset: Found {gameStateCommands.Count} game state commands and {visualCommands.Count} visual commands");
        
        if (gameStateCommands.Count == 0)
        {
            // Put all commands back on the stack
            while (tempStack.Count > 0)
            {
                _undoStack.Push(tempStack.Pop());
            }
            Debug.Log("No game state moves to undo in current turn");
            return false;
        }
        
        // Undo only the game state commands in reverse order (most recent first)
        int successfulUndos = 0;
        var failedCommands = new List<ICommand>();
        
        foreach (var command in gameStateCommands.AsEnumerable().Reverse())
        {
            try
            {
                if (command.CanUndo() && command.Undo())
                {
                    successfulUndos++;
                    OnCommandUndone?.Invoke(command);
                    Debug.Log($"Reset: Undone {command.Description}");
                }
                else
                {
                    Debug.LogError($"Reset: Failed to undo {command.Description}");
                    failedCommands.Add(command);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Reset: Exception undoing {command.Description}: {e.Message}");
                failedCommands.Add(command);
            }
        }
        
        // Put failed commands back on the stack (they couldn't be undone)
        foreach (var failedCommand in failedCommands.AsEnumerable().Reverse())
        {
            _undoStack.Push(failedCommand);
        }
        
        Debug.Log($"Turn reset: Successfully undid {successfulUndos} out of {gameStateCommands.Count} game state commands");
        return successfulUndos > 0;
    }
    
    /// <summary>
    /// Get the number of game state moves made since the current turn started
    /// </summary>
    public int GetMovesInCurrentTurn()
    {
        if (_undoStack.Count <= _commandCountAtTurnStart)
            return 0;
            
        // Count only game state commands since turn start
        var commandsSinceTurnStart = _undoStack.Take(_undoStack.Count - _commandCountAtTurnStart);
        return commandsSinceTurnStart.Count(cmd => cmd.IsGameStateCommand);
    }
    
    /// <summary>
    /// Check if there are any moves to reset in the current turn
    /// </summary>
    public bool CanResetCurrentTurn()
    {
        return GetMovesInCurrentTurn() > 0;
    }
    
    /// <summary>
    /// Get command history for debugging
    /// </summary>
    public List<string> GetCommandHistory()
    {
        return _undoStack.Select(cmd => $"{cmd.CreatedAt:HH:mm:ss} - {cmd.Description}").ToList();
    }
    

    
    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.GameSetup>(OnGameSetup);
    }
    
    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.GameSetup>(OnGameSetup);
    }
    
    private void OnGameSetup(CoreGameMessage.GameSetup message)
    {
        ClearHistory();
    }
} 