using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using Backtory.Core.Public;
using UnityEngine.Events;

public enum LeaderboardDisplayTypes
{
    SHOW_WHO_FOLLOWS,
    SHOW_TOP3_DISTANCE,
    SHOW_TOP10_DISTANCE,
    SHOW_TOP50_DISTANCE,
    SHOW_TOP100_DISTANCE
}
public class Leaderboard : MonoBehaviour, ILeaderboardAsync
{
    private List<BitwarPlayer> Top3, AroundMe;
    private int currentPlayerRank;
    // singleton
    private static Leaderboard _instance;

    public static Leaderboard instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    [Header("UI refs")]
    [Space(5f)]
    public Transform Top3Panel;
    public Transform FollowersPanel;
    public Transform aroundPlayersPanel;
    public Text WaitForItPlease/*playerDifferenceWthTopPlayersTxt, top10FollowersTxt, top50FollowersTxt, top100FollowersTxt*/;
    public Image higherRankIcon, top10Icon, top50Icon, top100Icon;
    public LeaderboardPlayerManifest GoldPlayer, SilverPlayer, BronzePlayer;
    public LeaderboardPlayerManifest[] aroundPlayersPrefabs;
    // methods
    void Awake()
    {
        // validating the singleton: not fancy lock or nothing
        if (_instance == null)
        {
            _instance = this;
        }
        Top3 = new List<BitwarPlayer>();
        AroundMe = new List<BitwarPlayer>();
    }
    private void Start()
    {
        BacktoryBackend.instance.InitializeBacktoryAsync(this as ILeaderboardAsync);
    }

    private void OnEnable()
    {
        BacktoryBackend.instance.GetPlayerRank();
        //UIController.instance.FadeIn(LeaderboardLoadingPage);
    }

    private void OnDisable()
    {
        Top3 = new List<BitwarPlayer>();
        AroundMe = new List<BitwarPlayer>();
    }

    public void SetLeaderboard(int currentRank, List<BitwarPlayer> aroundPlayers, List<BitwarPlayer> top3)
    {
        for (int i = 0; i < aroundPlayersPrefabs.Length; i++)
        {
            aroundPlayersPrefabs[i].gameObject.SetActive(false);
        }
        if (aroundPlayers == null || top3 == null)
        {
            throw new System.Exception("you did not pass ant argument as aroundPlayers or top3; I need that stuff, man!");
        }
        if (currentRank == 1 || currentRank == 2 || currentRank == 3)
        {
            DisplayTop3(top3, currentRank);
            return;
        }
        DisplayTop3(top3);
        DisplayNormalPlayer(currentRank, aroundPlayers);
    }

    private void DisplayNormalPlayer(int currentPlayerRank, List<BitwarPlayer> aroundPlayers)
    {
        aroundPlayersPanel.gameObject.SetActive(true);
        FollowersPanel.gameObject.SetActive(false);
        //aroundPlayers.Add(currentPlayer);
        //aroundPlayers.Sort((a, b) => a.rank.CompareTo(b.rank));
        IdentifyAsCurrentPlayer(aroundPlayersPrefabs[currentPlayerRank - aroundPlayers[0].rank]);
        for (int i = 0; i < aroundPlayers.Count; i++)
        {
            aroundPlayersPrefabs[i].SetPlayerStats(aroundPlayers[i], inTop3: false);
        }
    }

    private void DisplayTop3(List<BitwarPlayer> tops, int currentPlayerRank = -1)
    {
        tops.Sort((x, y) => x.rank.CompareTo(y.rank));
        GoldPlayer.SetPlayerStats(tops[0], inTop3: true);
        SilverPlayer.SetPlayerStats(tops[1], inTop3: true);
        BronzePlayer.SetPlayerStats(tops[2], inTop3: true);

        if (currentPlayerRank == -1)
        {
            return;
        }
        else
        {
            aroundPlayersPanel.gameObject.SetActive(false);
            FollowersPanel.gameObject.SetActive(true);
            if (currentPlayerRank == 1)
            {
                IdentifyAsCurrentPlayer(GoldPlayer);
            }
            else if (currentPlayerRank == 2)
            {
                IdentifyAsCurrentPlayer(SilverPlayer);
            }
            else if (currentPlayerRank == 3)
            {
                IdentifyAsCurrentPlayer(BronzePlayer);
            }
            else
            {
                throw new System.Exception("Unhandled player rank : " + currentPlayerRank + ". How the hell this led his way into top3 players!!????");
            }
            //top100FollowersTxt.text = (currentPlayerScore - averageTop100).ToString();
            //top50FollowersTxt.text = (currentPlayerScore - averageTop50).ToString();
            //top10FollowersTxt.text = (currentPlayerScore - averageTop10).ToString();
        }
    }
    private void IdentifyAsCurrentPlayer(LeaderboardPlayerManifest player)
    {
        player.thisIsMeIndicator.SetActive(true);
    }

    public void ReceiveCurrentPlayerRank(int rank)
    {
        if(rank != -1)
        {
            currentPlayerRank = rank;
            StartCoroutine(GetOnWithLeaderboard(rank));
        }
        else
        {
            throw new System.Exception("Could not retrieve players rank; try again later!!!");
        }
    }
    private IEnumerator GetOnWithLeaderboard(int currentRank)
    {
        if (currentRank > 3)
        {
            BacktoryBackend.instance.GetPlayerAround();
        }
        BacktoryBackend.instance.GetTopPlayers();
        while (Top3.Count == 0)
        {
            yield return new WaitForSeconds(1f);
        }
        //UIController.instance.FadeOut(LeaderboardLoadingPage);
        SetLeaderboard(currentRank, AroundMe, Top3);
    }
    public void ReceiveAroundPlayers(List<BacktoryLeaderBoard.UserProfile> around)
    {
        for (int i = 0; i < around.Count; i++)
        {
            AroundMe.Add(BitwarPlayer.Parse(around[i], currentPlayerRank - 2 + i)); 
             // currentPlayerRank - 2 + i : just in this case that we know we just want the 5 around, including the player himself
        }
    }

    public void ReceiveTopPlayers(List<BacktoryLeaderBoard.UserProfile> topPlayersInList)
    {
        for (int i = 0; i < topPlayersInList.Count; i++)
        {
            Top3.Add(BitwarPlayer.Parse(topPlayersInList[i], i + 1)); 
        }
    }
}
