using UnityEngine;

public class Ring : MonoBehaviour
{
    private int             _sourceTowerIndex;
    private int             _currentTowerIndex;
    private bool            _shouldRegisterInput;
    private SpriteRenderer  _spriteRenderer;

    private void Start()
    {
        // Get the SpriteRenderer component on this GameObject
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on Ring object.");
        }
    }

    public void SetCurrentTower(int sTower, int currTower, bool isTowerAvailable)
    {
        _currentTowerIndex = currTower;
        _sourceTowerIndex  = sTower;
        
        Debug.Log($"Coin {gameObject.name} placed on Tower: {_currentTowerIndex}");
        PresentRing(isTowerAvailable);
        _shouldRegisterInput = isTowerAvailable;
    }

    private void PresentRing(bool isTowerAvailable)
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        _spriteRenderer.color = isTowerAvailable ? Color.green : Color.red;
    }


    // Detect touch or mouse click
    private void OnMouseDown()
    {
        Debug.Log($"Ring {gameObject.name} was touched/clicked.");
        if (_shouldRegisterInput)
        {
            // Create and execute move command instead of publishing message directly
            var currentPlayer = GameServices.Instance.TurnManager.GetCurrentTurn;
            var diceValue = Mathf.Abs(_sourceTowerIndex - _currentTowerIndex);
            
            var moveCommand = new MoveCoinCommand(_sourceTowerIndex, _currentTowerIndex, currentPlayer, diceValue);
            CommandManager.Instance.ExecuteCommand(moveCommand);
        }
    }
}