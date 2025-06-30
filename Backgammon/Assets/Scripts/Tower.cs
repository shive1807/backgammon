using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single tower (point) on the Backgammon board.
/// Responsible for managing a stack of checkers and tracking ownership.
/// </summary>
public class Tower : MonoBehaviour
{
    // The index of this tower on the board (0 to 23)
    public int TowerIndex { get; private set; }

    // Stack to hold the checkers currently on this tower
    private Stack<GameObject> Checkers { get; set; } = new Stack<GameObject>();
    private Stack<GameObject> Rings { get; set; } = new Stack<GameObject>();

    // ID of the player who currently owns this tower (-1 means unoccupied)
    private int OwnerPlayerId { get; set; } = -1;
    
    public int GetOwnerPlayerId() => OwnerPlayerId;
    
    // Offset applied for each additional checker (for stacking visuals)
    private const float CheckerOffsetY = 0.2f;

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.CoinClicked>(OnCoinClicked);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.CoinClicked>(OnCoinClicked);
    }

    private void OnCoinClicked(CoreGameMessage.CoinClicked message)
    {
        var coinClicked = message.Coin;
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
    /// <param name="checker">The checker GameObject to add.</param>
    /// <param name="playerId">The ID of the player owning the checker.</param>
    public void AddChecker(GameObject checker, int playerId)
    {
        // If the tower is empty, assign ownership
        if (Checkers.Count == 0)
        {
            OwnerPlayerId = playerId;
        }
        // Disallow placing opponent's checker if already owned
        else if (OwnerPlayerId != playerId)
        {
            Debug.LogWarning("Attempted to add opponent's checker to this tower.");
            return;
        }
        
        checker.GetComponent<Coin>().SetCoin(OwnerPlayerId, TowerIndex);
        // Add to the checker stack
        Checkers.Push(checker);

        // Make it a child of this tower for scene hierarchy organization
        checker.transform.SetParent(transform);

        // Determine a stacking direction based on index
        Vector3 direction = TowerIndex <= 11 ? Vector3.up : Vector3.down;

        // Position the checker visually based on stack height and direction
        Vector3 newPos = transform.position + direction * CheckerOffsetY * (Checkers.Count - 1);
        checker.transform.position = newPos;
    }
    
    public void AddRing(GameObject ring, int playerId, Tower sourceTower)
    {
        // If the tower is empty, assign ownership
        if (Checkers.Count == 0)
        {
            OwnerPlayerId = playerId;
        }
        // Disallow placing opponent's checker if already owned
        else if (OwnerPlayerId != playerId)
        {
            // Debug.LogWarning("Attempted to add opponent's checker to this tower.");
            // return;
        }
        
        ring.GetComponent<Ring>().SetCurrentTower(this, sourceTower);
        
        // Add to the checker stack
        Rings.Push(ring);

        // Make it a child of this tower for scene hierarchy organization
        ring.transform.SetParent(transform);

        // Determine a stacking direction based on index
        Vector3 direction = TowerIndex <= 11 ? Vector3.up : Vector3.down;

        // Position the checker visually based on stack height and direction
        Vector3 newPos = transform.position + direction * CheckerOffsetY * (Checkers.Count - 1);
        ring.transform.position = newPos;
    }

    /// <summary>
    /// Removes and returns the top checker from this tower.
    /// Clears ownership if the tower becomes empty.
    /// </summary>
    /// <returns>The removed checker GameObject, or null if empty.</returns>
    public GameObject RemoveTopChecker()
    {
        if (Checkers.Count == 0)
            return null;

        GameObject topChecker = Checkers.Pop();

        // Reset ownership if the tower is now empty
        if (Checkers.Count == 0)
        {
            OwnerPlayerId = -1;
        }

        return topChecker;
    }

    /// <summary>
    /// Peeks at the top checker without removing it.
    /// </summary>
    /// <returns>The top checker GameObject, or null if none.</returns>
    public GameObject PeekTopChecker()
    {
        return Checkers.Count > 0 ? Checkers.Peek() : null;
    }

    /// <summary>
    /// Gets the number of checkers currently on the tower.
    /// </summary>
    public int CheckerCount => Checkers.Count;

    /// <summary>
    /// Checks if the tower is currently owned by the given player.
    /// </summary>
    /// <param name="playerId">Player ID to check against.</param>
    /// <returns>True if owned by that player, false otherwise.</returns>
    public bool IsOwnedBy(int playerId) => OwnerPlayerId == playerId;

    /// <summary>
    /// Checks whether the tower is empty.
    /// </summary>
    public bool IsEmpty() => Checkers.Count == 0;

    /// <summary>
    /// Clears all checkers from the tower (used during reset).
    /// </summary>
    public void ClearTower()
    {
        foreach (var checker in Checkers)
        {
            Destroy(checker); // Remove from scene
        }

        Checkers.Clear();
        OwnerPlayerId = -1;
    }
}
