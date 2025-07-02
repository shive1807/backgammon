using System;

namespace Commands
{
    /// <summary>
    /// Base interface for all game commands
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Execute the command
        /// </summary>
        /// <returns>True if command executed successfully</returns>
        bool Execute();
        
        /// <summary>
        /// Undo the command if possible
        /// </summary>
        /// <returns>True if command was undone successfully</returns>
        bool Undo();
        
        /// <summary>
        /// Check if the command can be executed in current game state
        /// </summary>
        bool CanExecute();
        
        /// <summary>
        /// Check if the command can be undone
        /// </summary>
        bool CanUndo();
        
        /// <summary>
        /// Description of the command for debugging/logging
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Timestamp when command was created
        /// </summary>
        DateTime CreatedAt { get; }
        
        /// <summary>
        /// Whether this command affects game state (true) or is just visual (false)
        /// Visual commands are not included in turn resets
        /// </summary>
        bool IsGameStateCommand { get; }
    }
    
    /// <summary>
    /// Base abstract class for commands with common functionality
    /// </summary>
    public abstract class BaseCommand : ICommand
    {
        public string Description { get; protected set; }
        public DateTime CreatedAt { get; private set; }
        public virtual bool IsGameStateCommand { get; protected set; } = true;
        
        protected BaseCommand(string description, bool isGameStateCommand = true)
        {
            Description = description;
            CreatedAt = DateTime.Now;
            IsGameStateCommand = isGameStateCommand;
        }
        
        public abstract bool Execute();
        public abstract bool Undo();
        public abstract bool CanExecute();
        public virtual bool CanUndo() => true;
    }
} 