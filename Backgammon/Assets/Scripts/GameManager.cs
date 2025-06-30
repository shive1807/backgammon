using UnityEngine;
using System.Collections.Generic;

public enum GameState
{
    Idle,
    Setup,
    RollDice,
    PlayerMove,
    SwitchTurn,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState = GameState.Idle;

    private List<int> _diceValues;
    private int _currentPlayer = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        TransitionToState(GameState.Setup);
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.Setup:
                break;

            case GameState.RollDice:
                // Could auto-roll or wait for player to tap
                if (Input.GetKeyDown(KeyCode.Space)) // Replace with button tap
                {
                    _diceValues = CanvasManager.Instance.ShuffleDiceAndReturnValues(_currentPlayer);
                    Debug.Log($"Player {_currentPlayer} rolled: {_diceValues[0]} and {_diceValues[1]}");
                    TransitionToState(GameState.PlayerMove);
                }
                break;

            case GameState.PlayerMove:
                // Wait for coin selection and validate move
                break;

            case GameState.SwitchTurn:
                _currentPlayer++; // Toggle 1 <-> 2
                _currentPlayer %= 2;
                TransitionToState(GameState.RollDice);
                break;

            case GameState.GameOver:
                Debug.Log("Game Over!");
                break;
        }
    }

    public void TransitionToState(GameState newState)
    {
        Debug.Log($"Transitioning to {newState}...");
        currentState = newState;

        switch (newState)
        {
            case GameState.Setup:
                SetupGame();
                break;

            case GameState.RollDice:
                CanvasManager.Instance.SetPlayerDice();
                break;

            case GameState.PlayerMove:
                AllowPlayerToMove();
                break;

            case GameState.SwitchTurn:
                TransitionToState(GameState.SwitchTurn);
                break;
        }
    }

    void SetupGame()
    {
        // Initialize board, spawn coins, etc.
        TransitionToState(GameState.RollDice);
    }

    void AllowPlayerToMove()
    {
        Debug.Log($"Player {_currentPlayer} can now move using dice {_diceValues[0]} and {_diceValues[1]}");
        // Wait for coin interaction through Coin.cs
    }

    public void CoinMoved()
    {
        Debug.Log("Coin moved successfully.");
        TransitionToState(GameState.SwitchTurn);
    }

    public bool IsPlayerTurn(int playerId)
    {
        return playerId == _currentPlayer;
    }

    public List<int> GetDiceValues()
    {
        return _diceValues;
    }

    public List<int> RemovePlayedValuesFromDice(int playedValue)
    {
        _diceValues.Remove(playedValue);
        return _diceValues;
    }

    public int CurrentPlayer => _currentPlayer;
}
