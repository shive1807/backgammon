# Command Pattern Implementation Guide

## Overview

This implementation replaces `FindObjectOfType` calls with a proper dependency injection system using `GameServices`. All game actions now go through the Command Pattern for better maintainability, undo/redo functionality, and testability.

## Setup Instructions

### 1. GameServices Setup

1. **Add GameServices to your main scene:**
   - Create an empty GameObject named "GameServices"
   - Add the `GameServices` component to it
   - Assign all required references in the inspector:

```
GameServices GameObject:
├── Core Game Services
│   ├── Game Board (drag GameBoard from scene)
│   ├── Turn Manager (drag TurnManager from scene)  
│   ├── Game Manager (drag GameManager from scene)
│   └── Command Manager (drag CommandManager from scene)
├── Spawn Points
│   ├── White Spawn Point (drag from scene)
│   └── Black Spawn Point (drag from scene)
└── Dice Managers
    ├── Element 0: Player 0 DiceManager
    └── Element 1: Player 1 DiceManager
```

### 2. CommandManager Setup

1. **Add CommandManager to scene:**
   - Create an empty GameObject named "CommandManager"  
   - Add the `CommandManager` component
   - Configure max history size (default: 50)

### 3. UI Setup

1. **Update CanvasManager:**
   - Add Undo and Redo buttons to your UI
   - Assign them to the CanvasManager's undoButton and redoButton fields
   - The buttons will automatically enable/disable based on command availability

## Usage Examples

### Basic Move Command
```csharp
// Create and execute a move command
var moveCommand = new MoveCoinCommand(sourceIndex, targetIndex, playerId, diceValue);
bool success = CommandManager.Instance.ExecuteCommand(moveCommand);
```

### Complex Multi-Move
```csharp
// Create a composite command for multiple moves
var moves = new List<(int from, int to, int dice)> 
{
    (5, 3, 2),  // Move from tower 5 to 3 using dice 2
    (8, 6, 2)   // Move from tower 8 to 6 using dice 2
};

var multiMoveCommand = GameCommandFactory.CreateMultiMoveCommand(playerId, moves);
CommandManager.Instance.ExecuteCommand(multiMoveCommand);
```

### Undo/Redo Operations
```csharp
// Undo last command
bool undoSuccess = CommandManager.Instance.UndoLastCommand();

// Redo last undone command  
bool redoSuccess = CommandManager.Instance.RedoLastCommand();

// Check if undo/redo is possible
bool canUndo = CommandManager.Instance.CanUndo();
bool canRedo = CommandManager.Instance.CanRedo();
```

### AI Integration
```csharp
// Generate AI command for current dice values
var aiCommand = GameCommandFactory.CreateAICommand(aiPlayerId, diceValues);
if (aiCommand != null)
{
    CommandManager.Instance.ExecuteCommand(aiCommand);
}
```

## Architecture Benefits

### 1. No More FindObjectOfType
- All dependencies are injected through `GameServices`
- Better performance (no scene searching)
- Cleaner, more predictable code

### 2. Undo/Redo System
- Full command history tracking
- Atomic operations (all-or-nothing execution)
- UI automatically updates based on command state

### 3. Better Testing
- Commands can be unit tested in isolation
- Mock dependencies can be injected
- Game state is more predictable

### 4. AI Integration
- AI can generate command sequences
- Easy to implement different AI difficulty levels
- Commands can be analyzed for game tree search

### 5. Debugging & Logging
- Full history of all game actions
- Easy to reproduce game states
- Better error handling and recovery

## Command Types

### MoveCoinCommand
- Handles coin movement between towers
- Supports attack logic
- Full undo capability

### StartTurnCommand  
- Combines dice rolling with turn setup
- Cannot be undone (as per game rules)

### CompositeCommand
- Executes multiple commands as a transaction
- Rolls back all commands if any fail
- Perfect for complex moves

### RollDiceCommand
- Handles dice rolling logic
- Stores results for access
- Typically not undoable

## Service Access Patterns

### Getting Game Components
```csharp
// Instead of FindObjectOfType<GameBoard>()
var gameBoard = GameServices.Instance.GameBoard;

// Instead of FindObjectOfType<TurnManager>()
var turnManager = GameServices.Instance.TurnManager;
```

### Validating Moves
```csharp
// Check if a move is valid before creating command
bool isValid = GameServices.Instance.IsValidMove(fromIndex, toIndex, playerId, diceValue);

// Get all valid moves for a player
var validMoves = GameServices.Instance.GetValidMoves(playerId, diceValues);
```

### Accessing Towers
```csharp
// Get tower by index (handles spawn points automatically)
var tower = GameServices.Instance.GetTowerByIndex(towerIndex);

// Get spawn tower for player
var spawnTower = GameServices.Instance.GetSpawnTower(playerId);

// Get all towers owned by player
var ownedTowers = GameServices.Instance.GetTowersOwnedBy(playerId);
```

## Best Practices

1. **Always check service availability:**
   ```csharp
   if (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
       return false;
   ```

2. **Use factory methods for complex commands:**
   ```csharp
   var command = GameCommandFactory.CreateBearOffCommand(fromTower, playerId, diceValue);
   ```

3. **Handle command execution results:**
   ```csharp
   if (!CommandManager.Instance.ExecuteCommand(command))
   {
       Debug.LogError($"Failed to execute command: {command.Description}");
   }
   ```

4. **Subscribe to command events for UI updates:**
   ```csharp
   CommandManager.Instance.OnCommandExecuted += HandleCommandExecuted;
   CommandManager.Instance.OnCanUndoChanged += UpdateUndoButton;
   ```

## Migration from Old Code

### Before (using FindObjectOfType):
```csharp
var gameBoard = FindObjectOfType<GameBoard>();
var tower = gameBoard.towers[index];
if (tower.IsOwnedBy(playerId))
{
    // Direct manipulation
    tower.RemoveTopChecker();
}
```

### After (using Commands + Services):
```csharp
var moveCommand = new MoveCoinCommand(sourceIndex, targetIndex, playerId, diceValue);
CommandManager.Instance.ExecuteCommand(moveCommand);
```

This new architecture provides a solid foundation for building complex game features while maintaining clean, testable, and performant code. 