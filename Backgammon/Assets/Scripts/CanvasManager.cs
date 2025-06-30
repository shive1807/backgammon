using UnityEngine;
using System.Collections.Generic;

public class CanvasManager : MonoBehaviour
{
    public List<Dice> p1Dice = new ();
    public List<Dice> p2Dice = new ();

    public static CanvasManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SetPlayerDice()
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

    public List<int> ShuffleDiceAndReturnValues(int turnIndex)
    {
        if (turnIndex == 0)
        {
            return new List<int>() { p1Dice[0].Roll(), p1Dice[1].Roll() };
        }
        else
        {
            return new List<int>() { p2Dice[0].Roll(), p2Dice[1].Roll() };
        }
    }
}
