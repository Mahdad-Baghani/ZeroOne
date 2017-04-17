using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Settings : MonoBehaviour
{
    static bool isBusy = false;
    public static bool isMute;
    public Image[] muteButton;
    public Sprite muteIcon, unmuteIcon;
    AudioSource[] audioSources;
    AudioSource publicAudioSource;
    public InputField nameInput, mailInput;
    public Text nameCovertText, mailCoverText;
    public Transform[] shakingTransforms;
    public Transform videoPanel;
    public Transform videoBackground, gameOverPanel;
    public Transform mainMenuPanel;
    public Text gameOverCountDownText, countDownToResumeText;
    public int countDownStartTime = 5, textXPosition1 = -500, textXPosition2 = 0, textXPosition3 = 500, textYPosition = -502, countDownEndTime = 0;
    public float countDownUnit = 1;
    float[] maxTimer;
    Tween enterTween, exitTween;
    AudioSource cameraAudioSource;
    public static bool isPaused, isGameOver, hasWatchedTheVideo = false, videoWatchingSelected = false, skippedVideo = false;
    public enum Modes { Normal, Hardcore };
    public static int normalModeScore = 0, hardcoreModeScore = 0;
    public static Modes mode = Modes.Normal;
    public float factor;
    public creator creator;
    public creator_LR creatorHardcore;
    Tween videoPanelScalingUpwards, videoPanelBackgroundColoring;
    public static Color color1, color2;

    public GameObject pipelineZero, pipelineOne; //perfabs
    [SerializeField]
    GameObject watchVideoButton;
	private static string adId = null;
	//PoolObject[] pipelineZeroes, pipelineOnes, pipelineZOs;
    //Singletone
    static Settings instance;
    public static Settings Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No instance of Settings.");
                return null;
            }
            return instance;
        }
    }
    [SerializeField]
    AudioClip[] soundClips;

    public Transform[] menuButtons;
    public enum SoundTypes { Mainmenu = 0, Gameplay = 1, GameOver = 2, SuckZero = 3, SuckOne = 4, Button = 5, Beep = 6, Whoosh = 7, none }//Ordering is important.

    void Awake()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        for (int i = 0; i < allButtons.Length; i++)
        {
            allButtons[i].onClick.AddListener(PlayButtonSound);
        }
        instance = GetComponent<Settings>();
        gameOverPanel.gameObject.SetActive(false);
        Pause();
		Tapsell.initialize ("ktqoirkampoamskicdroahjfhltpahokcdbqbochfhaigfomeklfamgaobsshfsfkoebgi");
		Tapsell.setRewardListener( (TapsellAdFinishedResult result) => 
			{
				// you may give rewards to user if result.completed and
				// result.rewarded are both true
				Debug.Log("Video was seen");
				tapsleDebug.text = "Video Played Completely";
				//		hasWatchedTheVideo = true;//never used in an if;
				//		PlayerPrefs.SetInt("hasReachedVideoLimit", 1);
				CountDownToPlay();
			}
		);
        //DeveloperInterface.getInstance().init("ktqoirkampoamskicdroahjfhltpahokcdbqbochfhaigfomeklfamgaobsshfsfkoebgi");
        OneSignal.StartInit("1848cfa7-b904-4332-9dd4-8105b74b80c5").EndInit();

        color1 = new Color(1f, (151f / 255f), 29f / 255f);
        color2 = new Color(29f / 255f, 210f / 255f, 1f);
        creator.normalCanvas.SetActive(true);
        creator.normalCanvas.transform.FindChild("MainMenu Panel").gameObject.SetActive(true);
        //creator = FindObjectOfType<creator>();
        //creatorHardcore = FindObjectOfType<creator_LR>();

        //Color randomColor = new Color();
        //if (Random.Range(0f,1f) < 0.5f)
        //{
        //    randomColor = creator.color1;
        //}
        //else
        //{
        //    randomColor = creator.color2;
        //}
        //print("color: " + randomColor);
        //videoPanelBackgroundColoring = videoBackground.GetComponent<Image>().DOColor(Random.Range(0f, 1f) < 0.5f ? creator.color1 : creator.color2, countDownStartTime).Pause();
        videoPanelScalingUpwards = videoBackground.DOScaleY(3, countDownStartTime).Pause().SetAutoKill(false);//.OnPlay(() => videoPanelBackgroundColoring.Play()).OnRewind(() => videoPanelBackgroundColoring.Rewind());
        publicAudioSource = GetComponent<AudioSource>();
        audioSources = GameObject.FindObjectsOfType<AudioSource>();
        cameraAudioSource = Camera.main.GetComponent<AudioSource>();
        isMute = false;
        for (int i = 0; i < muteButton.Length; i++)
        {
            //muteButton[i].sprite = muteIcon;
            muteButton[i].color = Color.white;
        }
    }
    // Gets called when the player opens the notification.

    void Start()
    {
        //InvokeRepeating("DecMaxTimer", 3, 8F);
        //InvokeRepeating("MoreSpeed", 3, 8F);
        //for (int i = 0; i < 5; i++)
        //{
        //    GameObject newInstance;

        //    newInstance = Instantiate<GameObject>(pipelineOne);
        //    newInstance.SetActive(false);
        //    //pipelineOnes.Add(newInstance, false);

        //    newInstance = Instantiate<GameObject>(pipelineZero);
        //    newInstance.SetActive(false);
        //    //pipelineZeroes.Add(newInstance, false);
        //}
        Pause();
        OneSignal.RegisterForPushNotifications();
        //creatorHardcore = FindObjectOfType<creator_LR>();
        maxTimer = new float[3];

        for (int i = 0; i < shakingTransforms.Length; i++)
        {
            //shakingTransforms[i].DOLocalJump(new Vector3(0,2,0),1,2,0.5f).SetLoops(-1,LoopType.Yoyo);
        }
        enterTween = gameOverCountDownText.transform.DOLocalMoveX(textXPosition2, countDownUnit / 2).Pause().SetAutoKill(false).SetEase(Ease.OutBack);
        exitTween = gameOverCountDownText.transform.DOLocalMoveX(textXPosition3, countDownUnit / 2).Pause().SetAutoKill(false).SetEase(Ease.InBack);//.OnComplete(() => countDownText.transform.localPosition = new Vector3(textXPosition1, textYPosition, 0))
        Sequence mainMenuButtonsSeq = DOTween.Sequence();
        //mainMenuButtonsSeq.Join(menuButtons[4].DOPunchPosition(new Vector3(0, 8, 0) * 10, 0.8f, DEBUG_Vibrato*5));
        //mainMenuButtonsSeq.Join(menuButtons[5].DOPunchPosition(new Vector3(0, 8, 0) * 10, 1f, DEBUG_Vibrato*5));
        for (int i = 0; i < 4; i++)
        {
            //Tween myTween = menuButtons[i].DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.4f, 2);//.SetLoops(-1, LoopType.Incremental).SetDelay((float)i / 4)
            //mySeq.Insert((i) * DEBUG_1, menuButtons[i].DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), DEBUG_1, DEBUG_Vibrato));
            mainMenuButtonsSeq.Insert((i) * 0.1f, menuButtons[i].DOPunchScale(new Vector3(0.1f * DEBUG_ScaleFACTOR, 0.1f * DEBUG_ScaleFACTOR, 0.1f * DEBUG_ScaleFACTOR), DEBUG_1, DEBUG_Vibrato, 2));//DEBUG_1=0.2
            //menuButtons[i].GetComponent<Image>().DOColor(Color.red, 0.3f).OnComplete(() => menuButtons[i].GetComponent<Image>().DOColor(Color.white,0.3f));
        }
        
        Sequence titleSeq = DOTween.Sequence();
        Tween[] punchTitle = new Tween[2];
        
        punchTitle[0] = menuButtons[4].DOPunchScale(new Vector3(0.8f, 1f, 0), 0.5f,5);
        punchTitle[1] = menuButtons[5].DOPunchScale(new Vector3(1f, 0.8f, 0), 0.5f,5);

        Tween[] scaleTitle = new Tween[2];
        scaleTitle[0] = menuButtons[4].DOScale(1.1f, 0.8f).SetEase(Ease.Unset).OnComplete(() => menuButtons[4].DOScale(1f, 0.8f).SetEase(Ease.Unset));
        scaleTitle[1] = menuButtons[5].DOScale(1.1f, 0.8f).SetEase(Ease.Unset).OnComplete(() => menuButtons[5].DOScale(1f, 0.8f).SetEase(Ease.Unset));

        //titleSeq.Insert(0, punchTitle[0]);
        //titleSeq.Insert(0.1f, punchTitle[1]);
        //titleSeq.Append(menuButtons[4].DOScale(1.5f,0.8f).SetEase(Ease.InBack));
        //titleSeq.Append(menuButtons[5].DOScale(1.5f, 0.8f).SetEase(Ease.InBack));
        //titleSeq.Append(menuButtons[4].DOPunchPosition(new Vector3(0, 3, 0) * 10, 2f));
        //titleSeq.Join(menuButtons[5].DOPunchPosition(new Vector3(0, 3, 0) * 10, 2f));
        //titleSeq.SetLoops(-1);
        //titleSeq.Play();

        mainMenuButtonsSeq.SetLoops(-1);
        mainMenuButtonsSeq.timeScale *= 0.9f;
        //mySeq.SetEase();
        mainMenuButtonsSeq.Play();
    }
    public float DEBUG_1 = 0.3f, DEBUG_2 = 0.2f,DEBUG_ScaleFACTOR;
    public int DEBUG_Vibrato=2;
    public void ResetMaxTimer()
    {
        if (mode == Modes.Normal)
        {
            maxTimer[0] = 1.5f;
            maxTimer[1] = 1.3f;
            maxTimer[2] = 1.7f;
        }
        else
        {
            maxTimer[0] = 3f;
            maxTimer[1] = 2.4f;
            maxTimer[2] = 2.5f;

        }
    }
    public Text tapsleDebug;
    // Gets called when the player opens the notification.
    public void PlayVideo()
    {
        tapsleDebug.text = "playstart";
		Debug.Log("PlayVideo() called");

		if(Settings.adId!=null)
		{
			TapsellShowOptions showOptions = new TapsellShowOptions ();
			showOptions.backDisabled = false;
			showOptions.immersiveMode = false;
			showOptions.rotationMode = TapsellShowOptions.ROTATION_UNLOCKED;
			showOptions.showDialog = true;
			Tapsell.showAd (Settings.adId, showOptions);
		}
        
		//DeveloperInterface.getInstance().showNewVideo(0, DeveloperInterface.VideoPlay_TYPE_NON_SKIPPABLE, (bool connected2, bool isAvailable2, int num) =>
		//{
		//		Debug.Log("showNewVideo() finished");
		//		tapsleDebug.text = "Video Played Completely";
		//		hasWatchedTheVideo = true;//never used in an if;
		//		PlayerPrefs.SetInt("hasReachedVideoLimit", 1);
		//		CountDownToPlay();
		//});

		//DeveloperInterface.getInstance().checkCtaAvailability(0, DeveloperInterface.VideoPlay_TYPE_NON_SKIPPABLE, (bool connected, bool isAvailable) =>
        //{
        //    tapsleDebug.text = "step2";
        //    tapsleDebug.text = "cnn:" + connected;
        //    tapsleDebug.text += " avb:" + isAvailable;
        //    if (connected && isAvailable)
        //    {
        //        tapsleDebug.text = "step3";
        //        tapsleDebug.text = "connected and available ";
        //        DeveloperInterface.getInstance().showNewVideo(0, DeveloperInterface.VideoPlay_TYPE_NON_SKIPPABLE, (bool connected2, bool isAvailable2, int num) =>
        //         {
        //             tapsleDebug.text = "Video Played Completely";
        //             hasWatchedTheVideo = true;//never used in an if;
        //             PlayerPrefs.SetInt("hasReachedVideoLimit", 1);
        //             CountDownToPlay();
        //         });
        //    }
        //});

    }
    public static void DestroyPipelineZOs()
    {
        GameObject[] pipelineZOs = GameObject.FindGameObjectsWithTag("ZO");
        for (int i = 0; i < pipelineZOs.Length; i++)
        {
            if (pipelineZOs[i].name.Contains("Clone"))
            {
                Destroy(pipelineZOs[i]);
            }
        }
    }
    // Update is called once per frame
    public void muteBtn()
    {
        isMute = !isMute;
        //Camera.main.GetComponent<AudioListener>().enabled = !isMute;
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.mute = isMute;
        }
        //SpriteRenderer[] muteButtons = GameObject.fin
        Sprite icon;
        Color muteButtonsColor;
        if (isMute)
        {
            icon = unmuteIcon;
            muteButtonsColor = Color.red;
        }
        else
        {
            icon = muteIcon;
            muteButtonsColor = Color.white;
        }
        for (int i = 0; i < muteButton.Length; i++)
        {
            //muteButton[i].color = muteButtonsColor;
            //muteButton[i].sprite = icon;
            muteButton[i].color = muteButtonsColor;
        }
        //AudioSource[] sounds = FindObjectsOfType<AudioSource>();
        //foreach (AudioSource item in audioSources)
        //    {
        //    item.mute = isMute;
        //    }
        //if (isMute)
        //    {
        //    Camera.main.GetComponent<AudioSource>().Play();
        //    } 
    }
    void DecMaxTimer()
    {
        if (!Settings.isPaused)
        {
            //Debug.Log("calling decMax on" + Time.timeSinceLevelLoad);
            for (int i = 0; i < maxTimer.Length; i++)
            {
                if (maxTimer[i] > 0.8)
                {
                    maxTimer[i] -= 0.03f;
                }
            }
            //Debug.Log("maxTimer[1]: " + maxTimer[1]); 
        }

    }
    public static float speed;
    /// <summary>
    /// Is called every score
    /// </summary>
    public float speedIncreaseStep;
    float diffSpeed;
    public void MoreSpeed(int currentScore)
    {
        //float floatedScore = currentScore /15;
        if (currentScore % (int)speedIncreaseStep == 0)
        {
            diffSpeed *= 0.95f;//decrese the amount of increase by 5%;
            print("speed" + "+diffSpeed= " +  + speed + " + "+ diffSpeed + " = " + speed + diffSpeed + " : with score: " + currentScore);
            speed += diffSpeed;//(Mathf.Abs(Mathf.Log10(speed))); 
            speedIncreaseStep = speedIncreaseStep * 1.5f;
        }
    }

    public static float GetRandomMaxTimer()
    {
        return instance.maxTimer[(int)Random.Range(0, instance.maxTimer.Length)];
    }
    public void GameStarted()
    {
        speedIncreaseStep = 10;
        ResetMaxTimer();
        diffSpeed = 0.12f;
        isGameOver = false;
        Unpause();
    }
    public void ConvertToPersian(InputField input)
    {
        Text cover = input.transform.FindChild("CoverText").GetComponent<Text>();
        //outputText.text = input.text;
        //outputText.text = "" + outputText.text.faConvert();
        cover.text = "" + input.text.faConvert();
    }
    public void EnterAnimation(GameObject target)
    {
        PlaySound(SoundTypes.Whoosh);
        target.transform.position = new Vector3(836, target.transform.position.y, 0);
        target.SetActive(true);
        target.transform.DOLocalMoveX(0, 0.3f, true);
    }
    public void ExirAnimation(GameObject target)
    {
        PlaySound(SoundTypes.Whoosh);
        //target.transform.position = new Vector3(0, target.transform.position.y, 0);
        target.SetActive(true);
        target.transform.DOLocalMoveX(836, 0.3f).SetEase(Ease.InBack).OnComplete(() => target.SetActive(false));
    }
    public void PanelOutScaleRotate(Transform target)
    {
        target.gameObject.SetActive(true);

        target.localScale = Vector3.one;

        target.eulerAngles = new Vector3(0, 0, -180);
        target.DORotate(new Vector3(0, 0, 0), 0.8f).SetEase(Ease.OutExpo);
        target.DOScale(0, 0.8f).SetEase(Ease.OutExpo).OnComplete(() => { target.localScale = Vector3.one; target.eulerAngles = Vector3.zero; target.gameObject.SetActive(false); });
    }
    public void PanelInScaleRotate(Transform target)
    {
        target.gameObject.SetActive(true);

        target.localScale = Vector3.zero;

        target.eulerAngles = new Vector3(0, 0, -180);
        target.DORotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InExpo);
        target.DOScale(1, 0.5f).SetEase(Ease.InExpo);
    }
    public void StartVideoPanel()
    {
        videoPanel.gameObject.SetActive(true);
        skippedVideo = false;
        hasWatchedTheVideo = false;
        videoWatchingSelected = false;
        watchVideoButton.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 3, 5);
        creator.scoreDigitalLCD.SetActive(false);
        //print(creatorHardcore.name);
        //print(creatorHardcore.scoreDigitalLCDLeft.name);
        creatorHardcore.scoreDigitalLCDLeft.SetActive(false);
        creatorHardcore.scoreDigitalLCDRight.SetActive(false);

        //videoPanel.localScale = Vector3.one;//DOScale(1, 0.6f).OnComplete(() => StartCoroutine(StartCountingDown()));
        StartCoroutine(StartCountingDown());
    }
    IEnumerator StartCountingDown()
    {
        gameOverCountDownText.gameObject.SetActive(true);
        gameOverCountDownText.transform.localPosition = new Vector3(textXPosition1, textYPosition, 0);
        videoPanel.gameObject.SetActive(true);
        videoPanel.localPosition = Vector3.zero;
        videoPanelScalingUpwards.Restart();

        float i = countDownStartTime;
        while (i > countDownEndTime)
        {
            gameOverCountDownText.text = i.ToString();
            //print(gameOverCountDownText.text = i.ToString());
            enterTween.Play();

            yield return new WaitUntil(() => enterTween.IsComplete());

            exitTween.Play();

            yield return new WaitUntil(() => exitTween.IsComplete());

            enterTween.Rewind();
            exitTween.Rewind();
            //reset CountDownText pos:
            gameOverCountDownText.transform.localPosition = new Vector3(textXPosition1, textYPosition, 0);

            //step:
            i -= countDownUnit;
            if (videoWatchingSelected || skippedVideo)
            {
                //i = countDownEndTime;
                yield break;
            }
        }
        //After Last Count Down
        //exitTween.Play();
        //yield return new WaitUntil(() => exitTween.IsComplete());
        //enterTween.Rewind();
        //exitTween.Rewind();
        //if (videoWatchingSelected)
        //{
        //    yield return 0;
        //}
        //if (!videoWatchingSelected)
        //{
        print("calling CountDownEnded()");
        ShowGameOverPanel();
        //}
    }
    public void ShowGameOverPanel()
    {
        creator.scoreDigitalLCD.SetActive(false);
        //print(creatorHardcore.name);
        //print(creatorHardcore.scoreDigitalLCDLeft.name);
        creatorHardcore.scoreDigitalLCDLeft.SetActive(false);
        creatorHardcore.scoreDigitalLCDRight.SetActive(false);
        PauseMusic();
        PlaySound(SoundTypes.GameOver);

        gameOverCountDownText.gameObject.SetActive(false);

        //reset background scale
        videoBackground.localScale = Vector3.one;

        //move the video panel to it's start state.
        videoPanel.localPosition = Vector3.zero;
        videoPanel.gameObject.SetActive(false);
        //videoPanel.DOLocalMoveX(837, countDownUnit / 2).OnComplete(() => { videoPanel.localPosition = Vector3.zero; videoPanel.gameObject.SetActive(false); });

        //bring on the gameOver panel
        print("bring on the gameOver panel");
        gameOverPanel.gameObject.SetActive(true);

        gameOverPanel.GetComponent<Animator>().SetTrigger("beat");

        gameOverPanel.localPosition = Vector3.zero;
        gameOverPanel.localScale = Vector3.zero;
        gameOverPanel.DOScale(1, countDownUnit / 2);

        if (mode == Modes.Normal)
        {
            creator.GameOver();
        }
        else if (mode == Modes.Hardcore)
        {
            creatorHardcore.GameOver();
        }
    }

	public static void getVideoAd()
	{
		Tapsell.requestAd("58890d3b4684656e2c7f1c96", false, 
			(TapsellResult result) => {
				// onAdAvailable
				Debug.Log("Action: onAdAvailable, storing ad");
				Settings.adId = result.adId; // store this to show the ad later
				Debug.Log("Action: ad stored");
			},

			(string zoneId) => {
				// onNoAdAvailable
				Debug.Log("No Ad Available");
				Settings.adId = null;
			},

			(TapsellError error) => {
				// onError
				Debug.Log(error.error);
				Settings.adId = null;
			},

			(string zoneId) => {
				// onNoNetwork
				Debug.Log("No Network");
				Settings.adId = null;
			},

			(TapsellResult result) => {
				// onExpiring
				Debug.Log("Expiring");
				// this ad is expired, you must download a new ad for this zone
				Settings.adId = null;
				getVideoAd();
			}
		);
	}

    //public ShowMovie tapselMovieForShow;
    public void WatchVideoBtn()
    {
        //videoWatchingSelected = false;
        //    videoWatchingSelected = tapselMovieForShow.playVideo();
        videoWatchingSelected = true;
        if (videoWatchingSelected)
        {
            StopCoroutine(StartCountingDown());
            videoPanelScalingUpwards.Rewind();
            videoPanel.localPosition = Vector3.zero;

            PlayVideo();
            //StartCoroutine(WatchVideo_IE());
        }
    }
    IEnumerator WatchVideo_IE()
    {
        videoPanel.gameObject.SetActive(false);//videoPanel.DOLocalMoveX(800, 0.6f).OnComplete(() => videoPanel.gameObject.SetActive(false));
        yield return new WaitForSeconds(2);//wait for user to watch video
        print("Player watched the video");
        hasWatchedTheVideo = true;
        CountDownToPlay();
    }
    public void SkipVideoPanel()
    {
        skippedVideo = true;
        ShowGameOverPanel();
    }
    void CountDownToPlay()
    {
        countDownToResumeText.gameObject.SetActive(true);
        countDownToResumeText.DOText("Play!", 2, true, ScrambleMode.Uppercase, "-!?").OnComplete(() => { ResumeAfterVideo(); countDownToResumeText.gameObject.SetActive(false); }).SetEase(Ease.OutQuad);//"@#$%&+-|<>?"
    }
    public void ResumeAfterVideo()
    {
        countDownToResumeText.text = "";
        countDownToResumeText.gameObject.SetActive(false);
        //change music 
        PlayMusic(SoundTypes.Gameplay);

        Unpause();
    }

    public void PlayMusic(SoundTypes music)//mode should have been set before calling this method, because it uses mode conditioning.
    {
        cameraAudioSource.clip = soundClips[(int)music];
        cameraAudioSource.Play();
    }
    public void PauseMusic()
    {
        cameraAudioSource.Pause();
    }
    public void PlaySound(SoundTypes sound)
    {
        //print("playing sound: " + (int)sound);
        publicAudioSource.clip = soundClips[(int)sound];
        publicAudioSource.Play();
    }
    public void goToMainMenu()
    {
        CancelInvoke();
        //Application.LoadLevel(Application.loadedLevel);
        if (mode == Modes.Normal)//THAT'S WHY INHERITANCE IS GOOD.
        {
            for (int i = 0; i < creator.instances.Length; i++)
            {
                creator.instances[i].FakeDestroy();
            }
        }
        else
        {
            for (int i = 0; i < creatorHardcore.instances.Length; i++)
            {
                creatorHardcore.instances[i].FakeDestroy();
            }
        }
        //mainMenuPanel.localScale = Vector3.zero;
        //mainMenuPanel.eulerAngles = new Vector3(0,0,180);
        //mainMenuPanel.DOScale(1, 0.6f);
        //mainMenuPanel.DORotate(Vector3.zero, 0.6f);
        //PanelInScaleRotate(mainMenuPanel);
        Settings.DestroyPipelineZOs();
        isPaused = true;
        Time.timeScale = 1;
        //mainMenuAnimator.SetTrigger("fadeout");
        PlayMusic(SoundTypes.Mainmenu);
    }
    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (mode == Modes.Normal)
        {
            creator.Resume(true);
        }
        else if (mode == Modes.Hardcore)
        {
            creatorHardcore.Resume(true);
        }
    }
    public void Replay()
    {
        Unpause();
        if (mode == Modes.Normal)
        {
            creator.startBtn();
        }
        else if (mode == Modes.Hardcore)
        {
            creatorHardcore.startBtn();
        }
    }
    public void Pause()
    {
        isPaused = true;
        if (mode == Modes.Normal)
        {
            //print(creator.name);
            //print(creator.scoreDigitalLCD.name);
            //creator.scoreDigitalLCD.SetActive(false);
            //creator.pauseButton.SetActive(false);
            creator.Resume(false);
        }
        else if (mode == Modes.Hardcore)
        {
            creatorHardcore.Resume(false);
            //creatorHardcore.scoreDigitalLCDRight.SetActive(false);
            //creatorHardcore.scoreDigitalLCDLeft.SetActive(false);
            //creatorHardcore.pauseButton.SetActive(false);
        }
    }
    public void PauseTime()
    {
        Time.timeScale = 0;
    }
    void PlayButtonSound()
    {
        PlaySound(SoundTypes.Button);
    }
    public void ClearConsole()
    {
        Debug.ClearDeveloperConsole();
    }
}

//class PoolObject
//{
//    bool isBeingUsed;
//    GameObject instance;
//    void FakeStart(Vector3 pos,Color color)
//    {
//        if (instance == null)
//        {
//            instance = new GameObject();
//        }
//    }
//    void FakeDestroy()
//    {

//    }
//}