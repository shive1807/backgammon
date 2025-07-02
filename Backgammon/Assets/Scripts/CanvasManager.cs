using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{ 
    [SerializeField] private Button doneButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button undoButton;
    [SerializeField] private Button redoButton;

    private void Start()
    {
        doneButton.onClick.AddListener(() =>
        {
            MessageBus.Instance.Publish(new CoreGameMessage.OnDonePressed());
        });
        
        resetButton.onClick.AddListener(() =>
        {
            // Now uses CommandManager undo instead of old reset system
            CommandManager.Instance.UndoLastCommand();
        });
        
        // Add undo/redo functionality
        if (undoButton != null)
        {
            undoButton.onClick.AddListener(() =>
            {
                CommandManager.Instance.UndoLastCommand();
            });
        }
        
        if (redoButton != null)
        {
            redoButton.onClick.AddListener(() =>
            {
                CommandManager.Instance.RedoLastCommand();
            });
        }
    }
    
    private void OnEnable()
    {
        if (CommandManager.Instance != null)
        {
            CommandManager.Instance.OnCanUndoChanged += UpdateUndoButton;
            CommandManager.Instance.OnCanRedoChanged += UpdateRedoButton;
        }
    }
    
    private void OnDisable()
    {
        if (CommandManager.Instance != null)
        {
            CommandManager.Instance.OnCanUndoChanged -= UpdateUndoButton;
            CommandManager.Instance.OnCanRedoChanged -= UpdateRedoButton;
        }
    }
    
    private void UpdateUndoButton(bool canUndo)
    {
        if (undoButton != null)
            undoButton.interactable = canUndo;
    }
    
    private void UpdateRedoButton(bool canRedo)
    {
        if (redoButton != null)
            redoButton.interactable = canRedo;
    }
}
