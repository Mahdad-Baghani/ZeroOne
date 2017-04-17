using DG.Tweening;
//using ClassLibrary1_TestForUnityDLL;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;
public class creator : MonoBehaviour
{
    public static bool isRegisterPanelOn = true;
    float timer, maxRange;
    public GameObject Zero, One, pipelineZero, pipelineOne, capsule, pipelineZO, registerModal;//zero, one prefabs are no longer getting used.
    public Transform Top, Bottom, pipelineRightSpawnPoint, pipelineLeftSpawnPoint;
    public static int volume, difficulty, powerupScore;
    
    public static string username;
    public Text scoreText, gameOverScore;
    public Text helptext;
    public Animator mainMenuAnimator, gameOverAnimator, capsuleAnimator, scoreCircleAnimator;
    
    public Text helpText, scoreComment;
    //public Animator fuse1_Animator, fuse2_Animator;
    public enum Powerup { TopIsBlocked, BottomIsBlocked, none };
    public static Powerup powerup;
    public SpriteRenderer topSpawner, bottomSpawner, laserTop, laserBottom;
    public static float scoreToGetFuse;
    Tweener capsulRotation, scoreShakeTween;
    //public LaserChargerSystem laserSystem;
    public GameObject normalCanvas, videoPanel, vacuumZero, vacuumOne;
    public GameObject scoreDigitalLCD;
    public SpriteRenderer[] light;
    public NumberObject[] instances;//pool object
    public Text countDownText;
    bool isOneUp = true;
    Color chosenColor;
    public GameObject pauseButton;
    public BacktoryBackend network;
    Tween vacumOnePunch,vacumZeroPunch;

    void Awake()
    {
    }
    void Start()
    {
        vacumOnePunch = vacuumOne.transform.DOPunchPosition(new Vector3(0, -1, 0), 0.3f).Pause().SetAutoKill(false);
        vacumZeroPunch = vacuumZero.transform.DOPunchPosition(new Vector3(0, 1, 0), 0.3f).Pause().SetAutoKill(false);
        //capsule.transform.DOScale(new Vector3(0, 0.85f, 0), 0.5f).SetLoops(3, LoopType.Restart);

        //float sizeShake = 0.3f;
        //capsule.transform.DOShakeScale(.4f, new Vector3(sizeShake, sizeShake,0)).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

        //capsulRotation = capsule.transform.DOLocalRotate(new Vector3(0, 0, 180), 0.5f).SetRelative(true).SetAutoKill(false).SetId("capsulRotation");

        scoreShakeTween = scoreText.transform.DOShakeScale(0.5f, 0.5f, 10, 40).Pause().SetAutoKill(false);
        scoreText.DOColor(Color.green, 0.7f).SetId("scoreColor").Pause().SetAutoKill(false);
        //laserSystem.StopLaser();

        //set scores || refresh?
        if (PlayerPrefs.GetInt("bestScore") > 0)
        {
            //helpText.text = "Match The NUMBERRRS!\n Best Score: " + PlayerPrefs.GetInt("bestScore");
        }
        else
        {
            //helpText.text = "Match The Numbers!";
        }
        capsuleAnimator = capsule.GetComponent<Animator>();
        
    }

    public void ToggelRegisterPanel()
    {
        if (PlayerPrefs.GetInt("registration") == 1)
        {
            return;
        }
        if (!isRegisterPanelOn)
        {
            registerModal.transform.DOLocalMoveX(10, 0.6f).SetEase(Ease.InBack);
        }
        else
        {
            registerModal.transform.DOLocalMoveX(-777, 0.6f).SetEase(Ease.OutBack).OnComplete(() => { registerModal.SetActive(false); }); //MAHDAD has duplicated this in his RegistrationMenu.cs that should be removed.
        }
        isRegisterPanelOn = !isRegisterPanelOn;
    }
    void Update()
    {
        #region FOR_DEBUG
        //if (Input.GetKey(KeyCode.A))
        //{
        //    SetPowerup(Powerup.BottomIsBlocked, 5);
        //}
        //else if (Input.GetKey(KeyCode.F))
        //{
        //    SetPowerup(Powerup.TopIsBlocked, 5);
        //}

        if (Input.GetButton("Jump"))
        {
            PlayerPrefs.DeleteAll();
        }
        #endregion
        if (!Settings.isPaused)//if this gameObject is disabled, Update method wouldn't be called.
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Input.mousePosition;
                Collider2D hitCollider = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero, 10, 1 << 5).collider;//enables player to only touch the UI.
                if (hitCollider == null)//hitCollider.name.Contains("pipe") || hitCollider.name.Contains("Effector") || hitCollider.name.Contains("capsule"))
                {
                    RotateCapsule();
                }
                else//should have hit the pause btn in UI layer.
                {
                    print("hit: " + hitCollider.GetComponent<Collider2D>().gameObject);
                }
            }
            timer += Time.deltaTime;

            //float maxTimerChosen;
            //maxTimerChosen = maxTimer[(int)Random.Range(0, maxTimer.Length)];
            if (timer > Settings.GetRandomMaxTimer())// SPAWN! 
            {
                NumberObject newBorn;// = new NumberObject();

                int index = (int)Random.Range(0, instances.Length);
                newBorn = instances[index];
                while (instances[index].isBeingUsed)//find a free zo.
                {
                    index = (int)Random.Range(0, instances.Length);
                    newBorn = instances[index];
                    //if (index == instances.Length)
                    //{
                    //    NumberObject newInstance = new NumberObject();
                    //    //instances.
                    //}
                }
                if (powerup == Powerup.TopIsBlocked || Random.Range(0f, 1f) < 0.5)
                {
                    newBorn.startPoint = Bottom.position;
                }
                else
                {
                    newBorn.startPoint = Top.position;
                }


                if (Random.Range(0f, 50f) > 25)
                {
                    newBorn.GetComponent<SpriteRenderer>().color = chosenColor = Settings.color1;
                    //chosenColor = "goToOrange";
                }
                else
                {
                    newBorn.GetComponent<SpriteRenderer>().color = chosenColor = Settings.color2;
                    //chosenColor = "goToBlue";

                }

                //turn on the lights
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    if (Random.Range(0f, 1f) > 0.5f)
                    {
                        light[1].color = light[0].color = Settings.color1;
                    }
                    else
                    {
                        light[1].color = light[0].color = Settings.color2;
                    }
                }
                else
                {
                    if (Random.Range(0f, 1f) > 0.5f)
                    {
                        light[3].color = light[2].color = Settings.color1;
                    }
                    else
                    {
                        light[3].color = light[2].color = Settings.color2;
                    }
                }
                Invoke("TurnOffTheLights", 0.5f);
                if (newBorn.startPoint == Top.position)
                {
                    newBorn.FakeCreate(true);
                    //print("TOP "+chosenColor);
                    laserTop.color = chosenColor;
                    //laserTop.SetTrigger(chosenColor);
                }
                else
                {
                    newBorn.FakeCreate(false);
                    //print("BOTTOM " + chosenColor);
                    laserBottom.color = chosenColor;
                    //laserBottom.SetTrigger(chosenColor);
                }
                //Debug.Log("maxTimer changed to: " + maxTimer);
                timer = 0;
            }
        }
        //Create Battery
        //if (powerupScore == (int)scoreToGetFuse && powerup == Powerup.none)
        //{
        //    powerupScore = 0;
        //    if (Random.Range(0f, 1f) < 0.5f)
        //    {
        //    }
        //    else
        //    {
        //    }
        //}
    }
    public void TurnOnCapsuleLights()
    {
        if (chosenColor.r == 1)
        {
            //print("chosenColor is: Orange"); 
        }
        else
        {
            //print("chosenColor is: Blue"); 
        }
        for (int i = 4; i < 8; i++)
        {
            light[i].DOColor(chosenColor, 0.4f).SetEase(Ease.OutBounce).From();
        }

    }
    public void TurnOffCapsuleLights()
    {
        for (int i = 4; i < 8; i++)
        {
            light[i].color = new Color(0, 0, 0, 0);
        }
    }
    private void RotateCapsule()
    {
        //capsule.transform.DOLocalRotate(new Vector3(0, 0, 180), 0.6f).SetRelative(true).SetAutoKill(false).SetId("capsulRotation").SetEase(Ease.OutElastic).OnComplete(() => StraightenCapsul()); 
        //if (isOneUp)
        //{
        //    capsulRotation.Play().OnComplete(() => capsulRotation.Flip());
        //}
        //else
        //{
        //    capsulRotation.PlayBackwards();
        //}
        //capsulRotation.Play().OnComplete(() => capsulRotation.Rewind()).OnComplete(() => capsulRotation.Flip());
        capsuleAnimator.SetTrigger("rotate");
        scoreCircleAnimator.SetTrigger("rotate");
        isOneUp = !isOneUp;
        //capsule.transform.DORotate(new Vector3(0, 0, 180), 0.3f).Flip();
        //DOTween.TweensById("capsulRotation")[0].timeScale *=-1;
        //DOTween.TweensById("capsulRotation")[0].Restart();
    }
    void StraightenCapsul()
    {
        if (capsule.transform.localEulerAngles.z % 180 != 0)
        {
            capsule.transform.DOLocalRotate(new Vector3(0, 0, 0), 0);//can't be doen withe euler angles?
        }
    }
    
    public void SetPowerup(Powerup type, float second)//type lasts "second" seconds.
    {
        powerup = type;
        //Debug.Log("Powerup " + type.ToString() + " activated");
        switch (type)
        {
            case Powerup.TopIsBlocked:
                topSpawner.color = laserTop.color = Color.black;
                break;
            case Powerup.BottomIsBlocked:
                bottomSpawner.color = laserBottom.color = Color.black;
                break;
            case Powerup.none:
                break;
            default:
                break;
        }
        Invoke("SetPowerupNone", second);
    }
    /// <summary>
    /// Disable the powerup
    /// </summary>
    void SetPowerupNone()
    {
        if (powerup != Powerup.none)
        {
            //Debug.Log("Powerup " + powerup.ToString() + " deactivated");
        }
        Settings.speed = 0.4f;
        powerup = Powerup.none;
        Color chosenColor;
        if (Random.Range(0f, 50f) > 25)
        {
            chosenColor = Settings.color1;
        }
        else
        {
            chosenColor = Settings.color2;
        }
        if (powerup == Powerup.TopIsBlocked)
        {
            laserTop.color = chosenColor;
        }
        else if (powerup == Powerup.BottomIsBlocked)
        {
            laserBottom.color = chosenColor;
        }
        topSpawner.color = laserTop.color = Color.white;
        bottomSpawner.color = laserBottom.color = Color.white;
    }

    /// <summary>
    /// Increases the score and powerupScore if no power up was enable;
    /// </summary>
    /// <param name="i"></param>
    public void IncreseScore(int i)
    {
        //print(i);
        Settings.normalModeScore++;
        Settings.Instance.MoreSpeed(Settings.normalModeScore);
        //print("Score: " + score);
        //if (powerup == Powerup.none)
        //{
        //    powerupScore++;
        //}
        scoreText.text = Settings.normalModeScore.ToString();
        //scoreAnimator.SetTrigger("scored");
        scoreShakeTween.Restart();//
        TurnOnCapsuleLights();

        //pipeline 
        Vector3 pipelinePos;
        if (Random.Range(0f, 1f) > 0.5f)
        {
            pipelinePos = pipelineRightSpawnPoint.position;
        }
        else
        {
            pipelinePos = pipelineLeftSpawnPoint.position;
        }

        if (i == 1)
        {
            //vacuumOne.transform.DOPunchPosition(new Vector3(0, -1, 0), 0.3f); : cached
            vacumOnePunch.Play();

            pipelineZO = Instantiate(pipelineOne, pipelinePos, Quaternion.identity) as GameObject;
        }
        else
        {
            //vacuumZero.transform.DOPunchPosition(new Vector3(0, 1, 0), 0.3f); : cached
            vacumZeroPunch.Play();
            pipelineZO = Instantiate(pipelineZero, pipelinePos, Quaternion.identity) as GameObject;
        }
        pipelineZO.GetComponent<SpriteRenderer>().color = chosenColor;

        //Invoke("TurnOffCapsuleLights", 0.2f);
        //TurnOffTheLights();
        //scoreText.transform.DOShakeScale(0.5f,0.2f,15,40);
    }

    public void DecScore()
    {
        Settings.isGameOver = true;
        //show VideoPanel
        if (PlayerPrefs.GetInt("hasReachedVideoLimit") == 0)
        {
            PlayerPrefs.SetInt("hasReachedVideoLimit", 1);
            ShowVideoPanel();
        }
        else
        {
            //game over
            Settings.Instance.ShowGameOverPanel();
        }
        //if Declined, then GameOver()
        //GameOver();
        //score--;// or Game over
        //if (score<1)
        //{
        //    score = 0;
        //}
    }
    void TurnOffTheLights()
    {
        for (int j = 0; j < 4; j++)
        {
            light[j].color = new Color(0, 0, 0, 0);
        }
    }
    public void ShowVideoPanel()
    {
        pauseButton.SetActive(false);
        //videoPanel.SetActive(true);
        Settings.Instance.StartVideoPanel();
        //gameOverAnimator.gameObject.SetActive(true);
        //gameOverAnimator.transform.eulerAngles = new Vector3(0, 0, -180);
        //gameOverAnimator.transform.DOLocalMoveX(0,0.8f);
        //gameOverAnimator.SetTrigger("beat");

        for (int i = 0; i < instances.Length; i++)
        {
            instances[i].FakeDestroy();
        }
        //capsule.SetActive(false);
        Settings.Instance.Pause();
        Resume(false);
    }
    public void Die_Btn()
    {
        videoPanel.SetActive(false);
        GameOver();
    }
    void printThis(string msg)
    {
        print(msg);
    }

    public void GameOver()
    {
        CancelInvoke();
        Settings.Instance.Pause();
        Resume(false);
        BacktoryBackend.instance.SendPlayerStats(Settings.normalModeScore, 0);
        powerup = Powerup.none;

        //Invoke("gameOverMusicSet", 1.5f);
        //laserSystem.StopLaser();

        //save and compare score to best score
        scoreComment.gameObject.SetActive(true);
        if (Settings.normalModeScore > PlayerPrefs.GetInt("bestScore"))//if new record
        {

            scoreComment.text = "New Record!";

            PlayerPrefs.SetInt("bestScore", (int)Settings.normalModeScore);
        }
        else
        {
            scoreComment.text = "Best Score: " + PlayerPrefs.GetInt("bestScore");
        }

        //edit_3
        //NumberObject[] remained_zero_ones = FindObjectsOfType<NumberObject>();
        //foreach (NumberObject item in remained_zero_ones)
        //    {
        //    Destroy(item.gameObject);
        //    }

        //score = 0;
        scoreText.text = "0";
        //print("isFuseOpen: " + isFuse1_Open.ToString());
        gameOverScore.text = Settings.normalModeScore.ToString();

        for (int i = 0; i < instances.Length; i++)
        {
            instances[i].FakeDestroy();
        }

        //capsule.SetActive(false);//maybe can be put in resume (top)
        //gameOverAnimator.gameObject.SetActive(true);
        //gameOverAnimator.SetTrigger("beat");
        Settings.isGameOver = false;
    }

    public void startBtn()
    {
        PlayerPrefs.SetInt("hasReachedVideoLimit", 0);
        Settings.Instance.GameStarted();
        scoreDigitalLCD.SetActive(false);
        //invokeRepeating a function that does: maxTimer-=value;
        Settings.mode = Settings.Modes.Normal;
        //if (gameOverAnimator.gameObject.activeSelf == true)
        //{
        //    gameOverAnimator.transform.DORotate(new Vector3(0, 0, 180), 0.8f).SetEase(Ease.OutExpo);
        //    gameOverAnimator.transform.DOScale(0, 0.8f).SetEase(Ease.OutExpo).OnComplete(() =>
        //    {
        //        gameOverAnimator.transform.localScale = new Vector3(1, 1, 1);
        //        gameOverAnimator.gameObject.SetActive(false);
        //        gameOverAnimator.transform.eulerAngles = Vector3.zero;
        //    }
        //    );
        //}

        Resume(true);
        scoreText.transform.parent.gameObject.SetActive(true);
        //Camera.main.GetComponent<AudioListener>().enabled = !creator.isMute;
        Settings.normalModeScore = 0;
        scoreText.text = "00";
        powerupScore = 0;
        scoreToGetFuse = 2;

        //laserSystem.StopLaser();

        SetPowerupNone();

        timer = 0;
        Settings.speed = 0.4f;
        //invokeRepeating a function that does: maxTimer-=value;

        //mainMenuAnimator.SetTrigger("fadeout");
        //animation calls startTheGame();
        //Invoke("unpause", 0.3f);

        //change music 
        Settings.Instance.PlayMusic(Settings.SoundTypes.Gameplay);
        Settings.isGameOver = false;
        Settings.Instance.Unpause();

		Debug.Log ("Get Video Ad");

		Settings.getVideoAd ();

    }

    public void Resume(bool active)
    {
        pauseButton.SetActive(active);
        scoreDigitalLCD.SetActive(active);
        //capsule.SetActive(active);
        scoreCircleAnimator.gameObject.SetActive(active);
    }

    public void quit()
    {
        Application.Quit();
    }

}
