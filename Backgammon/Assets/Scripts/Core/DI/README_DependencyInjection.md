# Dependency Injection System & Context Installers

## Overview

This document explains how to use the dependency injection (DI) system and context installers in the backgammon game. The system replaces singleton patterns and `FindObjectOfType` calls with proper dependency injection for better testability, maintainability, and decoupling.

## Core Components

### 1. DIContainer
The main dependency injection container that manages service registration and resolution.

### 2. DIBinder
Fluent API for binding services to implementations.

### 3. MonoInstaller
Base class for creating custom installers that register services.

### 4. ProjectContext
Global context that manages project-wide services.

### 5. SceneContext
Scene-specific context for scene-local services.

### 6. MonoInjectHelper
Helper class for injecting dependencies into MonoBehaviours.

## Setup Instructions

### 1. Create ProjectContext

1. **Create a ProjectContext GameObject:**
   ```
   1. Create empty GameObject named "[ProjectContext]"
   2. Add ProjectContext component
   3. Add your context installers to the Installers list
   ```

2. **Add GameContextInstaller:**
   ```
   1. Create empty GameObject named "GameContextInstaller"
   2. Add GameContextInstaller component
   3. Drag it to ProjectContext's Installers list
   4. Assign all service references in the inspector
   ```

### 2. Configure GameContextInstaller

The `GameContextInstaller` automatically registers all core game services:

```csharp
// Core Game Services
- GameManager (singleton)
- TurnManager (singleton)
- GameBoard (singleton)
- CommandManager (singleton)

// Data & State Management
- PlayerDataManager (singleton)
- PrefabManager (singleton)

// Audio
- AudioManager (singleton)

// UI & Canvas
- CanvasManager (singleton)

// Factories
- GameCommandFactory (singleton)
- DiceManager[] (factory)

// Message System
- MessageBus (singleton)
```

### 3. Enable Auto-Find Services

Set `autoFindServices = true` in GameContextInstaller to automatically find services in the scene, or manually assign them in the inspector.

## Usage Examples

### 1. Basic Dependency Injection

```csharp
public class MyGameComponent : MonoBehaviour
{
    [Inject] private GameManager gameManager;
    [Inject] private TurnManager turnManager;
    [Inject] private CommandManager commandManager;
    [Inject(optional: true)] private AudioManager audioManager;
    
    private void Start()
    {
        // Inject dependencies
        MonoInjectHelper.InjectIntoObject(this);
        
        // Use injected services
        if (gameManager != null)
        {
            Debug.Log($"Current game state: {gameManager.CurrentState}");
        }
    }
}
```

### 2. Manual Service Resolution

```csharp
public class ManualInjectionExample : MonoBehaviour
{
    private void Start()
    {
        // Get container from ProjectContext
        var container = ProjectContext.Container;
        
        // Resolve services manually
        var gameManager = container.Resolve<GameManager>();
        var turnManager = container.Resolve<TurnManager>();
        var messageBus = container.Resolve<MessageBus>();
        
        // Use services
        int currentTurn = turnManager.GetCurrentTurn;
        messageBus.Publish(new CoreGameMessage.TurnOver());
    }
}
```

### 3. Creating Custom Installers

```csharp
public class CustomGameplayInstaller : MonoInstaller
{
    [SerializeField] private MyCustomService customService;
    
    public override void InstallBindings(DIContainer container)
    {
        // Bind custom service
        container.Bind<MyCustomService>().FromInstance(customService);
        
        // Bind interface to implementation
        container.Bind<IMyInterface>().To<MyImplementation>().AsSingle();
        
        // Bind factory
        container.BindFactory<MyFactoryType>(container => 
        {
            return new MyFactoryType(container.Resolve<GameManager>());
        });
    }
}
```

### 4. Injecting into Dynamic Objects

```csharp
public class DynamicObjectCreator : MonoBehaviour
{
    public void CreateAndInjectObject()
    {
        // Create new GameObject
        var newObj = Instantiate(myPrefab);
        
        // Inject into the new object and all its children
        MonoInjectHelper.InjectIntoGameObject(newObj);
    }
}
```

## Advanced Usage

### 1. Binding Scopes

```csharp
// Transient - new instance every time
container.Bind<IMyService>().To<MyService>().AsTransient();

// Singleton - single instance per container
container.Bind<IMyService>().To<MyService>().AsSingle();

// Instance - bind to existing instance
container.Bind<IMyService>().FromInstance(existingInstance);
```

### 2. Factory Bindings

```csharp
// Simple factory
container.BindFactory<MyClass>(container => new MyClass());

// Factory with dependencies
container.BindFactory<MyClass>(container => 
{
    var dependency = container.Resolve<MyDependency>();
    return new MyClass(dependency);
});
```

### 3. Optional Injection

```csharp
public class OptionalInjectionExample : MonoBehaviour
{
    [Inject(optional: true)] private AudioManager audioManager;
    [Inject] private GameManager gameManager; // Required
    
    private void Start()
    {
        MonoInjectHelper.InjectIntoObject(this);
        
        // Check if optional service is available
        if (audioManager != null)
        {
            // Use audio service
        }
    }
}
```

## Best Practices

### 1. Use Interfaces
```csharp
// Good - bind interface to implementation
container.Bind<IGameService>().To<GameService>().AsSingle();

// Less ideal - bind concrete class
container.Bind<GameService>().AsSingle();
```

### 2. Avoid Circular Dependencies
```csharp
// Bad - circular dependency
public class ServiceA 
{
    [Inject] private ServiceB serviceB;
}

public class ServiceB 
{
    [Inject] private ServiceA serviceA; // Circular!
}
```

### 3. Use Constructor Injection for Required Dependencies
```csharp
public class MyService
{
    private readonly GameManager gameManager;
    
    // Constructor injection - preferred for required dependencies
    public MyService(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
}
```

### 4. Group Related Services in Installers
```csharp
// GameContextInstaller - Core game services
// AudioContextInstaller - Audio services
// UIContextInstaller - UI services
```

## Migration from Singleton Pattern

### Before (Singleton)
```csharp
public class OldService : MonoBehaviour
{
    public static OldService Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

// Usage
OldService.Instance.DoSomething();
```

### After (Dependency Injection)
```csharp
public class NewService : MonoBehaviour
{
    // No singleton pattern needed
    public void DoSomething() { }
}

// In installer
container.Bind<NewService>().FromInstance(newServiceInstance);

// In consuming class
[Inject] private NewService newService;

// Usage
newService.DoSomething();
```

## Troubleshooting

### Common Issues

1. **"No binding found for X"**
   - Make sure the service is registered in an installer
   - Check that the installer is added to ProjectContext

2. **"Circular dependency detected"**
   - Review your dependency graph
   - Consider using factory patterns or events

3. **Injection not working**
   - Make sure `MonoInjectHelper.InjectIntoObject(this)` is called
   - Check that the field/property has `[Inject]` attribute

4. **Services not found during auto-find**
   - Ensure services exist in the scene
   - Check that services are active
   - Consider manual assignment in inspector

### Debug Tips

1. **Enable debug logging:**
   ```csharp
   Debug.Log("[Installer] Registered service: " + typeof(T).Name);
   ```

2. **Check container contents:**
   ```csharp
   var container = ProjectContext.Container;
   var gameManager = container.Resolve<GameManager>();
   Debug.Log($"GameManager found: {gameManager != null}");
   ```

3. **Validate installer setup:**
   ```csharp
   private void OnValidate()
   {
       if (gameManager == null)
           Debug.LogWarning("GameManager not assigned!");
   }
   ```

## Example Scene Setup

```
Scene Hierarchy:
├── [ProjectContext]
│   └── ProjectContext (component)
│       └── Installers List:
│           ├── GameContextInstaller
│           └── AudioContextInstaller
├── GameManager
├── TurnManager
├── GameBoard
├── CommandManager
├── AudioManager
└── Other Game Objects...
```

This setup ensures all services are properly registered and available for injection throughout your game. 