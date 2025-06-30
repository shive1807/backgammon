using System;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public Tower            currentTower;
    public Tower            sourceTower;

    private SpriteRenderer  _spriteRenderer;
    private bool            _shouldRegisterInput;
    public event Action<Tower, Tower> OnRingClicked;

    void Start()
    {
        // Get the SpriteRenderer component on this GameObject
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on Ring object.");
        }
    }

    public void SetCurrentTower(Tower currTower, Tower sTower)
    {
        currentTower = currTower;
        sourceTower  = sTower;
        
        Debug.Log($"Coin {gameObject.name} placed on Tower: {currentTower.name}");
        if (currentTower.GetOwnerPlayerId() == GameManager.Instance.CurrentPlayer)
        {
            SetGreen();
            _shouldRegisterInput = true;
        }
        else
        {
            SetRed();
            _shouldRegisterInput = false;
        }
    }

    // Set the ring color to green
    private void SetGreen()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        _spriteRenderer.color = Color.green;
        Debug.Log("Ring color set to green.");
    }

    // Set the ring color to red
    private void SetRed()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        _spriteRenderer.color = Color.red;
        Debug.Log("Ring color set to red.");
    }

    // Detect touch or mouse click
    private void OnMouseDown()
    {
        Debug.Log($"Ring {gameObject.name} was touched/clicked.");
        if (_shouldRegisterInput)
        {
            OnRingClicked?.Invoke(sourceTower, currentTower);
        }
    }
}