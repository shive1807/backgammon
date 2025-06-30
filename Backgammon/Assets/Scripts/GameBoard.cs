using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GameBoard : MonoBehaviour
{
    public List<Tower> towers = new (); // 24 towers on the board
    public GameObject coinPrefab; // Prefab for checker coins
    public GameObject ringPrefab;
    private List<GameObject> _rings = new ();
    private Stack<Coin> currentTurnCoins = new();

    private void Start()
    {
        //initialize tower index.
        var i = 0;
        foreach (var tower in towers)
        {
            tower.Initialize(i);
            i++;
        }
        
        // Optionally clear board before adding coins
        foreach (var tower in towers)
            tower.ClearTower();

        AddWhiteCoins();
        AddBlackCoins();
    }
    
    private void AddWhiteCoins()
    {
        AddCoinsToTower(23, 2, 0); // Tower 24 (index 23)
        AddCoinsToTower(12, 5, 0); // Tower 13 (index 12)
        AddCoinsToTower(7, 3, 0);  // Tower 8 (index 7)
        AddCoinsToTower(5, 5, 0);  // Tower 6 (index 5)
    }

    private void AddBlackCoins()
    {
        AddCoinsToTower(0, 2, 1);  // Tower 1 (index 0)
        AddCoinsToTower(11, 5, 1); // Tower 12 (index 11)
        AddCoinsToTower(16, 3, 1); // Tower 17 (index 16)
        AddCoinsToTower(18, 5, 1); // Tower 19 (index 18)
    }

    /// <summary>
    /// Adds coins to a specific tower and sets their color.
    /// </summary>
    /// <param name="towerIndex">Index of the tower to place checkers on (0–23).</param>
    /// <param name="count">Number of checkers to add.</param>
    /// <param name="playerId">0 = White, 1 = Black</param>
    private void AddCoinsToTower(int towerIndex, int count, int playerId)
    {
        for (var i = 0; i < count; i++)
        {
            var coin = Instantiate(coinPrefab);
            //coin.GetComponent<Coin>().OnCoinClicked += OnCoinClicked;
            
            if (playerId == 0)
            {
                coin.name = $"WhiteChecker_{towerIndex}_{i}";
                coin.GetComponent<Renderer>().material.color = Color.white;
            }
            else
            {
                coin.name = $"BlackChecker_{towerIndex}_{i}";
                coin.GetComponent<Renderer>().material.color = Color.gray;
            }

            towers[towerIndex].AddChecker(coin, playerId);
        }
    }
    

    private void OnCoinClicked(Tower tower)
    {
        ClearRings();
        
        var diceValues = GameManager.Instance.GetDiceValues();
        var runOnce       = diceValues.Distinct().Count() != diceValues.Count();

        foreach (var diceValue in diceValues)
        {
            var ring = Instantiate(ringPrefab);
            ring.GetComponent<Ring>().OnRingClicked += MoveChecker;
            var towerIndex = tower.TowerIndex;
            var targetTowerIndex = towerIndex;
            if (GameManager.Instance.CurrentPlayer == 0)
            {
                targetTowerIndex -= diceValue;
            }
            else
            {
                targetTowerIndex += diceValue;
            }
            
            if (targetTowerIndex > towers.Count || targetTowerIndex < 0)
            {
                Destroy(ring);
                continue;
            }
            
            towers[targetTowerIndex].AddRing(ring, 0, tower);
            _rings.Add(ring);

            if (runOnce) return;
        }
    }

    private void ClearRings()
    {
        foreach (var rings in _rings)
        {
            Destroy(rings);
        }
        _rings.Clear();
    }

    //remove from the current tower and move the coin to the new target tower.
    private void MoveChecker(Tower sourceTowerIndex, Tower targetTowerIndex)
    {
        var topChecker = sourceTowerIndex.RemoveTopChecker();
        //topChecker.GetComponent<Coin>().MoveToTower(targetTowerIndex);
        ClearRings();
        GameManager.Instance.RemovePlayedValuesFromDice(Mathf.Abs(sourceTowerIndex.TowerIndex - targetTowerIndex.TowerIndex));
    }
}
