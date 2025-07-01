using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Commands;

/// <summary>
/// Composite command that executes multiple commands as a single transaction
/// </summary>
public class CompositeCommand : BaseCommand
{
    private readonly List<ICommand> _commands = new List<ICommand>();
    private readonly List<ICommand> _executedCommands = new List<ICommand>();
    
    public CompositeCommand(string description) : base(description)
    {
    }
    
    public CompositeCommand(string description, params ICommand[] commands) : base(description)
    {
        _commands.AddRange(commands);
    }
    
    /// <summary>
    /// Add a command to the composite
    /// </summary>
    public void AddCommand(ICommand command)
    {
        if (command != null)
            _commands.Add(command);
    }
    
    /// <summary>
    /// Add multiple commands to the composite
    /// </summary>
    public void AddCommands(params ICommand[] commands)
    {
        _commands.AddRange(commands.Where(c => c != null));
    }
    
    public override bool CanExecute()
    {
        // All commands must be executable
        return _commands.Count > 0 && _commands.All(cmd => cmd.CanExecute());
    }
    
    public override bool Execute()
    {
        if (!CanExecute())
        {
            Debug.LogWarning($"Composite command cannot be executed: {Description}");
            return false;
        }
        
        _executedCommands.Clear();
        
        // Execute all commands in order
        foreach (var command in _commands)
        {
            if (!command.Execute())
            {
                Debug.LogError($"Command failed in composite: {command.Description}");
                // Rollback all executed commands
                UndoExecutedCommands();
                return false;
            }
            
            _executedCommands.Add(command);
        }
        
        Debug.Log($"Composite command executed successfully: {Description}");
        return true;
    }
    
    public override bool Undo()
    {
        if (_executedCommands.Count == 0)
        {
            Debug.LogWarning($"No commands to undo in composite: {Description}");
            return false;
        }
        
        return UndoExecutedCommands();
    }
    
    public override bool CanUndo()
    {
        // Can undo if there are executed commands and all of them can be undone
        return _executedCommands.Count > 0 && _executedCommands.All(cmd => cmd.CanUndo());
    }
    
    private bool UndoExecutedCommands()
    {
        bool allUndone = true;
        
        // Undo in reverse order
        for (int i = _executedCommands.Count - 1; i >= 0; i--)
        {
            var command = _executedCommands[i];
            if (!command.Undo())
            {
                Debug.LogError($"Failed to undo command in composite: {command.Description}");
                allUndone = false;
                // Continue trying to undo the rest
            }
        }
        
        if (allUndone)
        {
            _executedCommands.Clear();
            Debug.Log($"Composite command undone successfully: {Description}");
        }
        
        return allUndone;
    }
    
    /// <summary>
    /// Get the number of commands in this composite
    /// </summary>
    public int CommandCount => _commands.Count;
    
    /// <summary>
    /// Get all command descriptions for debugging
    /// </summary>
    public List<string> GetCommandDescriptions()
    {
        return _commands.Select(cmd => cmd.Description).ToList();
    }
} 