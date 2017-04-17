using UnityEngine;
using Backtory.Core.Public;
using System.Collections;
using System;

[System.Serializable]
public class BitwarPlayer
{
    public string nickname;
    public int bitScore;
    public int hardScore;
    public int rank;
    public Gender gender;

    // methods
    // constructor
    public BitwarPlayer(string nickname, Gender gender, int bitScore, int timeScore, int rank)
    {
        this.nickname = nickname;
        this.bitScore = bitScore;
        this.hardScore = timeScore;
        this.rank = rank;
        this.gender = gender;

    }
    //public static BitwarPlayer Parse(string bitwarStrRep)
    //{
    //    string[] playerDetails = new string[5];
    //    int scoreInteger, rankInteger;
    //    int startIndex = 0;
    //    // parses the string to a valid bitwarPlayer
    //    if (!bitwarStrRep.Contains("$"))
    //    {
    //        throw new System.Exception("there has been an error while parsing the player");
    //    }
    //    for (int i = 0; i < 5; i++)
    //    {
    //        int dollarIndex = bitwarStrRep.IndexOf('$');
    //        playerDetails[i] = bitwarStrRep.Substring(startIndex, dollarIndex);
    //        bitwarStrRep = bitwarStrRep.Remove(startIndex, dollarIndex + 1);
    //    }
    //    if (!int.TryParse(playerDetails[3], out scoreInteger))
    //    {
    //        throw new System.Exception("Error while parsing player score! what the heck are u guys doin???");
    //    }
    //    if(!int.TryParse(playerDetails[4], out rankInteger))
    //    {
    //        throw new System.Exception("Error while parsing player score! what the heck are u guys doin???");
    //    }
    //    return new BitwarPlayer(playerDetails[0], // nickname
    //                            playerDetails[1], //email
    //                            playerDetails[2], // ID
    //                            scoreInteger, // score
    //                            rankInteger); //rank
    //}
    public static BitwarPlayer Parse(BacktoryLeaderBoard.UserProfile backtoryUser, int userRank)
    {
        Gender gend;
        try
        {
            gend = (Gender) Enum.Parse(typeof(Gender), backtoryUser.UserBriefProfile.FirstName, true);
        }
        catch (Exception)
        {
            gend = (UnityEngine.Random.Range(0, 1) == 0) ? Gender.male : Gender.female;
        }
        return new BitwarPlayer(
                backtoryUser.UserBriefProfile.UserName,
                gend,
                backtoryUser.Scores[0],
                backtoryUser.Scores[1],
                userRank);
    }
}
