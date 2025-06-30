using UnityEngine;

public static class GameSettings
{
    public const int NumberOfPlayers = 2;
    
    public const string CoinPrefab = "Coin";
    public const string RingPrefab = "Ring";
    
    // --------------------------
    // Player 0 Initial Setup
    // --------------------------
    public const int TowerIndex_TopRight     = 23;
    public const int TowerIndex_MiddleLeft   = 12;
    public const int TowerIndex_Center       = 7;
    public const int TowerIndex_BottomLeft   = 5;

    public const int Coins_TopRight     = 2;
    public const int Coins_MiddleLeft   = 5;
    public const int Coins_Center       = 3;
    public const int Coins_BottomLeft   = 5;

    public const int Player0 = 0;

    // --------------------------
    // Player 1 Initial Setup
    // --------------------------
    public const int TowerIndex_TopLeft      = 0;
    public const int TowerIndex_MiddleRight  = 11;
    public const int TowerIndex_CenterRight  = 16;
    public const int TowerIndex_BottomRight  = 18;

    public const int Coins_TopLeft      = 2;
    public const int Coins_MiddleRight  = 5;
    public const int Coins_CenterRight  = 3;
    public const int Coins_BottomRight  = 5;

    public const int Player1 = 1;

    // Optional loopable data
    public static readonly InitialCoinData[] InitialCoinsSetup =
    {
        // Player 0
        new InitialCoinData(TowerIndex_TopRight,    Coins_TopRight,    Player0),
        new InitialCoinData(TowerIndex_MiddleLeft,  Coins_MiddleLeft,  Player0),
        new InitialCoinData(TowerIndex_Center,      Coins_Center,      Player0),
        new InitialCoinData(TowerIndex_BottomLeft,  Coins_BottomLeft,  Player0),

        // Player 1
        new InitialCoinData(TowerIndex_TopLeft,      Coins_TopLeft,      Player1),
        new InitialCoinData(TowerIndex_MiddleRight,  Coins_MiddleRight,  Player1),
        new InitialCoinData(TowerIndex_CenterRight,  Coins_CenterRight,  Player1),
        new InitialCoinData(TowerIndex_BottomRight,  Coins_BottomRight,  Player1),
    };

    public struct InitialCoinData
    {
        public int TowerIndex;
        public int CoinCount;
        public int PlayerId;

        public InitialCoinData(int towerIndex, int coinCount, int playerId)
        {
            TowerIndex = towerIndex;
            CoinCount = coinCount;
            PlayerId = playerId;
        }
    }
}