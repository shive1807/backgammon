using UnityEngine;
using System.Collections.Generic;

public enum GameState
{
    Setup,
    RollDice,
    PlayerMove,
    SwitchTurn,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState;

    public Dice dice1;  // Reference to your Dice component
    public Dice dice2;
    public Coin selectedCoin; // The coin selected for movement
    public List<Coin> player1Coins;
    public List<Coin> player2Coins;

    private int[] diceValues;
    private int currentPlayer = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
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
                    diceValues = new [] { dice1.Roll(), dice1.Roll() };
                    Debug.Log($"Player {currentPlayer} rolled: {diceValues[0]} and {diceValues[1]}");
                    TransitionToState(GameState.PlayerMove);
                }
                break;

            case GameState.PlayerMove:
                // Wait for coin selection and validate move
                break;

            case GameState.SwitchTurn:
                currentPlayer = 3 - currentPlayer; // Toggle 1 <-> 2
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
        Debug.Log($"Player {currentPlayer} can now move using dice {diceValues[0]} and {diceValues[1]}");
        // Wait for coin interaction through Coin.cs
    }

    public void CoinMoved()
    {
        Debug.Log("Coin moved successfully.");
        TransitionToState(GameState.SwitchTurn);
    }

    public bool IsPlayerTurn(int playerId)
    {
        return playerId == currentPlayer;
    }

    public int[] GetDiceValues()
    {
        return diceValues;
    }

    public int CurrentPlayer => currentPlayer;
}
