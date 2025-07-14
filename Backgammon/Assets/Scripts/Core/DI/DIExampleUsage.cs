using UnityEngine;
using Core.DI;
using Core.GameAudio;
using Core.Context;

namespace Core.DI
{
    /// <summary>
    /// Example MonoBehaviour demonstrating dependency injection usage.
    /// Shows how to use the [Inject] attribute to get services from the DI container.
    /// </summary>
    public class DIExampleUsage : MonoBehaviour
    {
        [Header("Injected Services")]
        [Inject] private GameManager gameManager;
        [Inject] private TurnManager turnManager;
        [Inject] private GameBoard gameBoard;
        [Inject] private CommandManager commandManager;
        [Inject] private MessageBus messageBus;
        [Inject] private AudioManager audioManager;
        [Inject(optional: true)] private PlayerDataManager playerDataManager;
        
        [Header("Manual Injection")]
        [SerializeField] private bool performManualInjection = false;
        
        private void Start()
        {
            // Manual injection if needed
            if (performManualInjection)
            {
                PerformManualInjection();
            }
            
            // Automatic injection using MonoInjectHelper
            MonoInjectHelper.InjectIntoObject(this);
            
            // Test the injected services
            TestInjectedServices();
        }
        
        private void PerformManualInjection()
        {
            // Example of manual injection from ProjectContext
            var container = ProjectContext.Container;
            
            try
            {
                gameManager = container.Resolve<GameManager>();
                turnManager = container.Resolve<TurnManager>();
                gameBoard = container.Resolve<GameBoard>();
                commandManager = container.Resolve<CommandManager>();
                messageBus = container.Resolve<MessageBus>();
                audioManager = container.Resolve<AudioManager>();
                
                // Optional injection with try-catch
                try
                {
                    playerDataManager = container.Resolve<PlayerDataManager>();
                }
                catch (System.Exception)
                {
                    Debug.LogWarning("[DIExampleUsage] PlayerDataManager not available (optional)");
                }
                
                Debug.Log("[DIExampleUsage] Manual injection completed successfully!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DIExampleUsage] Manual injection failed: {e.Message}");
            }
        }
        
        private void TestInjectedServices()
        {
            Debug.Log("[DIExampleUsage] Testing injected services...");
            
            // Test GameManager
            if (gameManager != null)
            {
                Debug.Log($"[DIExampleUsage] GameManager injected successfully: {gameManager.GetType().Name}");
            }
            else
            {
                Debug.LogError("[DIExampleUsage] GameManager injection failed!");
            }
            
            // Test TurnManager
            if (turnManager != null)
            {
                Debug.Log($"[DIExampleUsage] TurnManager injected successfully: Current turn = {turnManager.GetCurrentTurn}");
            }
            else
            {
                Debug.LogError("[DIExampleUsage] TurnManager injection failed!");
            }
            
            // Test GameBoard
            if (gameBoard != null)
            {
                Debug.Log($"[DIExampleUsage] GameBoard injected successfully: {gameBoard.towers.Count} towers");
            }
            else
            {
                Debug.LogError("[DIExampleUsage] GameBoard injection failed!");
            }
            
            // Test CommandManager
            if (commandManager != null)
            {
                Debug.Log($"[DIExampleUsage] CommandManager injected successfully: {commandManager.GetType().Name}");
            }
            else
            {
                Debug.LogError("[DIExampleUsage] CommandManager injection failed!");
            }
            
            // Test MessageBus
            if (messageBus != null)
            {
                Debug.Log($"[DIExampleUsage] MessageBus injected successfully: {messageBus.GetType().Name}");
            }
            else
            {
                Debug.LogError("[DIExampleUsage] MessageBus injection failed!");
            }
            
            // Test AudioManager (optional)
            if (audioManager != null)
            {
                Debug.Log($"[DIExampleUsage] AudioManager injected successfully: {audioManager.GetType().Name}");
            }
            else
            {
                Debug.LogWarning("[DIExampleUsage] AudioManager not available");
            }
            
            // Test PlayerDataManager (optional)
            if (playerDataManager != null)
            {
                Debug.Log($"[DIExampleUsage] PlayerDataManager injected successfully: {playerDataManager.Data.playerName}");
            }
            else
            {
                Debug.LogWarning("[DIExampleUsage] PlayerDataManager not available (optional)");
            }
        }
        
        [System.Obsolete("Example of using injected services in game logic")]
        private void ExampleGameLogic()
        {
            // Example: Use injected services in game logic
            if (gameManager != null && turnManager != null)
            {
                // Get current player
                int currentPlayer = turnManager.GetCurrentTurn;
                
                // Publish a message using injected MessageBus
                if (messageBus != null)
                {
                    messageBus.Publish(new CoreGameMessage.TurnOver());
                }
                
                // Play audio using injected AudioManager
                if (audioManager != null)
                {
                    // audioManager.PlaySound("turnEnd");
                }
            }
        }
        
        /// <summary>
        /// Example of how to inject into a dynamically created GameObject
        /// </summary>
        public void InjectIntoNewGameObject()
        {
            // Create a new GameObject
            var newObj = new GameObject("Dynamic Object");
            var newComponent = newObj.AddComponent<DIExampleUsage>();
            
            // Inject dependencies into the new component
            MonoInjectHelper.InjectIntoObject(newComponent);
            
            Debug.Log("[DIExampleUsage] Injected into dynamically created GameObject");
        }
        
        /// <summary>
        /// Example of how to inject into all children of a GameObject
        /// </summary>
        public void InjectIntoChildren()
        {
            // Inject into all child GameObjects
            MonoInjectHelper.InjectIntoGameObject(gameObject);
            
            Debug.Log("[DIExampleUsage] Injected into all children");
        }
    }
} 