using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public enum TowerType
{
    White,
    Black,
    Empty
}

/// <summary>
/// Represents a single tower (point) on the Backgammon board.
/// Responsible for managing a stack of checkers and tracking ownership.
/// </summary>
public class Tower : MonoBehaviour
{
    // The index of this tower on the board (0 to 23)
    public int TowerIndex { get; private set; }

    private TowerType TowerType { get; set; }

    // Stack to hold the checkers currently on this tower
    private Stack<Coin> Coins { get; set; } = new Stack<Coin>();
    private Stack<Ring> Rings { get; set; } = new Stack<Ring>();

    // ID of the player who currently owns this tower (-1 means unoccupied)
    private int OwnerPlayerId { get; set; } = -1;
    
    public int GetOwnerPlayerId() => OwnerPlayerId;
    
    public TowerType GetTowerType() => TowerType;
    
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
        TowerType  = TowerType.Empty;
    }

    /// <summary>
    /// Adds a checker to this tower.
    /// Sets the tower's ownership if it's the first checker.
    /// Prevents adding an opponent's checker if it's already owned.
    /// </summary>
    /// <param name="playerId">The ID of the player owning the checker.</param>
    public void AddInitCoins(int playerId)
    {
        var coinObject = Instantiate(PrefabManager.Instance.GetPrefab(GameSettings.CoinPrefab), transform, true);
        var coin       = coinObject.GetComponent<Coin>();
        
        coinObject.name = playerId == 0 ? $"WhiteChecker_{TowerIndex}" : $"BlackChecker_{TowerIndex}";
        coinObject.GetComponent<Renderer>().material.color = playerId == 0 ? Color.white : Color.gray;


        // If the tower is empty, assign ownership
        if (Coins.Count == 0)
        {
            OwnerPlayerId = playerId;
            TowerType = playerId == 0 ? TowerType.White : TowerType.Black;
            Debug.Log($"Tower #{TowerType} has been initialized.");
        }

        coin.SetInitCoin(OwnerPlayerId, TowerIndex);
        // Add to the coin stack
        Coins.Push(coin);

        // Determine a stacking direction based on index
        var direction = TowerIndex <= 11 ? Vector3.up : Vector3.down;

        // Position the checker visually based on stack height and direction
        var newPos = transform.position + direction * CheckerOffsetY * (Coins.Count - 1);
        coinObject.transform.position = newPos;
    }

    public void AddCoin(Coin coin)
    {
        // Update tower ownership if empty or if this is the only coin
        if (TowerType == TowerType.Empty || Coins.Count == 0)
        {
            OwnerPlayerId = coin.GetOwnerId();
            TowerType = coin.GetCoinType() == CoinType.White ? TowerType.White : TowerType.Black;
        }
        
        Coins.Push(coin);
        var direction = TowerIndex <= 11 ? Vector3.up : Vector3.down;
        var newPos = transform.position + direction * CheckerOffsetY * (Coins.Count - 1);
        coin.gameObject.transform.position = newPos;
        coin.UpdateCoinTower(TowerIndex, true);
    }

    // ResetCoin removed - Command Pattern handles undo functionality

    // GetListOfMovedCoins removed - Command Pattern handles move tracking

    public void HighlightTopCoin()
    {
        if (Coins.Count <= 0) return;
        
        var coin = Coins.Peek();
        coin.Highlight();
    }
    
    /// <summary>
    /// Get the top coin without removing it
    /// </summary>
    public Coin GetTopCoin()
    {
        return Coins.Count > 0 ? Coins.Peek() : null;
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
        
        // Add to the ring stack
        Rings.Push(ring);
        ring.SetCurrentTower(sourceTowerIndex, currentTowerIndex, OwnerPlayerId == playerId || CanAttack());

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

        var topChecker = Coins.Pop();

        // Reset ownership if the tower is now empty
        if (Coins.Count == 0)
        {
            OwnerPlayerId = -1;
        }

        return topChecker;
    }
    
    /// <summary>
    /// Removes a specific coin from this tower (needed for undo functionality with attacks).
    /// Rebuilds the stack without the specified coin.
    /// </summary>
    /// <param name="coinToRemove">The specific coin to remove</param>
    /// <returns>True if the coin was found and removed, false otherwise</returns>
    public bool RemoveSpecificCoin(Coin coinToRemove)
    {
        if (Coins.Count == 0 || coinToRemove == null)
            return false;
            
        // Convert stack to list for easier manipulation
        var coinList = new List<Coin>(Coins);
        
        // Find the coin to remove
        bool found = false;
        for (int i = 0; i < coinList.Count; i++)
        {
            if (coinList[i] == coinToRemove)
            {
                coinList.RemoveAt(i);
                found = true;
                break;
            }
        }
        
        if (!found)
            return false;
            
        // Rebuild the stack without the removed coin
        Coins.Clear();
        
        // Add coins back in reverse order to maintain stack structure
        for (int i = coinList.Count - 1; i >= 0; i--)
        {
            Coins.Push(coinList[i]);
        }
        
        // Update positions for remaining coins
        UpdateCoinPositions();
        
        // Reset ownership if the tower is now empty
        if (Coins.Count == 0)
        {
            OwnerPlayerId = -1;
            TowerType = TowerType.Empty;
        }
        
        return true;
    }
    
    /// <summary>
    /// Updates the visual positions of all coins in the tower
    /// </summary>
    private void UpdateCoinPositions()
    {
        var coinArray = Coins.ToArray();
        var direction = TowerIndex <= 11 ? Vector3.up : Vector3.down;
        
        for (int i = 0; i < coinArray.Length; i++)
        {
            var coin = coinArray[coinArray.Length - 1 - i]; // Reverse order since stack is LIFO
            var newPos = transform.position + direction * CheckerOffsetY * i;
            coin.gameObject.transform.position = newPos;
        }
    }
    /// <summary>
    /// Gets the number of checkers currently on the tower.
    /// </summary>
    public int CoinsCount => Coins.Count;

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
    /// Checks whether the tower is available to Attack.
    /// </summary>
    public bool CanAttack() => Coins.Count == 1;
}
