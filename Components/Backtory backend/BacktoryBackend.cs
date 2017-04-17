using System;
using Backtory.Core.Public;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
public class BacktoryBackend : MonoBehaviour
{
    private static BacktoryBackend _instance;

    public static BacktoryBackend instance
    {
        get { return _instance; }
        set { _instance = value; }
    }
    private ILeaderboardAsync leaderboard;
    private IRegistrationObserver registerBooth; // !!

    public Text log;



    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
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
            PrintCallBack<BacktoryUser>();
        });
    }

    public void Login(string username, string password)
    {
        BacktoryUser.LoginInBackground(username, password, response => log.text = response.Successful ? "succeeded" : "failed; " + response.Message);
    }
    public void SendPlayerStats(int bitScore, int timeScore)
    {
        string str = "";
        //Debug.Log("sending score is started");
        new BacktoryEventModel(bitScore, timeScore).SendInBackground(response => str = response.Successful ? "succeeded" : "failed");
        //Debug.Log("send score to server" + PlayerPrefs.GetInt("bestScore").ToString());
        //Debug.Log(str);

    }



    public void GetPlayerRank()
    {
        new TopPlayersLeaderBoard().GetPlayerRankInBackground(rankResponse =>
        {
            // Callback from server
            // Check if backtory returned result successfully
            if (rankResponse.Successful)
            {
                // Extract response info
                //string leaderboardPosition = "my rank: " + rankResponse.Body.Rank
                //        + "\n my scores: " + rankResponse.Body.Scores;
                //playerRank = rankResponse.Body.Rank;
                //// Log it
                //Debug.Log("leader board pos : " + leaderboardPosition);
                leaderboard.ReceiveCurrentPlayerRank(rankResponse.Body.Rank);
            }
            else
            {
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

                // do something based on error code
                leaderboard.ReceiveAroundPlayers(null);

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
                // do something based on error code
                leaderboard.ReceiveTopPlayers(null);
            }
        });
    }
  

    //public void onGetTopPlayers()
    //{
    //    new TopPlayersLeaderBoard().GetTopPlayersInBackground(5, PrintCallBack<BacktoryLeaderBoard.LeaderBoardResponse>());

    //}

    public void onAroundMePlayers()
    {
        new TopPlayersLeaderBoard().GetPlayersAroundMeInBackground(5, PrintCallBack<BacktoryLeaderBoard.LeaderBoardResponse>());
    }

    #endregion

    #region sample stuff
    internal const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private static string LastGenEmail;

    private static string LastGenUsername
    {
        get
        {
            return PlayerPrefs.GetString("last username");
        }
        set
        {
            PlayerPrefs.SetString("last username", value);
        }
    }
    private static string LastGenPassword
    {
        get
        {
            return PlayerPrefs.GetString("last password");
        }
        set
        {
            PlayerPrefs.SetString("last password", value);
        }
    }

    private static string RandomAlphabetic(int length)
    {
        var charArr = new char[length];
        //var random = new System.Random(Environment.TickCount);
        for (int i = 0; i < charArr.Length; i++)
        {
            //charArr[i] = chars[random.Next()];
            charArr[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        return new string(charArr);
    }

    internal static string GenerateEmail(bool random)
    {
        string s = random ? RandomAlphabetic(3) + "@" + RandomAlphabetic(3) + ".com" : "ar.d.farahani@gmail.com";
        LastGenEmail = s;
        return s;
    }

    internal static string GenerateUsername(bool random)
    {
        string s = random ? RandomAlphabetic(6) : "hamze";
        LastGenUsername = s;
        return s;
    }

    internal static string GeneratePassword(bool random)
    {
        string s = random ? RandomAlphabetic(6) : "1234";
        LastGenPassword = s;
        return s;
    }

    internal Action<IBacktoryResponse<T>> PrintCallBack<T>()
    {
        return (backtoryResponse) =>
        {

            if (backtoryResponse.Successful)
            {
                log.text = JsonConvert.SerializeObject(backtoryResponse.Body, Formatting.Indented, JsonnetSetting());
                Debug.Log(log);
            }
            else
            {
                log.text = backtoryResponse.Message;
            }
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
    //public class GlobalEventListener : IGlobalEventListener
    //{
    //  public Text resultText { set; get; }
    //  public void OnEvent(BacktorySDKEvent logoutEvent)
    //  {
    //    if (logoutEvent is LogoutEvent)
    //      resultText.text = "you must login again!";
    //  }
    //}
    #endregion


    //public void onGuestRegisterClick()
    //{
    //    UnityWebRequest.Get("").Send();
    //    StartCoroutine(GuestRegister());

    //}

    //IEnumerator GuestRegister()
    //{
    //    UnityWebRequest guestLoginRequest = new BacktoryUser().LoginAsGuest();
    //    guestLoginRequest.SetRequestHeader(Backtory.ContentTypestring, Backtory.ApplicationJson);
    //    guestLoginRequest.SetRequestHeader(Backtory.AuthIdstring, BacktoryConfig.BacktoryAuthInstanceId);
    //    yield return guestLoginRequest.Send();

    //    if (guestLoginRequest.isError)
    //    {
    //        switch (guestLoginRequest.responseCode)
    //        {
    //            case (int)HttpStatusCode.NotFound:
    //                //TODO: update result textview
    //                Debug.Log(guestLoginRequest.downloadHandler.text);
    //                break;
    //        }
    //    } else
    //    {
    //        Debug.Log(guestLoginRequest.downloadHandler.text);
    //    }
    //}
}