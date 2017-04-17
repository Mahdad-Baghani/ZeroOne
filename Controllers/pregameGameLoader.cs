using UnityEngine; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class pregameGameLoader : MonoBehaviour
{
    private AsyncOperation loadMainGame;
    [SerializeField] private int mainMenuIndex;

    [SerializeField] Image splashRenderer, gameLogoRenderer, waitForIt;
    [SerializeField] private float splashScreenDuration, gameLogoDuration;


    // methods
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private IEnumerator Start()
    {
        yield return null;
        //yield return new WaitForSeconds(.5f); // wait a little bit before jumping into shit and get yourself dirty for no good shit
        //yield return StartCoroutine(FadeInGameUI(splashRenderer, splashScreenDuration));
        //yield return StartCoroutine(FadeOutGameUI(splashRenderer, splashScreenDuration));
        //yield return StartCoroutine(FadeInGameUI(gameLogoRenderer, gameLogoDuration));
        //yield return StartCoroutine(FadeOutGameUI(gameLogoRenderer, gameLogoDuration));
        //loadMainGame = SceneManager.LoadSceneAsync(mainMenuIndex);
        //yield return StartCoroutine(FadeInGameUI(waitForIt, 3f));
        //while(!loadMainGame.isDone)
        //{
        //    Debug.Log("not done with loading");
        //    yield return new WaitForEndOfFrame();
        //}
        //StartCoroutine(FadeOutGameUI(waitForIt, 3f));

    }

    IEnumerator FadeInGameUI(Image renderer, float duration)
    {
        renderer.GetComponent<CanvasRenderer>().SetAlpha(0);
        renderer.gameObject.SetActive(true); // I do not want to risk enabling the Image with alpha = 1 in one frame and 
                                                //  suddenly jump to the next frame with an alpha = 0 
        renderer.CrossFadeAlpha(1, duration, true);
        yield return new WaitForSeconds(duration);

    }
    IEnumerator FadeOutGameUI(Image renderer, float duration)
    {
        renderer.GetComponent<CanvasRenderer>().SetAlpha(1);
        renderer.CrossFadeAlpha(0, duration, true);
        yield return new WaitForSeconds(duration);
        renderer.gameObject.SetActive(false);
    }
}
