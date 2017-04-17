using Backtory.Core.Public;


  public class BacktoryEventModel : BacktoryGameEvent
    {

        [EventName]
        public static string eventName = "BestScore";

        [FieldName("ZeroOne_BestScoresAll")]
        public int CoinValue { set; get; }

        [FieldName("Time")]
        public int TimeValue { set; get; }

        public BacktoryEventModel(int coinValue, int timeValue)
        {
            CoinValue = coinValue;
            TimeValue = timeValue;
        }
    }

    public class TopPlayersLeaderBoard : BacktoryLeaderBoard
    {
        [LeaderboardId]
        public static string id = "584ff9a6e4b048d0cf3e8fc5";
    }

