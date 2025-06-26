using System;
using UnityEngine;
using System.Collections.Generic;

using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public List<Tower> towers = new List<Tower>(); // 24 towers on the board

    public Transform whiteTowerStack; // For storing unused white coins
    public Transform blackTowerStack; // For storing unused black coins

    public GameObject coinPrefab; // Prefab for checker coins
    public GameObject ringPrefab;
    private List<GameObject> _rings = new List<GameObject>();

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
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
        for (int i = 0; i < count; i++)
        {
            var coin = Instantiate(coinPrefab);
            coin.GetComponent<Coin>().OnCoinClicked += OnCoinClicked;
            
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
        foreach (var rings in _rings)
        {
            Destroy(rings);
        }
        _rings.Clear();
        
        var diceValues = GameManager.Instance.GetDiceValues();
        foreach (var diceValue in diceValues)
        {
            var ring = Instantiate(ringPrefab);

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
            
            towers[targetTowerIndex].AddRing(ring, 0);
            _rings.Add(ring);
        }
    }
}
