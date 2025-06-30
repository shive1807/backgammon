using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int GetCurrentTurn { get; private set; }

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Subscribe<CoreGameMessage.SwitchTurn>(OnSwitchTurn);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.SwitchTurn>(OnSwitchTurn);
    }
    
    private void OnGameSetup(CoreGameMessage.GameSetup message)
    {
        GetCurrentTurn = 0;
    }

    private void OnSwitchTurn(CoreGameMessage.SwitchTurn message)
    {
        GetCurrentTurn++;
        GetCurrentTurn %= GameSettings.NumberOfPlayers;
    }
}