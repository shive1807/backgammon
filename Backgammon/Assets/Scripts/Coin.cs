using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Reference to the tower this coin is currently placed on
    public Tower currentTower;

    private Camera _mainCamera;

    private int OwnerId;
    
    // C# Event: Raised when this coin is clicked
    public event Action<Tower> OnCoinClicked;


    private void Start()
    {
        _mainCamera = Camera.main;
        Debug.Log($"Coin {gameObject.name} initialized.");
    }

    public void SetOwner(int ownerId)
    {
        OwnerId = ownerId;
    }

    void Update()
    {
        // Handle touch or mouse click
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouchOrClick();
        }
    }

    void HandleTouchOrClick()
    {
        Vector2 worldPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("2D Ray hit: " + hit.collider.gameObject.name);

            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log($"✅ Coin {gameObject.name} clicked!");
                if (OwnerId != GameManager.Instance.CurrentPlayer)
                {
                    return;
                }
                if (currentTower != null)
                {
                    var diceValues = GameManager.Instance.GetDiceValues();
                    OnCoinClicked?.Invoke(currentTower);
                    Debug.Log($"Coin {gameObject.name} is currently on Tower: {currentTower.name}");
                }
                else
                {
                    Debug.Log($"Coin {gameObject.name} is not on any tower.");
                }
            }
            else
            {
                Debug.Log("Hit something else, not this coin.");
            }
        }
        else
        {
            Debug.Log("Raycast2D missed.");
        }
    }


    // Call this method when the coin is placed on a tower
    public void SetCurrentTower(Tower tower)
    {
        currentTower = tower;
        Debug.Log($"Coin {gameObject.name} placed on Tower: {tower.name}");
    }
    
    // Optional: to clear tower info when removed
    public void ClearTower()
    {
        Debug.Log($"Coin {gameObject.name} removed from Tower: {currentTower?.name}");
        currentTower = null;
    }
}