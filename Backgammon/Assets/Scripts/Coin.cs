using System;
using UnityEngine;

/// <summary>
/// Represents a Coin in the game that can be clicked by the player.
/// </summary>
public class Coin : MonoBehaviour
{
    private int _prevTower;
    private int _currentTower; // ID or index of the tower this coin is currently on
    private int _ownerId;      // ID of the player who owns this coin
    private bool _isMovedInCurrentTurn;
    
    [SerializeField]
    private SpriteRenderer highlightRenderer;

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.OnCoinMoved>(OnCheckerMoved);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.OnCoinMoved>(OnCheckerMoved);
    }

    /// <summary>
    /// Initializes the coin with an owner and tower.
    /// </summary>
    /// <param name="ownerId">The player ID who owns this coin.</param>
    /// <param name="tower">The ID or index of the tower this coin is on.</param>
    public void SetCoin(int ownerId, int tower)
    {
        _prevTower    = -1;
        _ownerId      = ownerId;
        _currentTower = tower;
        highlightRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Handle mouse/touch input on left click or tap
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouchOrClick();
        }
    }

    /// <summary>
    /// Detects if the coin was clicked or tapped and handles interaction logic.
    /// </summary>
    private void HandleTouchOrClick()
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("No main camera found in the scene.");
            return;
        }

        // Convert mouse position to world point and raycast at that position
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        // If nothing is hit or it's not this coin, ignore
        if (hit.collider == null || hit.collider.gameObject != gameObject)
            return;

        Debug.Log($"✅ Coin {gameObject.name} clicked!");

        // Ensure only the owner can interact with the coin
        if (_ownerId != GameManager.Instance.GetTurnManager().GetCurrentTurn)
            return;

        // Publish an event to the message bus indicating this coin was clicked
        MessageBus.Instance.Publish(new CoreGameMessage.CoinClicked(_ownerId, _currentTower));
        Debug.Log($"Coin {gameObject.name} is currently on Tower: {_currentTower}");
    }

    /// <summary>
    /// Updates the coin current tower.
    /// </summary>
    public void UpdateCoinTower(int targetTower, bool isMovedInCurrentTurn)
    {
        _prevTower    = _currentTower;
        _currentTower = targetTower;
        _isMovedInCurrentTurn = isMovedInCurrentTurn;
    }
    
    public int GetPrevTower() => _prevTower;
    public int GetCurrentTower() => _currentTower;
    
    public bool GetIsMovedInCurrentTurn() => _isMovedInCurrentTurn;
    
    public void Highlight(bool show = true)
    {
        highlightRenderer.gameObject.SetActive(show);
    }

    private void OnCheckerMoved(CoreGameMessage.OnCoinMoved message)
    {
        Highlight(false);
    }
}