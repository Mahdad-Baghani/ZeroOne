using Backtory.Core.Public;
using System.Collections.Generic;
public interface ILeaderboardAsync
{
    void ReceiveAroundPlayers(List<BacktoryLeaderBoard.UserProfile> aroundMe);
    void ReceiveTopPlayers(List<BacktoryLeaderBoard.UserProfile> topPlayersInList);
    void ReceiveCurrentPlayerRank(int rank);
}