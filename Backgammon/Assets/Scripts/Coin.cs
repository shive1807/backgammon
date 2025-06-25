using UnityEngine;
using UnityEngine.EventSystems;

public class Coin : MonoBehaviour
{
    // Reference to the tower this coin is currently placed on
    public Tower currentTower;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        Debug.Log($"Coin {gameObject.name} initialized.");
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
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("2D Ray hit: " + hit.collider.gameObject.name);

            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log($"✅ Coin {gameObject.name} clicked!");

                if (currentTower != null)
                {
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