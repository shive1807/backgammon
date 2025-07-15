using UnityEngine;
using Core.DI;
using Core.GameAudio;
using Commands;

namespace Core.DI
{
    /// <summary>
    /// Main context installer for the backgammon game.
    /// Registers all core services and managers with the DI container.
    /// </summary>
    ///
    [DefaultExecutionOrder(-100)]
    public class GameContextInstaller : MonoInstaller
    {
        [Header("Core Game Services")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private GameBoard gameBoard;
        [SerializeField] private CommandManager commandManager;
        
        [Header("Data & State Management")]
        [SerializeField] private PlayerDataManager playerDataManager;
        [SerializeField] private PrefabManager prefabManager;
        
        [Header("Audio")]
        [SerializeField] private AudioManager audioManager;
        
        [Header("UI & Canvas")]
        [SerializeField] private CanvasManager canvasManager;
        
        [Header("Auto-Find Services")]
        [SerializeField] private bool autoFindServices = true;
        
        public override void InstallBindings(DiContainer container)
        {
            Debug.Log("[GameContextInstaller] Installing game bindings...");
            
            // Auto-find services if not manually assigned
            if (autoFindServices)
            {
                AutoFindServices();
            }
            
            // Install core services
            InstallCoreServices(container);
            
            // Install managers
            InstallManagers(container);
            
            // Install singletons
            InstallSingletons(container);
            
            // Install factories
            InstallFactories(container);
            
            // Install message bus
            InstallMessageBus(container);
            
            Debug.Log("[GameContextInstaller] Game bindings installed successfully!");
        }
        
        private void AutoFindServices()
        {
            // Auto-find services if not manually assigned
            if (gameManager == null)
                gameManager = FindObjectOfType<GameManager>();
            
            if (turnManager == null)
                turnManager = FindObjectOfType<TurnManager>();
            
            if (gameBoard == null)
                gameBoard = FindObjectOfType<GameBoard>();
            
            if (commandManager == null)
                commandManager = FindObjectOfType<CommandManager>();
            
            if (playerDataManager == null)
                playerDataManager = FindObjectOfType<PlayerDataManager>();
            
            if (prefabManager == null)
                prefabManager = FindObjectOfType<PrefabManager>();
            
            if (audioManager == null)
                audioManager = FindObjectOfType<AudioManager>();
            
            if (canvasManager == null)
                canvasManager = FindObjectOfType<CanvasManager>();
        }
        
        private void InstallCoreServices(DiContainer container)
        {
            // Game Manager - Core game state management
            if (gameManager != null)
            {
                container.Bind<GameManager>().FromInstance(gameManager);
                Debug.Log("[GameContextInstaller] Registered GameManager");
            }
            else
            {
                Debug.LogWarning("[GameContextInstaller] GameManager not found!");
            }
            
            // Turn Manager - Turn-based gameplay
            if (turnManager != null)
            {
                container.Bind<TurnManager>().FromInstance(turnManager);
                Debug.Log("[GameContextInstaller] Registered TurnManager");
            }
            else
            {
                Debug.LogWarning("[GameContextInstaller] TurnManager not found!");
            }
            
            // Game Board - Board state and tower management
            if (gameBoard != null)
            {
                container.Bind<GameBoard>().FromInstance(gameBoard);
                Debug.Log("[GameContextInstaller] Registered GameBoard");
            }
            else
            {
                Debug.LogWarning("[GameContextInstaller] GameBoard not found!");
            }
        }
        
        private void InstallManagers(DiContainer container)
        {
            // Command Manager - Command pattern implementation
            if (commandManager != null)
            {
                container.Bind<CommandManager>().FromInstance(commandManager);
                Debug.Log("[GameContextInstaller] Registered CommandManager");
            }
            else
            {
                Debug.LogWarning("[GameContextInstaller] CommandManager not found!");
            }
            
            // Player Data Manager - Player progression and data
            if (playerDataManager != null)
            {
                container.Bind<PlayerDataManager>().FromInstance(playerDataManager);
                Debug.Log("[GameContextInstaller] Registered PlayerDataManager");
            }
            else
            {
                Debug.LogWarning("[GameContextInstaller] PlayerDataManager not found!");
            }
            
            // Prefab Manager - Prefab instantiation
            if (prefabManager != null)
            {
                container.Bind<PrefabManager>().FromInstance(prefabManager);
                Debug.Log("[GameContextInstaller] Registered PrefabManager");
            }
            else
            {
                Debug.LogWarning("[GameContextInstaller] PrefabManager not found!");
            }
            
            // Canvas Manager - UI management
            if (canvasManager != null)
            {
                container.Bind<CanvasManager>().FromInstance(canvasManager);
                Debug.Log("[GameContextInstaller] Registered CanvasManager");
            }
            else
            {
                Debug.LogWarning("[GameContextInstaller] CanvasManager not found!");
            }
        }
        
        private void InstallSingletons(DiContainer container)
        {
            // Audio Manager - Audio system
            if (audioManager != null)
            {
                container.Bind<AudioManager>().FromInstance(audioManager);
                Debug.Log("[GameContextInstaller] Registered AudioManager");
            }
            else
            {
                Debug.LogWarning("[GameContextInstaller] AudioManager not found!");
            }
            
            // Game Services - Service locator (might be phased out in favor of DI)
            var gameServices = FindObjectOfType<GameServices>();
            if (gameServices != null)
            {
                container.Bind<GameServices>().FromInstance(gameServices);
                Debug.Log("[GameContextInstaller] Registered GameServices (compatibility)");
            }
        }
        
        private void InstallFactories(DiContainer container)
        {
            // Game Command Factory - Static class, no registration needed
            Debug.Log("[GameContextInstaller] GameCommandFactory is static, no registration needed");
            
            // Dice Manager Factory - Creates dice managers for players
            container.BindFactory<DiceManager[]>(container => 
            {
                var diceManagers = FindObjectsOfType<DiceManager>();
                return diceManagers;
            });
            Debug.Log("[GameContextInstaller] Registered DiceManager factory");
        }
        
        private void InstallMessageBus(DiContainer container)
        {
            // Message Bus - Event system
            container.Bind<MessageBus>().FromInstance(MessageBus.Instance);
            Debug.Log("[GameContextInstaller] Registered MessageBus");
        }
        
        private void OnValidate()
        {
            // Validate that all required services are assigned
            if (Application.isPlaying)
                return;
                
            ValidateServices();
        }
        
        private void ValidateServices()
        {
            bool allValid = true;
            
            if (gameManager == null)
            {
                Debug.LogWarning("[GameContextInstaller] GameManager not assigned!");
                allValid = false;
            }
            
            if (turnManager == null)
            {
                Debug.LogWarning("[GameContextInstaller] TurnManager not assigned!");
                allValid = false;
            }
            
            if (gameBoard == null)
            {
                Debug.LogWarning("[GameContextInstaller] GameBoard not assigned!");
                allValid = false;
            }
            
            if (commandManager == null)
            {
                Debug.LogWarning("[GameContextInstaller] CommandManager not assigned!");
                allValid = false;
            }
            
            if (!allValid && !autoFindServices)
            {
                Debug.LogError("[GameContextInstaller] Some required services are missing. Either assign them manually or enable 'Auto-Find Services'.");
            }
        }
    }
} 