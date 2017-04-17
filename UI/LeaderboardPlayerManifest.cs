using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaderboardPlayerManifest : MonoBehaviour
{
    public Text nicknameTxt, bitScoreTxt, timeScoreTxt, rankTxt;
    public Color color1;
    public Color color2;
    private static int colorCounter = -1;
    public Image backgroundImage, MaleGender, FemaleGender;
    public GameObject thisIsMeIndicator;


    // methods
   
    public void SetPlayerStats(BitwarPlayer player, bool inTop3)
    {
        this.gameObject.SetActive(true);

        nicknameTxt.text = player.nickname.faConvert();
        // #revision : add persian compatibility
        bitScoreTxt.text = player.bitScore.ToString();
        timeScoreTxt.text = player.hardScore.ToString();
        if (player.gender == Gender.male)
        {
            MaleGender.enabled = true;
            FemaleGender.enabled = false;
        }
        else if (player.gender == Gender.female)
        {
            MaleGender.enabled = false;
            FemaleGender.enabled = true;
        }
        if(inTop3)
        {
            rankTxt.enabled = false;
        }
        else
        {
            rankTxt.text = player.rank.ToString(); 
            if (colorCounter == -1)
            {
                backgroundImage.color = color1;
                colorCounter = 1;
            }
            else
            {
                backgroundImage.color = color2;
                colorCounter = -1;
            }

        }
    }
}
