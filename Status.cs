using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour
{
    public static int countOfLosts, countOfZerosAndOnes, sumUpScore,highestScore;
    public static string username;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void IncreaseLosts()
    {
        countOfLosts++;
        
    }
    public static void IncreaseZerosAndOnes()
    {
        countOfZerosAndOnes++;
    }
    public static void IncreaseSumUpScore(int newScore)
    {
        sumUpScore += newScore;
    }
    public static void SetUsername(string newName)
    {
        username = newName;
    }
    public static void SetHighestScore(int newHighScore)
    {
        highestScore = newHighScore;
    }
    public static void SaveScores()
    {
        if (PlayerPrefs.HasKey("countOfLosts"))
        {
            int temp = 0;
            temp=PlayerPrefs.GetInt("countOfLosts")+countOfLosts;
            PlayerPrefs.SetInt("countOfLosts", countOfLosts);

        }
        if (!PlayerPrefs.HasKey("sumUpScore"))
        {
            int temp = 0;
            temp = PlayerPrefs.GetInt("sumUpScore") + sumUpScore;
            PlayerPrefs.SetInt("sumUpScore", sumUpScore);

        }
        if (!PlayerPrefs.HasKey("username"))
        {
            string temp = " ";
            temp = PlayerPrefs.GetString("username") + username;
            PlayerPrefs.SetString("username", username);

        }
        if (!PlayerPrefs.HasKey("highestScore"))
        {
            int temp = 0;
            temp = PlayerPrefs.GetInt("highestScore") + highestScore;
            PlayerPrefs.SetInt("highestScore", highestScore);

        }


    }

}
