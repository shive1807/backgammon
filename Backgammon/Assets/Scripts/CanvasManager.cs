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
            MessageBus.Instance.Publish(new CoreGameMessage.OnResetPressed());
        });
    }
}
