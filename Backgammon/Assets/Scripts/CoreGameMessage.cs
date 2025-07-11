using System.Collections.Generic;
using Interface;

public class CoreGameMessage
{
    public class CoinClicked : IMessage
    {
        public readonly int OwnerId;
        public readonly int TowerIndex;

        public CoinClicked(int ownerId, int towerIndex)
        {
            OwnerId    = ownerId;
            TowerIndex = towerIndex;
        }
    }

    // RingClicked removed - handled by Command Pattern

    public class OnCoinMoved : IMessage
    {
        public readonly int CheckerMovedByDiceValue;

        public OnCoinMoved(int checkerMovedByDiceValue)
        {
            CheckerMovedByDiceValue = checkerMovedByDiceValue;
        }
    }

    // PlayTurn removed - not used in current implementation
    
    public class CleanTowerRings : IMessage
    {
        public CleanTowerRings()
        {
            
        }
    }

    public class GameSetup : IMessage
    {
        
    }

    public class TurnDiceSetupAndRoll : IMessage
    {
        public readonly int PlayerIndex;

        public TurnDiceSetupAndRoll(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }

    public class DiceRolled : IMessage
    {
        public readonly List<int> Dice;
        public readonly int CurrentPlayerIndex;
        public DiceRolled(List<int> diceVales, int currentPlayerIndex)
        {
            Dice = diceVales;
            CurrentPlayerIndex = currentPlayerIndex;
        }
    }

    public class SwitchTurn : IMessage
    {
        public SwitchTurn()
        {
            
        }
    }

    public class TurnOver : IMessage
    {
        
    }

    public class OnDonePressed : IMessage
    {
        
    }
    
    public class DiceValueRestored : IMessage
    {
        public readonly int RestoredDiceValue;
        public readonly int CurrentPlayerIndex;
        
        public DiceValueRestored(int restoredDiceValue, int currentPlayerIndex)
        {
            RestoredDiceValue = restoredDiceValue;
            CurrentPlayerIndex = currentPlayerIndex;
        }
    }

    // OnResetPressed removed - replaced by CommandManager undo functionality
}