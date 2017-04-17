using UnityEngine;
using Backtory.Core.Public;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

public class Backend : MonoBehaviour
{
    private static Backend _instance;

    public static Backend instance
    {
        get { return _instance; }
        set { _instance = value; }
    }
    public string registrationResult;

    private void Awake()
    {
        if(_instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void RegisterUser(string nickname, string email)
    {
        string os = SystemInfo.operatingSystem;
        //Debug.Log(deviceModel);
        Debug.Log(SystemInfo.operatingSystem);
        new BacktoryUser.Builder().
            SetFirstName(nickname).
            SetLastName(os ).
            SetEmail(email).
            SetUsername(email).
            SetPassword("rr").
            SetPhoneNumber("091551"+UnityEngine.Random.Range(0, 1000)).
            build().RegisterInBackground(PrintCallBack<BacktoryUser>());
    }
    public void LoginUser(string userName)
    {
        BacktoryUser.LoginInBackground(userName, "rr", null);
    }
    public void SetPlayerScore()
    {
        //BitwarLeaderboard leaderboardEvent = new BitwarLeaderboard(100);
        //leaderboardEvent.SendInBackground(backtoryResponse =>
        //{
        //    if(backtoryResponse.Successful)
        //    {
        //        Debug.Log("Saved score");
        //    }
        //    else
        //    {
        //        Debug.Log(backtoryResponse.Message);
        //        Debug.Log("some error while sending the score event");
        //    }
        //});
        string str="";
        new BacktoryEventModel(100, 0).SendInBackground(response => str = response.Successful ? "succeeded" : "failed");
        Debug.LogError(str);
    }

    public void GetPlayerRank()
    {
        new TopPlayersLeaderBoard().GetPlayerRankInBackground(PrintCallBack<BacktoryLeaderBoard.LeaderBoardRank>());
    }
    private Action<IBacktoryResponse<T>> PrintCallBack<T>()
    {
        return (backtoryResponse) =>
        {
            if (backtoryResponse.Successful)
                registrationResult = JsonConvert.SerializeObject(backtoryResponse.Body, Formatting.Indented, JsonnetSetting());
            else
                registrationResult = backtoryResponse.Message;
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
