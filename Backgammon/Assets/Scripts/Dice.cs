using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    // Assign these in Inspector: 7 dot images representing 1 to 6 dot positions
    public Image[] dots; // 0 to 6 (total 7 dots)

    // Dice roll
    public int Roll()
    {
        int number = Random.Range(1, 7); // 1 to 6
        ShowDots(number);
        return number;
    }

    // Show corresponding dots based on number
    void ShowDots(int number)
    {
        // Reset all dots
        foreach (var dot in dots)
        {
            dot.enabled = false;
        }

        switch (number)
        {
            case 1:
                dots[3].enabled = true; // center
                break;
            case 2:
                dots[0].enabled = true; // top-left
                dots[6].enabled = true; // bottom-right
                break;
            case 3:
                dots[0].enabled = true;
                dots[3].enabled = true;
                dots[6].enabled = true;
                break;
            case 4:
                dots[0].enabled = true;
                dots[2].enabled = true; // top-right
                dots[4].enabled = true; // bottom-left
                dots[6].enabled = true;
                break;
            case 5:
                dots[0].enabled = true;
                dots[2].enabled = true;
                dots[3].enabled = true;
                dots[4].enabled = true;
                dots[6].enabled = true;
                break;
            case 6:
                dots[0].enabled = true;
                dots[1].enabled = true; // mid-left
                dots[2].enabled = true;
                dots[4].enabled = true;
                dots[5].enabled = true; // mid-right
                dots[6].enabled = true;
                break;
        }
    }
}