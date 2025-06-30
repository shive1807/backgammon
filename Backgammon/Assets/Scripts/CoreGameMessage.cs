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

    public class RingClicked : IMessage
    {
        public readonly int SourceTowerIndex;
        public readonly int CurrentTowerIndex;

        public RingClicked(int sourceTowerIndex, int currentTowerIndex)
        {
            SourceTowerIndex  = sourceTowerIndex;
            CurrentTowerIndex  = currentTowerIndex;
        }
    }

    public class OnCheckerMoved : IMessage
    {
        public readonly int CheckerMovedByDiceValue;

        public OnCheckerMoved(int checkerMovedByDiceValue)
        {
            CheckerMovedByDiceValue = checkerMovedByDiceValue;
        }
    }

    public class PlayTurn : IMessage
    {
        public readonly int PlayerId;
        public List<int> AvailableMoves;

        public PlayTurn(int playerId, List<int> availableMoves)
        {
            PlayerId = playerId;
            AvailableMoves = availableMoves;
        }
    }
    
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

    public class DiceShuffled : IMessage
    {
        public readonly List<int> Dice;
        public readonly int CurrentPlayerIndex;
        public DiceShuffled(List<int> diceVales, int currentPlayerIndex)
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

    public class OnDonePressed : IMessage
    {
        
    }

    public class OnResetPressed : IMessage
    {
        
    }
}