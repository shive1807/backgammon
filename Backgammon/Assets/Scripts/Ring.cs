using UnityEngine;

public class Ring : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Tower currentTower;

    void Start()
    {
        // Get the SpriteRenderer component on this GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on Ring object.");
        }
    }

    public void SetCurrentTower(Tower tower)
    {
        currentTower = tower;
        Debug.Log($"Coin {gameObject.name} placed on Tower: {tower.name}");
        if (currentTower.GetOwnerPlayerId() == GameManager.Instance.CurrentPlayer)
        {
            SetGreen();
        }
        else
        {
            SetRed();
        }
    }

    // Set the ring color to green
    private void SetGreen()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.color = Color.green;
        Debug.Log("Ring color set to green.");
    }

    // Set the ring color to red
    private void SetRed()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.color = Color.red;
        Debug.Log("Ring color set to red.");
    }

    // Detect touch or mouse click
    private void OnMouseDown()
    {
        Debug.Log($"Ring {gameObject.name} was touched/clicked.");
    }
}