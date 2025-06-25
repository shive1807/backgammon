using UnityEngine;

public class Ring : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the SpriteRenderer component on this GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on Ring object.");
        }
    }

    // Set the ring color to green
    public void SetGreen()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;
            Debug.Log("Ring color set to green.");
        }
    }

    // Set the ring color to red
    public void SetRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            Debug.Log("Ring color set to red.");
        }
    }
}