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
    private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();
    
    // Events for UI updates
    public event Action<bool> OnCanUndoChanged;
    public event Action<bool> OnCanRedoChanged;
    public event Action<ICommand> OnCommandExecuted;
    public event Action<ICommand> OnCommandUndone;
    
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
                
                // Clear redo stack since we executed a new command
                _redoStack.Clear();
                
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
                UpdateCanUndoRedo();
                
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
                _redoStack.Push(command);
                OnCommandUndone?.Invoke(command);
                UpdateCanUndoRedo();
                
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
    /// Redo the last undone command
    /// </summary>
    public bool RedoLastCommand()
    {
        if (!CanRedo())
        {
            Debug.LogWarning("No commands to redo");
            return false;
        }
        
        var command = _redoStack.Pop();
        return ExecuteCommand(command);
    }
    
    /// <summary>
    /// Check if undo is possible
    /// </summary>
    public bool CanUndo()
    {
        return _undoStack.Count > 0 && _undoStack.Peek().CanUndo();
    }
    
    /// <summary>
    /// Check if redo is possible
    /// </summary>
    public bool CanRedo()
    {
        return _redoStack.Count > 0 && _redoStack.Peek().CanExecute();
    }
    
    /// <summary>
    /// Clear all command history
    /// </summary>
    public void ClearHistory()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        UpdateCanUndoRedo();
        Debug.Log("Command history cleared");
    }
    
    /// <summary>
    /// Get command history for debugging
    /// </summary>
    public List<string> GetCommandHistory()
    {
        return _undoStack.Select(cmd => $"{cmd.CreatedAt:HH:mm:ss} - {cmd.Description}").ToList();
    }
    
    private void UpdateCanUndoRedo()
    {
        OnCanUndoChanged?.Invoke(CanUndo());
        OnCanRedoChanged?.Invoke(CanRedo());
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