using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a single tower (point) on the Backgammon board.
/// Responsible for managing a stack of checkers and tracking ownership.
/// </summary>
public class Tower : MonoBehaviour
{
    // The index of this tower on the board (0 to 23)
    public int TowerIndex { get; private set; }

    // Stack to hold the checkers currently on this tower
    private Stack<Coin> Coins { get; set; } = new Stack<Coin>();
    private Stack<Ring> Rings { get; set; } = new Stack<Ring>();

    // ID of the player who currently owns this tower (-1 means unoccupied)
    private int OwnerPlayerId { get; set; } = -1;
    
    public int GetOwnerPlayerId() => OwnerPlayerId;
    
    // Offset applied for each additional checker (for stacking visuals)
    private const float CheckerOffsetY = 0.2f;

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.CleanTowerRings>(OnCleanTowerRing);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.CleanTowerRings>(OnCleanTowerRing);
    }


    /// <summary>
    /// Sets the tower's index (usually done at initialization time).
    /// </summary>
    /// <param name="index">Tower index from 0 to 23.</param>
    public void Initialize(int index)
    {
        TowerIndex = index;
    }

    /// <summary>
    /// Adds a checker to this tower.
    /// Sets the tower's ownership if it's the first checker.
    /// Prevents adding an opponent's checker if it's already owned.
    /// </summary>
    /// <param name="playerId">The ID of the player owning the checker.</param>
    public void AddChecker(int playerId)
    {
        var coinObject = Instantiate(PrefabManager.Instance.GetPrefab(GameSettings.CoinPrefab), transform, true);
        var coin       = coinObject.GetComponent<Coin>();
        
        coinObject.name = playerId == 0 ? $"WhiteChecker_{TowerIndex}" : $"BlackChecker_{TowerIndex}";
        coinObject.GetComponent<Renderer>().material.color = playerId == 0 ? Color.white : Color.gray;


        // If the tower is empty, assign ownership
        if (Coins.Count == 0)
        {
            OwnerPlayerId = playerId;
        }
        
        // Disallow placing opponent's checker if already owned
        else if (OwnerPlayerId != playerId)
        {
            Debug.LogWarning("Attempted to add opponent's checker to this tower.");
            return;
        }

        coin.SetCoin(OwnerPlayerId, TowerIndex);
        // Add to the checker stack
        Coins.Push(coin);


        // Determine a stacking direction based on index
        var direction = TowerIndex <= 11 ? Vector3.up : Vector3.down;

        // Position the checker visually based on stack height and direction
        var newPos = transform.position + direction * CheckerOffsetY * (Coins.Count - 1);
        coinObject.transform.position = newPos;
    }

    public void AddCoin(Coin coin)
    {
        Coins.Push(coin);
        var direction = TowerIndex <= 11 ? Vector3.up : Vector3.down;
        var newPos = transform.position + direction * CheckerOffsetY * (Coins.Count - 1);
        coin.gameObject.transform.position = newPos;
        coin.UpdateCoinTower(TowerIndex, true);
    }

    public void ResetCoin(Coin coin)
    {
        Coins.Push(coin);
        var direction = TowerIndex <= 11 ? Vector3.up : Vector3.down;
        var newPos = transform.position + direction * CheckerOffsetY * (Coins.Count - 1);
        coin.gameObject.transform.position = newPos;
        coin.UpdateCoinTower(TowerIndex, false);
    }

    public int GetListOfMovedCoins() => Coins.Count(coin => coin.GetIsMovedInCurrentTurn());

    public Coin RemoveCoin(Coin coin)
    {
        return Coins.Pop();
    }

    public void HighlightTopCoin()
    {
        var coin = Coins.Peek();
        coin.Highlight();
    }
    
    public void AddRing(int playerId, int sourceTowerIndex, int currentTowerIndex)
    {
        var ringObject = Instantiate(PrefabManager.Instance.GetPrefab(GameSettings.RingPrefab), transform, true);
        var ring = ringObject.GetComponent<Ring>();

        // If the tower is empty, assign ownership
        if (Coins.Count == 0)
        {
            OwnerPlayerId = playerId;
        }
        // Disallow placing opponent's checker if already owned
        else if (OwnerPlayerId != playerId)
        {
            // Debug.LogWarning("Attempted to add opponent's checker to this tower.");
            //return;
        }

        // r.SetCurrentTower(this);
        
        // Add to the checker stack
        Rings.Push(ring);
        ring.SetCurrentTower(sourceTowerIndex, currentTowerIndex, OwnerPlayerId == playerId);
        // Make it a child of this tower for scene hierarchy organization

        // Determine a stacking direction based on index
        var direction = TowerIndex <= 11 ? Vector3.up : Vector3.down;

        // Position the checker visually based on stack height and direction
        var newPos = transform.position + direction * CheckerOffsetY * (Coins.Count - 1);
        ringObject.transform.position = newPos;
    }

    private void OnCleanTowerRing(CoreGameMessage.CleanTowerRings message)
    {
        foreach (var ring in Rings)
        {
            Destroy(ring.gameObject);
        }
        
        Rings.Clear();
    }

    /// <summary>
    /// Removes and returns the top checker from this tower.
    /// Clears ownership if the tower becomes empty.
    /// </summary>
    /// <returns>The removed checker GameObject, or null if empty.</returns>
    public Coin RemoveTopChecker()
    {
        if (Coins.Count == 0)
            return null;

        Coin topChecker = Coins.Pop();

        // Reset ownership if the tower is now empty
        if (Coins.Count == 0)
        {
            OwnerPlayerId = -1;
        }

        return topChecker;
    }
    /// <summary>
    /// Gets the number of checkers currently on the tower.
    /// </summary>
    public int CheckerCount => Coins.Count;

    /// <summary>
    /// Checks if the tower is currently owned by the given player.
    /// </summary>
    /// <param name="playerId">Player ID to check against.</param>
    /// <returns>True if owned by that player, false otherwise.</returns>
    public bool IsOwnedBy(int playerId) => OwnerPlayerId == playerId;

    /// <summary>
    /// Checks whether the tower is empty.
    /// </summary>
    public bool IsEmpty() => Coins.Count == 0;

    /// <summary>
    /// Clears all checkers from the tower (used during reset).
    /// </summary>
    public void ClearTower()
    {
        foreach (var checker in Coins)
        {
            Destroy(checker); // Remove from scene
        }

        Coins.Clear();
        OwnerPlayerId = -1;
    }
}
