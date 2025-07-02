using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{ 
    [SerializeField] private Button doneButton;
    [SerializeField] private Button resetButton;

    private void Start()
    {
        doneButton.onClick.AddListener(() =>
        {
            MessageBus.Instance.Publish(new CoreGameMessage.OnDonePressed());
        });
        
        resetButton.onClick.AddListener(() =>
        {
            // Reset all moves made in the current turn
            bool success = CommandManager.Instance.UndoCurrentTurn();
            if (!success)
            {
                Debug.Log("No moves to reset in current turn");
            }
        });
        
        // Set initial button states
        UpdateResetButton();
    }
    
    private void OnEnable()
    {
        if (CommandManager.Instance != null)
        {
            CommandManager.Instance.OnCommandExecuted += OnCommandExecuted;
            CommandManager.Instance.OnCommandUndone += OnCommandUndone;
            CommandManager.Instance.OnTurnStarted += OnTurnStarted;
        }
    }
    
    private void OnDisable()
    {
        if (CommandManager.Instance != null)
        {
            CommandManager.Instance.OnCommandExecuted -= OnCommandExecuted;
            CommandManager.Instance.OnCommandUndone -= OnCommandUndone;
            CommandManager.Instance.OnTurnStarted -= OnTurnStarted;
        }
    }
    
    private void OnCommandExecuted(Commands.ICommand command)
    {
        UpdateResetButton();
    }
    
    private void OnCommandUndone(Commands.ICommand command)
    {
        UpdateResetButton();
    }
    
    private void OnTurnStarted()
    {
        UpdateResetButton();
    }
    
    private void UpdateResetButton()
    {
        if (resetButton != null && CommandManager.Instance != null)
        {
            resetButton.interactable = CommandManager.Instance.CanResetCurrentTurn();
        }
    }
}
