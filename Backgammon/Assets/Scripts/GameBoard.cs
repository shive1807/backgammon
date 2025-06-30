using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    public List<Tower> towers = new (); // 24 towers on the board

    private int _availableActions;

    private int _currentTurn;
    
    private List<int> _diceValues;

    private List<int> _diceValuesUsed;
    
    [SerializeField]
    private Tower whiteSpawnPoint;
    
    [SerializeField]
    private Tower blackSpawnPoint;
    
    private void OnGameSetup(CoreGameMessage.GameSetup message)
    {
        _diceValuesUsed   = new List<int>();
        _diceValues       = new List<int>();
        _availableActions = 0;
        //initialize tower index.
        var i = 0;
        foreach (var tower in towers)
        {
            tower.Initialize(i);
            i++;
        }
        
        AddWhiteCoins();
        AddBlackCoins();
    }

    private void OnEnable()
    {
        MessageBus.Instance.Subscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Subscribe<CoreGameMessage.TurnDiceSetupAndRoll>(TurnDiceSetupAndRoll);
        MessageBus.Instance.Subscribe<CoreGameMessage.DiceRolled>(OnDiceShuffled);
        MessageBus.Instance.Subscribe<CoreGameMessage.CoinClicked>(OnCoinClicked);
        MessageBus.Instance.Subscribe<CoreGameMessage.RingClicked>(OnRingClicked);
        MessageBus.Instance.Subscribe<CoreGameMessage.OnCoinMoved>(OnCheckerMoved);
        MessageBus.Instance.Subscribe<CoreGameMessage.OnResetPressed>(OnResetPressed);
    }

    private void OnDisable()
    {
        MessageBus.Instance.Unsubscribe<CoreGameMessage.GameSetup>(OnGameSetup);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.TurnDiceSetupAndRoll>(TurnDiceSetupAndRoll);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.DiceRolled>(OnDiceShuffled);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.CoinClicked>(OnCoinClicked);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.RingClicked>(OnRingClicked);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.OnCoinMoved>(OnCheckerMoved);
        MessageBus.Instance.Unsubscribe<CoreGameMessage.OnResetPressed>(OnResetPressed);
    }

    private void AddWhiteCoins()
    {
        AddCoinsToTower(GameSettings.TowerIndex_TopRight,    GameSettings.Coins_TopRight,    GameSettings.Player0);
        AddCoinsToTower(GameSettings.TowerIndex_MiddleLeft,  GameSettings.Coins_MiddleLeft,  GameSettings.Player0);
        AddCoinsToTower(GameSettings.TowerIndex_Center,      GameSettings.Coins_Center,      GameSettings.Player0);
        AddCoinsToTower(GameSettings.TowerIndex_BottomLeft,  GameSettings.Coins_BottomLeft,  GameSettings.Player0);
    }

    private void AddBlackCoins()
    {
        AddCoinsToTower(GameSettings.TowerIndex_TopLeft,      GameSettings.Coins_TopLeft,      GameSettings.Player1);  // Tower 1 (index 0)
        AddCoinsToTower(GameSettings.TowerIndex_MiddleRight,  GameSettings.Coins_MiddleRight,  GameSettings.Player1); // Tower 12 (index 11)
        AddCoinsToTower(GameSettings.TowerIndex_CenterRight,  GameSettings.Coins_CenterRight,  GameSettings.Player1); // Tower 17 (index 16)
        AddCoinsToTower(GameSettings.TowerIndex_BottomRight,  GameSettings.Coins_BottomRight,  GameSettings.Player1); // Tower 19 (index 18)
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
            Debug.Log($"Adding coins to tower {towerIndex} with player {playerId}");
            towers[towerIndex].AddChecker(playerId);
        }
    }

    private void OnDiceShuffled(CoreGameMessage.DiceRolled message)
    {
        _diceValues = message.Dice;
    }

    private static IEnumerator DelaySwitchTurn()
    {
        yield return new WaitForSeconds(1f);
        MessageBus.Instance.Publish(new CoreGameMessage.SwitchTurn());
    }

    

    private int GetAvailableActions(List<int> diceValues)
    {
        _availableActions = 0;
        
        foreach (var tower in towers)
        {
            if (tower.IsOwnedBy(_currentTurn))
            {
                foreach (var diceValue in diceValues)
                {
                    var targetValue = tower.TowerIndex;

                    if (_currentTurn == 0)
                    {
                        targetValue -= diceValue;
                    }
                    else
                    {
                        targetValue += diceValue;
                    }
                    if (targetValue > towers.Count || targetValue < 0)
                    {
                        continue;
                    }
                    
                    tower.HighlightTopCoin();
                    _availableActions++;
                }
            }
        }
        return _availableActions;
    }

    //update current player turn.
    //turn started and dice is rolled.
    private void TurnDiceSetupAndRoll(CoreGameMessage.TurnDiceSetupAndRoll message)
    {
        _currentTurn = message.PlayerIndex;
    }

    //listens to event raised by coin.
    private void OnCoinClicked(CoreGameMessage.CoinClicked message)
    {
        //MessageBus.Instance.Publish(new CoreGameMessage.CleanTowerRings());
        
        var diceValues = _diceValues;
        var runOnce       = diceValues.Distinct().Count() != diceValues.Count();

        foreach (var diceValue in diceValues)
        {
            var towerIndex       = message.TowerIndex;
            var targetTowerIndex = towerIndex;

            //for player 0 subtract and for player 1 add the dice values.
            targetTowerIndex += GameManager.Instance.GetTurnManager().GetCurrentTurn == 0 ? -diceValue : diceValue;
            
            if (targetTowerIndex > towers.Count || targetTowerIndex < 0)
            {
                continue;
            }
            
            towers[targetTowerIndex].AddRing(message.OwnerId, towerIndex, targetTowerIndex);

            if (runOnce) return;
        }
    }
    
    private void OnCheckerMoved(CoreGameMessage.OnCoinMoved message)
    {
        _diceValues.Remove(message.CheckerMovedByDiceValue);
        _diceValuesUsed.Add(message.CheckerMovedByDiceValue);
    }


    //remove from the current tower and move the coin to the new target tower.
    private void OnRingClicked(CoreGameMessage.RingClicked message)
    {
        var topChecker = towers[message.SourceTowerIndex].RemoveTopChecker();
        towers[message.CurrentTowerIndex].AddCoin(topChecker);
        MessageBus.Instance.Publish(new CoreGameMessage.CleanTowerRings());
        MessageBus.Instance.Publish(new CoreGameMessage.OnCoinMoved(Mathf.Abs(message.SourceTowerIndex - message.CurrentTowerIndex)));
    }
    
    //remove from the current tower and move the coin to the previous tower.
    private void OnResetPressed(CoreGameMessage.OnResetPressed message)
    {
        //Get the used values and put it to available dice values.
        _diceValues.AddRange(_diceValuesUsed);
        _diceValuesUsed.Clear();
        
        //Call each tower to put back the newly moved coin if they have any.
        foreach (var tower in towers)
        {
            var coinsToMove = tower.GetListOfMovedCoins();
            for (var i = 0; i < coinsToMove; i++)
            {
                var coin = tower.RemoveTopChecker();
                towers[coin.GetPrevTower()].ResetCoin(coin);
            }
        }
    }
}