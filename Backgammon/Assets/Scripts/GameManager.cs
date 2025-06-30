using UnityEngine;

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
        TransitionToState(GameState.Setup);
    }

    private void Update()
    {
        HandleStateUpdate();
    }

    /// <summary>
    /// Main state update handler called every frame.
    /// </summary>
    private void HandleStateUpdate()
    {
        switch (CurrentState)
        {
            case GameState.RollDice:
                // Simulate dice roll on space key press (replace with UI input)
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TransitionToState(GameState.PlayerMove);
                }
                break;

            case GameState.PlayerMove:
                // Await player interaction (e.g., selecting and moving a coin)
                break;

            case GameState.GameOver:
                // Show Game Over UI or handle cleanup
                Debug.Log("Game Over!");
                break;
        }
    }

    /// <summary>
    /// Transitions the game to a new state and triggers any entry logic.
    /// </summary>
    public void TransitionToState(GameState newState)
    {
        Debug.Log($"[GameManager] Transitioning from {CurrentState} to {newState}");
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Setup:
                SetupGame();
                break;

            case GameState.RollDice:
                MessageBus.Instance.Publish(new CoreGameMessage.TurnStartDice(_turnManager.GetCurrentTurn));
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
        TransitionToState(GameState.RollDice);
    }

    /// <summary>
    /// Provides access to the TurnManager.
    /// </summary>
    public TurnManager GetTurnManager() => _turnManager;
}
