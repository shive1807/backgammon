using UnityEngine;
using System.Collections;

/// <summary>
/// Enum representing the different phases of the game.
/// </summary>
public enum GameState
{
    Idle,
    Setup,
    RollDice,
    PlayerMove,
    SwitchTurn,
    GameOver
}

/// <summary>
/// GameManager controls the overall game state and transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")] 
    private GameState CurrentState { get; set; } = GameState.Idle;

    private TurnManager _turnManager;

    private void Awake()
    {
        // Ensure singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _turnManager = GetComponent<TurnManager>();
        
        if (_turnManager == null)
        {
            Debug.LogError("TurnManager not found on GameManager!");
        }
    }

    private void Start()
    {
        // Wait for GameServices to be ready before starting
        if (GameServices.Instance != null && GameServices.Instance.AreServicesReady())
        {
            TransitionToState(GameState.Setup);
        }
        else
        {
            // Delay start until GameServices is ready
            StartCoroutine(WaitForServicesAndStart());
        }
    }
    
    private IEnumerator WaitForServicesAndStart()
    {
        // Wait until GameServices is available and ready
        while (GameServices.Instance == null || !GameServices.Instance.AreServicesReady())
        {
            yield return null; // Wait one frame
        }
        
        TransitionToState(GameState.Setup);
    }

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.TurnOver>(OnTurnOver);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.TurnOver>(OnTurnOver);
    }

    private void OnTurnOver(CoreGameMessage.TurnOver message)
    {
        TransitionToState(GameState.SwitchTurn);
    }
    
    /// <summary>
    /// Transitions the game to a new state and triggers any entry logic.
    /// </summary>
    private void TransitionToState(GameState newState)
    {
        Debug.Log($"[GameManager] Transitioning from {CurrentState} to {newState}");
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Setup:
                SetupGame();
                break;

            case GameState.RollDice:
                // Use command pattern for dice rolling with null checks
                if (GameServices.Instance != null && GameServices.Instance.AreServicesReady() && CommandManager.Instance != null)
                {
                    var startTurnCommand = new StartTurnCommand(GameServices.Instance.TurnManager.GetCurrentTurn);
                    CommandManager.Instance.ExecuteCommand(startTurnCommand);
                }
                else
                {
                    // Fallback to original approach if GameServices not ready
                    Debug.LogWarning("GameServices not ready, falling back to direct message publishing");
                    MessageBus.Instance.Publish(new CoreGameMessage.TurnDiceSetupAndRoll(_turnManager.GetCurrentTurn));
                }
                break;

            case GameState.PlayerMove:
                EnablePlayerMove();
                break;

            case GameState.SwitchTurn:
                SwitchTurn();
                break;

            case GameState.GameOver:
                // Game over logic handled in Update or externally
                break;
        }
    }

    /// <summary>
    /// Publishes a game setup message and begins first turn.
    /// </summary>
    private void SetupGame()
    {
        MessageBus.Instance.Publish(new CoreGameMessage.GameSetup());
        TransitionToState(GameState.RollDice);
    }

    /// <summary>
    /// Called when the current player can make a move.
    /// </summary>
    private void EnablePlayerMove()
    {
        // Enable player controls here
    }

    /// <summary>
    /// Handles logic for switching to the next player's turn.
    /// </summary>
    private void SwitchTurn()
    {
        MessageBus.Instance.Publish(new CoreGameMessage.SwitchTurn());
        StartCoroutine(SetDelayedRollDice());
    }

    private IEnumerator SetDelayedRollDice()
    {
        yield return new WaitForSeconds(1f);
        TransitionToState(GameState.RollDice);
    }

    /// <summary>
    /// Provides access to the TurnManager.
    /// </summary>
    public TurnManager GetTurnManager() => _turnManager;
}
