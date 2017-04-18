using System;
using Backtory.Core.Public;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Threads;

public class BacktoryBackend: MonoBehaviour
{
    private static BacktoryBackend _instance;

    public static BacktoryBackend instance
    {
        get { return _instance; }
        private set { _instance = value; }
    }
    private ILeaderboardAsync leaderboard;
    private IRegistrationObserver registerBooth; // !!

    public void Awake()
    {
        if (_instance == null)
        {
            instance = this;
        }
    }
    public bool InitializeBacktoryAsync(ILeaderboardAsync leaderboard)
    {
        if(this.leaderboard == null)
        {
            this.leaderboard = leaderboard;
            return true;
        }
        return false;
    }
    public bool InitializeBacktoryRegistrationObserver(IRegistrationObserver registerBooth)
    {
        if (this.registerBooth == null)
        {
            this.registerBooth = registerBooth;
            return true;
        }
        return false;
    }

    #region click listeners
    public void LoginAsGuest()
    {
        BacktoryUser.LoginAsGuestInBackground((response) =>
        {
            //log.text = response.Successful ? "succeeded" : "failed; " + response.Message;
            registerBooth.RecieveRegistrationStatus(response as IBacktoryResponse<BacktoryUser>);
        });
    }

    public void RegisterUser(string gender, string username, string email)
    {
        if (registerBooth == null)
        {
            throw new System.Exception("I Donno where to send the registration info comming from the callback");
        }
        new BacktoryUser.Builder().SetFirstName(gender).
        SetLastName("01").
        SetUsername(username).
        SetEmail(email).
        SetPassword(email).
        SetPhoneNumber("09xxxxxxxxx").
        build().RegisterInBackground
        ((response) =>
        {
            registerBooth.RecieveRegistrationStatus(response);
            //PrintCallBack<BacktoryUser>();
        });
    }

    public void Login(string username, string password)
    {
        BacktoryUser.LoginInBackground
            (username,
            password,
            response =>
            {
                registerBooth.RecieveLoginStatus(response);
            });
    }
    public void SendPlayerStats(int bitScore, int timeScore)
    {
        new BacktoryEventModel(bitScore, timeScore).SendInBackground(response => {/* response.Successful ? "succeeded" : "failed"*/ });
    }
    public void GetPlayerRank()
    {
        new TopPlayersLeaderBoard().GetPlayerRankInBackground(rankResponse =>
        {
            // Callback from server
            // Check if backtory returned result successfully
            if (rankResponse.Successful)
            {
                leaderboard.ReceiveCurrentPlayerRank(rankResponse.Body.Rank);
            }
            else
            {
                // #revision
                // do something based on error code
                leaderboard.ReceiveCurrentPlayerRank(-1);
            }
        });
    }
    public void GetPlayerAround()
    {
        //new TopPlayersLeaderBoard().GetPlayersAroundMeInBackground(5, PrintCallBack<BacktoryLeaderBoard.LeaderBoardResponse>());
        TopPlayersLeaderBoard arround = new TopPlayersLeaderBoard();
        arround.GetPlayersAroundMeInBackground(5, leaderBoardRespounse =>
        {

            // Callback from server

            // Check if backtory returned result successfully
            if (leaderBoardRespounse.Successful)
            {
                // Extract response info

                leaderboard.ReceiveAroundPlayers(leaderBoardRespounse.Body.UsersProfile);
            }
            else
            {
                // #revision
                // do something based on error code
            }
        });
    }
    public void GetTopPlayers()
    {
        TopPlayersLeaderBoard around = new TopPlayersLeaderBoard();
        around.GetTopPlayersInBackground(3, leaderBoardRespounse =>
        {
            // Callback from server
            // Check if backtory returned result successfully
            if (leaderBoardRespounse.Successful)
            {
                // Extract response info
                leaderboard.ReceiveTopPlayers(leaderBoardRespounse.Body.UsersProfile);
            }
            else
            {
                // #revision
                // do something based on error code
            }
        });
    }
  

    //public void onAroundMePlayers()
    //{
    //    new TopPlayersLeaderBoard().GetPlayersAroundMeInBackground(5, PrintCallBack<BacktoryLeaderBoard.LeaderBoardResponse>());
    //}

    #endregion


    internal Action<IBacktoryResponse<T>> PrintCallBack<T>()
    {
        return (backtoryResponse) =>
        {
            Debug.Log(JsonConvert.SerializeObject(backtoryResponse.Body, Formatting.Indented, JsonnetSetting()));
        };
    }

    private JsonSerializerSettings JsonnetSetting()
    {
        return new JsonSerializerSettings()
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };
    }
}
