using UnityEngine;
using System.Collections;

public class scoreManager : MonoBehaviour {

    public static int[] highscores;
    public static int user_count;
    public static string[] highscores_names;
    public GameObject menu;
	void Start () {

    //if (Application.loadedLevel == 0)
    //    {
    //    menu.SetActive(true); 
    //    }
	}
	
    //void Update () {
	
    //}
    public void refreshTopTenScores()
        {
        string highscore_score, highscore_name;
        for (int i = 1; i <= 10; i++)
            {
            highscore_score = "highscore" + i.ToString();
            highscore_name = "highscore_name" + i.ToString();
            highscores[i] = PlayerPrefs.GetInt(highscore_score);
            highscores_names[i] = PlayerPrefs.GetString(highscore_name);
            }
        }
    public void setScore(string name, int score, int rank)
        {
        PlayerPrefs.SetInt("highscore" + rank.ToString(), score);
        PlayerPrefs.SetString("highscore_name" + rank.ToString(),name);
        }
    public string getRankName(int rank)
        {
        return PlayerPrefs.GetString("highscore" + rank.ToString());
        }
    public int getRankOfScore(int score)
        {
        for (int i = 1; i <= user_count; i++)//user_count should be unlimted as much as server allows
            {
            if (score > PlayerPrefs.GetInt("highscore" + i.ToString()))
                {
                return i-1;
                }
            }
        return user_count + 1;
        }
}
