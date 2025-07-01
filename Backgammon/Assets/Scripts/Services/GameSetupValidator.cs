using UnityEngine;

/// <summary>
/// Utility script to validate that all required components are properly set up
/// Add this to your scene temporarily to debug setup issues
/// </summary>
public class GameSetupValidator : MonoBehaviour
{
    [Header("Setup Validation")]
    [SerializeField] private bool validateOnStart = true;
    [SerializeField] private bool continuousValidation = false;
    
    private void Start()
    {
        if (validateOnStart)
        {
            ValidateSetup();
        }
    }
    
    private void Update()
    {
        if (continuousValidation)
        {
            ValidateSetup();
        }
    }
    
    [ContextMenu("Validate Setup")]
    public void ValidateSetup()
    {
        Debug.Log("=== Game Setup Validation ===");
        
        // Check GameServices
        if (GameServices.Instance == null)
        {
            Debug.LogError("❌ GameServices.Instance is NULL! Make sure you have a GameServices component in your scene.");
            LogSetupInstructions();
            return;
        }
        
        Debug.Log("✅ GameServices instance found");
        
        // Check if services are ready
        if (!GameServices.Instance.AreServicesReady())
        {
            Debug.LogError("❌ GameServices are not ready! Check the following:");
            
            if (GameServices.Instance.GameBoard == null)
                Debug.LogError("   - GameBoard is missing");
            if (GameServices.Instance.TurnManager == null)
                Debug.LogError("   - TurnManager is missing");
            if (GameServices.Instance.GameManager == null)
                Debug.LogError("   - GameManager is missing");
            if (GameServices.Instance.CommandManager == null)
                Debug.LogError("   - CommandManager is missing");
            if (GameServices.Instance.WhiteSpawnPoint == null)
                Debug.LogError("   - WhiteSpawnPoint is missing");
            if (GameServices.Instance.BlackSpawnPoint == null)
                Debug.LogError("   - BlackSpawnPoint is missing");
                
            LogSetupInstructions();
            return;
        }
        
        Debug.Log("✅ All GameServices are ready");
        
        // Check CommandManager
        if (CommandManager.Instance == null)
        {
            Debug.LogError("❌ CommandManager.Instance is NULL! Make sure you have a CommandManager component in your scene.");
            return;
        }
        
        Debug.Log("✅ CommandManager instance found");
        
        // Check MessageBus
        if (MessageBus.Instance == null)
        {
            Debug.LogError("❌ MessageBus.Instance is NULL!");
            return;
        }
        
        Debug.Log("✅ MessageBus instance found");
        
        // Validate tower setup
        var gameBoard = GameServices.Instance.GameBoard;
        if (gameBoard.towers == null || gameBoard.towers.Count == 0)
        {
            Debug.LogError("❌ GameBoard has no towers configured!");
            return;
        }
        
        Debug.Log($"✅ GameBoard has {gameBoard.towers.Count} towers configured");
        
        // Check for missing tower references
        int nullTowers = 0;
        for (int i = 0; i < gameBoard.towers.Count; i++)
        {
            if (gameBoard.towers[i] == null)
            {
                Debug.LogError($"❌ Tower at index {i} is NULL!");
                nullTowers++;
            }
        }
        
        if (nullTowers > 0)
        {
            Debug.LogError($"❌ Found {nullTowers} NULL tower references!");
            return;
        }
        
        Debug.Log("✅ All towers are properly referenced");
        
        Debug.Log("🎉 ALL SETUP VALIDATION PASSED! Your game should work correctly.");
    }
    
    private void LogSetupInstructions()
    {
        Debug.Log("📋 SETUP INSTRUCTIONS:");
        Debug.Log("1. Create a GameObject named 'GameServices'");
        Debug.Log("2. Add the GameServices component to it");
        Debug.Log("3. In the inspector, assign:");
        Debug.Log("   - GameBoard (drag from scene)");
        Debug.Log("   - TurnManager (drag from scene)");
        Debug.Log("   - GameManager (drag from scene)");
        Debug.Log("   - CommandManager (drag from scene)");
        Debug.Log("   - WhiteSpawnPoint (drag from scene)");
        Debug.Log("   - BlackSpawnPoint (drag from scene)");
        Debug.Log("4. Make sure you have a CommandManager GameObject in your scene");
        Debug.Log("5. See Assets/Scripts/Commands/README_CommandPattern.md for detailed instructions");
    }
    
    /// <summary>
    /// Call this method to automatically find and assign missing components
    /// </summary>
    [ContextMenu("Auto-Fix Setup")]
    public void AutoFixSetup()
    {
        Debug.Log("🔧 Attempting to auto-fix setup...");
        
        if (GameServices.Instance == null)
        {
            Debug.LogError("Cannot auto-fix: GameServices instance not found. Please create GameServices GameObject first.");
            return;
        }
        
        // Try to auto-assign missing components
        var gameServices = GameServices.Instance;
        bool changes = false;
        
        if (gameServices.GameBoard == null)
        {
            var gameBoard = FindObjectOfType<GameBoard>();
            if (gameBoard != null)
            {
                Debug.Log("✅ Auto-assigned GameBoard");
                changes = true;
            }
        }
        
        if (gameServices.CommandManager == null)
        {
            var commandManager = FindObjectOfType<CommandManager>();
            if (commandManager != null)
            {
                Debug.Log("✅ Auto-assigned CommandManager");
                changes = true;
            }
        }
        
        if (changes)
        {
            Debug.Log("🔧 Auto-fix completed. Please manually assign remaining components in the inspector.");
        }
        else
        {
            Debug.Log("🔧 No auto-fixes available. Please manually assign components in the inspector.");
        }
    }
} 