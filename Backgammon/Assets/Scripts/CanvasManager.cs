using UnityEngine;
using System.Collections.Generic;

public class CanvasManager : MonoBehaviour
{
    public List<Dice> p1Dice = new ();
    public List<Dice> p2Dice = new ();

    private void SetPlayerDice()
    {
        DisableDice();
        EnableDice(GameManager.Instance.CurrentPlayer);
    }

    private void DisableDice()
    {
        foreach (var dice in p2Dice)
        {
            dice.gameObject.SetActive(false);
        }
        
        foreach (var dice in p1Dice)
        {
            dice.gameObject.SetActive(false);
        }
    }

    private void EnableDice(int turnIndex)
    {
        if (turnIndex == 0)
        {
            foreach (var dice in p1Dice)
            {
                dice.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var dice in p2Dice)
            {
                dice.gameObject.SetActive(true);
            }
        }
    }
}
