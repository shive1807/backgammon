using Interface;

public class CoreGameMessage
{
    public class CoinClicked : IMessage
    {
        public readonly int Coin;

        public CoinClicked(int coin)
        {
            Coin = coin;
        }
    }
}