using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class creator_LR : MonoBehaviour
{
    float timer, maxRange, maxTimerChosen;
    public GameObject Zero, One, capsule_L, capsule_R, score_panel, videoPanel;
    public Transform leftTop, leftBottom, rightTop, rightBottom;
    public static int powerupScore;
    public static bool isGameOver;
    public static string username;
    public Text scoreTextLeft, gameOverScore, scoreTextRight, countDownText;
    public Animator gameOverAnimator, scoreAnimator, scoreCirclePanelLeft, scoreCirclePanelRight, capsuleLightsRight, capsuleLightsLeft;
    Animator leftCapsuleAnimator, rightCapsuleAnimator;
    public AudioClip inGameMusic, mainMenuMusic;
    //AudioSource mainAudioSource;
    public Text helpText, scoreComment;
    //public Animator fuse1_Animator, fuse2_Animator;
    public enum Powerup { TopIsBlocked, BottomIsBlocked, none };
    public static Powerup powerup;
    public SpriteRenderer topSpawner_L, bottomSpawner_L, laserTop_L, laserBottom_L, topSpawner_R, bottomSpawner_R, laserTop_R, laserBottom_R;
    public static float scoreToGetFuse;
    //
    //edit_3
    //pooling:
    public NumberObject_LR[] instances;
    public List<NumberObject_LR> instancesList;
    public SpriteRenderer[] capsuleLights, platformLights;
    Tweener scoreShakeTweenLeft, scoreShakeTweenRight;
    Color chosenColor_L, chosenColor_R;
    Tween[] capsuleLightsTween;
    public GameObject scoreDigitalLCDLeft, scoreDigitalLCDRight;
    public GameObject pauseButton;
    //Transform intanciatePoint_L, intanciatePoint_R;
    //GameObject numberObject_L, numberObject_R;
    //SpriteRenderer leftSideNumberSpriteRenderer, rightSideNumberSpriteRenderer;
    //
    
    void Start()
    {
        //settings = FindObjectOfType<Settings>();
        //CancelInvoke();
        //capsuleLightsTween = new Tween[8];
        //for (int j = 0; j < 8; j++)
        //{
        //    capsuleLightsTween[j] = capsuleLights[j].DOColor(creator.color2, 0.2f).SetEase(Ease.OutBounce).From().Pause().SetAutoKill(false);//.SetEase(Ease.OutBounce)
        //} 
        scoreShakeTweenLeft = scoreTextLeft.transform.DOShakeScale(0.5f, 0.5f, 10, 40).Pause().SetAutoKill(false);
        scoreShakeTweenRight = scoreTextRight.transform.DOShakeScale(0.5f, 0.5f, 10, 40).Pause().SetAutoKill(false);
        //set scores || refresh?
        if (PlayerPrefs.GetInt("bestScore") > 0)
        {
            //helpText.text = "Match The NUMBERRRS!\n Best Score: " + PlayerPrefs.GetInt("bestScore");
        }
        else
        {
            //helpText.text = "Match The Numbers!";
        }
        //isPaused = true;
        leftCapsuleAnimator = capsule_L.GetComponent<Animator>();
        rightCapsuleAnimator = capsule_R.GetComponent<Animator>();
        }
    void Update()
    {
        #region FOR_DEBUG
        if (Input.GetKey(KeyCode.A))
        {
            setPowerup(Powerup.BottomIsBlocked, 5);
        }
        else if (Input.GetKey(KeyCode.F))
        {
            setPowerup(Powerup.TopIsBlocked, 5);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            rightCapsuleAnimator.SetTrigger("rotate");
            scoreCirclePanelRight.SetTrigger("rotate");
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            leftCapsuleAnimator.SetTrigger("rotate");
            scoreCirclePanelLeft.SetTrigger("rotate");
        }

        #endregion

        if (!Settings.isPaused)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)// 
                {
                    Vector3 mousePos = Input.GetTouch(0).position;
                    //Debug.DrawLine(Vector2.zero, pos, Color.cyan);
                    Collider2D hitCollider = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero).collider;
                    if (hitCollider == null)//|| ((hitCollider.gameObject != fuse_1.gameObject) && (hitCollider.gameObject != fuse_2.gameObject))
                    {
                        if (mousePos.x > Screen.width / 2)
                        {
                            rightCapsuleAnimator.SetTrigger("rotate");
                            scoreCirclePanelRight.SetTrigger("rotate");

                        }
                        else if (mousePos.x <= Screen.width / 2)
                        {
                            leftCapsuleAnimator.SetTrigger("rotate");
                            scoreCirclePanelLeft.SetTrigger("rotate");
                        }
                        //print("rotated when: " + "hit = null");
                    }
                    else
                    {
                        //print("hit: " + hitCollider.GetComponent<Collider2D>().gameObject);
                    }
                }               
            }
            timer += Time.deltaTime;

            // creat two objects, complete them along the function:

            if (timer > Settings.GetRandomMaxTimer())
            {
                SpawnFromList();
                timer = 0;
            }
        }
        //print("Score: " + score);
        //Create Battery
        if (powerupScore == (int)scoreToGetFuse && powerup == Powerup.none)//score != 0 &&  !fuse_1.isOpened && !fuse_2.isOpened 
        {
            powerupScore = 0;
            //Instantiate(battery, new Vector3(100, 300, 0), new Quaternion());
            if (Random.Range(0f, 1f) < 0.5f)
            {
            }
            else
            {
            }
        }
    }

    private void SpawnFromList()
    {
        NumberObject_LR leftNewborn, rightNewborn;

        //instanciate two numbers, one for each side.
        //creating the left zo.
        int index = (int)Random.Range(0, instancesList.Count);//choose a random index of the pool.
        leftNewborn = instancesList[index];
        while (instancesList[index].isBeingUsed)//search for a free zo.
        {
            index = (int)Random.Range(0, instancesList.Count);
            leftNewborn = instancesList[index];
        }
        NumberObject_LR temp = instancesList[index];
        instancesList.RemoveAt(index);
        //creating the right zo.
        index = (int)Random.Range(0, instancesList.Count);
        rightNewborn = instancesList[index];

        instancesList.Add(temp);
        
        //while (instancesList[index].isBeingUsed)
        //{
        //    index = (int)Random.Range(0, instancesList.Count);
        //    rightNewborn = instancesList[index];
        //}


        //if (powerup == Powerup.TopIsBlocked)
        //{
        //    leftNewborn.startPoint = leftBottom.position;
        //    //intanciatePoint_L = Bottom_L;
        //    rightNewborn.startPoint = rightBottom.position;
        //}
        //else if (powerup == Powerup.BottomIsBlocked)
        //{
        //    leftNewborn.startPoint = leftTop.position;
        //    rightNewborn.startPoint = rightTop.position;
        //}
        //else //no powerup is already active.
        //{
        if (Random.Range(0f, 1f) < 0.5)
        {
            leftNewborn.startPoint = leftTop.position;
        }
        else
        {
            leftNewborn.startPoint = leftBottom.position;
        }
        if (Random.Range(0f, 1f) < 0.5)
        {
            rightNewborn.startPoint = rightTop.position;
        }
        else
        {
            rightNewborn.startPoint = rightBottom.position;
        }
        //}

        //numberObject_R = (GameObject)Instantiate(Random.Range(0f, 50f) > 25 ? Zero : One, intanciatePoint_R.position, new Quaternion());

        if (Random.Range(0f, 50f) > 25)
        {
            leftNewborn.GetComponent<SpriteRenderer>().color = chosenColor_L = Settings.color1;
            //chosenColor = "goToOrange";
        }
        else
        {
            leftNewborn.GetComponent<SpriteRenderer>().color = chosenColor_L = Settings.color2;
            //chosenColor = "goToBlue";
        }
        if (leftNewborn.startPoint == leftTop.position)
        {
            leftNewborn.FakeCreate(true);
            //print("TOP "+chosenColor);
            //laserTop_L.color = chosenColor_L;
            //laserTop.SetTrigger(chosenColor);
        }
        else
        {
            leftNewborn.FakeCreate(false);
            //print("BOTTOM " + chosenColor);
            //laserBottom_L.color = chosenColor_L;
            //laserBottom.SetTrigger(chosenColor);
        }

        if (Random.Range(0f, 50f) > 25)
        {
            rightNewborn.GetComponent<SpriteRenderer>().color = chosenColor_R = Settings.color1;
            //chosenColor = "goToOrange";
        }
        else
        {
            rightNewborn.GetComponent<SpriteRenderer>().color = chosenColor_R = Settings.color2;
            //chosenColor = "goToBlue";
        }
        if (rightNewborn.startPoint == rightTop.position)
        {
            rightNewborn.FakeCreate(true);
            //print("TOP "+chosenColor);
            laserTop_R.color = chosenColor_R;
            //laserTop.SetTrigger(chosenColor);
        }
        else
        {
            rightNewborn.FakeCreate(false);
            //print("BOTTOM " + chosenColor);
            laserBottom_R.color = chosenColor_R;
            //laserBottom.SetTrigger(chosenColor);
        }
        //Debug.Log("maxTimer changed to: " + maxTimer);
    }
    private void Spawn()
    {
        NumberObject_LR leftNewborn, rightNewborn;

        //instanciate two numbers, one for each side.
        //creating the left zo.
        int index = (int)Random.Range(0, instances.Length);//choose a random index of the pool.
        leftNewborn = instances[index];
        while (instances[index].isBeingUsed)//search for a free zo.
        {
            index = (int)Random.Range(0, instances.Length);
            leftNewborn = instances[index];
        }

        //creating the right zo.
        index = (int)Random.Range(0, instances.Length);
        rightNewborn = instances[index];

        while (instances[index].isBeingUsed)
        {
            index = (int)Random.Range(0, instances.Length);
            rightNewborn = instances[index];
        }


        //if (powerup == Powerup.TopIsBlocked)
        //{
        //    leftNewborn.startPoint = leftBottom.position;
        //    //intanciatePoint_L = Bottom_L;
        //    rightNewborn.startPoint = rightBottom.position;
        //}
        //else if (powerup == Powerup.BottomIsBlocked)
        //{
        //    leftNewborn.startPoint = leftTop.position;
        //    rightNewborn.startPoint = rightTop.position;
        //}
        //else //no powerup is already active.
        //{
        if (Random.Range(0f, 1f) < 0.5)
        {
            leftNewborn.startPoint = leftTop.position;
        }
        else
        {
            leftNewborn.startPoint = leftBottom.position;
        }
        if (Random.Range(0f, 1f) < 0.5)
        {
            rightNewborn.startPoint = rightTop.position;
        }
        else
        {
            rightNewborn.startPoint = rightBottom.position;
        }
        //}

        //numberObject_R = (GameObject)Instantiate(Random.Range(0f, 50f) > 25 ? Zero : One, intanciatePoint_R.position, new Quaternion());

        if (Random.Range(0f, 50f) > 25)
        {
            leftNewborn.GetComponent<SpriteRenderer>().color = chosenColor_L = Settings.color1;
            //chosenColor = "goToOrange";
        }
        else
        {
            leftNewborn.GetComponent<SpriteRenderer>().color = chosenColor_L = Settings.color2;
            //chosenColor = "goToBlue";
        }
        if (leftNewborn.startPoint == leftTop.position)
        {
            leftNewborn.FakeCreate(true);
            //print("TOP "+chosenColor);
            //laserTop_L.color = chosenColor_L;
            //laserTop.SetTrigger(chosenColor);
        }
        else
        {
            leftNewborn.FakeCreate(false);
            //print("BOTTOM " + chosenColor);
            //laserBottom_L.color = chosenColor_L;
            //laserBottom.SetTrigger(chosenColor);
        }

        if (Random.Range(0f, 50f) > 25)
        {
            rightNewborn.GetComponent<SpriteRenderer>().color = chosenColor_R = Settings.color1;
            //chosenColor = "goToOrange";
        }
        else
        {
            rightNewborn.GetComponent<SpriteRenderer>().color = chosenColor_R = Settings.color2;
            //chosenColor = "goToBlue";
        }
        if (rightNewborn.startPoint == rightTop.position)
        {
            rightNewborn.FakeCreate(true);
            //print("TOP "+chosenColor);
            laserTop_R.color = chosenColor_R;
            //laserTop.SetTrigger(chosenColor);
        }
        else
        {
            rightNewborn.FakeCreate(false);
            //print("BOTTOM " + chosenColor);
            laserBottom_R.color = chosenColor_R;
            //laserBottom.SetTrigger(chosenColor);
        }
        //Debug.Log("maxTimer changed to: " + maxTimer);
    }
    
    public void setPowerup(Powerup type, float second)//type lasts "second" seconds.
    {
        powerup = type;
        //Debug.Log("Powerup " + type.ToString() + " activated");
        switch (type)
        {
            case Powerup.TopIsBlocked:
                topSpawner_L.color = topSpawner_R.color = laserTop_R.color = laserTop_L.color = Color.black;
                break;
            case Powerup.BottomIsBlocked:
                bottomSpawner_L.color = laserBottom_R.color = bottomSpawner_R.color = laserBottom_L.color = Color.black;
                break;
            case Powerup.none:
                break;
            default:
                break;
        }
        Invoke("setpowerupNone", second);
    }
    void setpowerupNone()
    {
        if (powerup != Powerup.none)
        {
            //Debug.Log("Powerup " + powerup.ToString() + " deactivated");
        }
        powerup = Powerup.none;
        //Color chosenColor;
        //if (Random.Range(0f, 50f) > 25)
        //    {
        //    chosenColor = new Color(1f, (151f / 255f), 29f / 255f);
        //    }
        //else
        //    {
        //    chosenColor = new Color(29f / 255f, 210f / 255f, 1f);
        //    }
        //if (powerup == Powerup.TopIsBlocked)
        //    {
        //    laserTop.color = chosenColor;
        //    }
        //else if (powerup == Powerup.BottomIsBlocked)
        //    {
        //    laserBottom.color = chosenColor;
        //    }

        topSpawner_L.color = topSpawner_R.color = laserBottom_L.color = laserBottom_R.color = laserTop_L.color = laserTop_R.color = bottomSpawner_R.color = bottomSpawner_L.color = Color.white;
    }

    /// <summary>
    /// Increases Score one unit.
    /// </summary>
    /// <param name="i"> score is gained by a Zero or One</param>
    public void incScore(int i,bool isLeft)
    {
        Settings.hardcoreModeScore += 1;
        Settings.Instance.MoreSpeed(Settings.hardcoreModeScore);
        //if (powerup == Powerup.none)
        //{
        //    powerupScore++;
        //}
        scoreTextLeft.text = scoreTextRight.text = Settings.hardcoreModeScore.ToString();

        scoreShakeTweenLeft.Restart();//
        scoreShakeTweenRight.Restart();

        if (isLeft)
        {
            for (int j = 0; j < 4; j++)
            {
                capsuleLights[j].color = chosenColor_L;
                //capsuleLightsTween[j].Restart();//.From().OnComplete(() => capsuleLights[j].color = new Color(0,0,0,0))
            } 
            capsuleLightsLeft.SetTrigger("turnOn");
        }
        else
        {
            for (int j = 4; j < 8; j++)
            {
                capsuleLights[j].color = chosenColor_R;
                //capsuleLightsTween[j].Restart();//capsuleLights[j].DOColor(chosenColor_R, 0.2f).SetEase(Ease.OutBounce);//.SetEase(Ease.OutBounce)
            }
            capsuleLightsRight.SetTrigger("turnOn");
        }

        //pipeline 
        //Vector3 pipelinePos;
        //if (Random.Range(0f, 1f) > 0.5f)
        //{
        //    pipelinePos = pipelineRightSpawnPoint.position;
        //}
        //else
        //{
        //    pipelinePos = pipelineLeftSpawnPoint.position;
        //}
        //scoreAnimator.SetTrigger("scored");
        switch (i)
        {
            case 0:
                //scoreCirclePanelLeft.SetTrigger("rotate");
                break;
            case 1:
                //scoreCirclePanelLeft.SetTrigger("rotate");
                break;
            case 2:
                //scoreCirclePanelRight.SetTrigger("rotate");
                break;
            case 3:
                //scoreCirclePanelRight.SetTrigger("rotate");
                break;
            default:
                break;
        }
    }

    public void decScore()
    {
        isGameOver = true;
        if (PlayerPrefs.GetInt("hasReachedVideoLimit") == 0)
        {
            ShowVideoPanel();
        }
        else
        {
            //game over
            Settings.Instance.ShowGameOverPanel();
        }
        //score--;// or Game over
        //if (score<1)
        //{
        //    score = 0;
        //}
    }

    private void ShowVideoPanel()
    {
        Settings.Instance.StartVideoPanel();
        for (int i = 0; i < instancesList.Count; i++)
        {
            instancesList[i].FakeDestroy();
        }

        Settings.Instance.Pause();
        Resume(false);
    }

    public void gameOverMusicSet()
    {
        Camera.main.GetComponent<AudioSource>().volume = 0.6f;
        gameOverAnimator.SetTrigger("beat");
    }

    public void GameOver()
    {
        CancelInvoke();
        Settings.Instance.Pause();
        Resume(false);
        powerup = Powerup.none;
        //Invoke("gameOverMusicSet", 1.5f);

        //save and compare score to best score
        if (Settings.hardcoreModeScore > PlayerPrefs.GetInt("bestScoreHardcoe"))//new record
        {
            scoreComment.gameObject.SetActive(true);
            scoreComment.text = "New Record!";
            PlayerPrefs.SetInt("bestScoreHardcoe", Settings.hardcoreModeScore);
        }
        else
        {
            scoreComment.gameObject.SetActive(true);
            scoreComment.text = "Best Score: " + PlayerPrefs.GetInt("bestScoreHardcoe");
        }
        Status.IncreaseLosts();
        Status.IncreaseSumUpScore(Settings.hardcoreModeScore);
        //edit_3
        //NumberObject_LR[] remained_zero_ones = FindObjectsOfType<NumberObject_LR>();
        //foreach (NumberObject_LR item in remained_zero_ones)
        //    {
        //    Destroy(item.gameObject);
        //    }

        scoreTextLeft.text = scoreTextRight.text = Settings.hardcoreModeScore.ToString();
        //print("isFuseOpen: " + isFuse1_Open.ToString());
        gameOverScore.text = Settings.hardcoreModeScore.ToString();

        for (int i = 0; i < instances.Length; i++)
        {
            instances[i].FakeDestroy();
        }

        score_panel.SetActive(false);
        isGameOver = false;
    }
    
    public void Die_Btn()
    {
        videoPanel.SetActive(false);
        GameOver();
    }
    public void startBtn()
    {
        PlayerPrefs.SetInt("hasReachedVideoLimit", 0);
        Settings.mode = Settings.Modes.Hardcore;
        Resume(true);
        
        for (int i = 0; i < 8; i++)
        {
            capsuleLights[i].color = new Color(0, 0, 0, 0);
        }
        for (int i = 0; i < 8; i++)
        {
            platformLights[i].DOColor(Random.Range(0f, 1f) > 0.5f ? Settings.color1 : Settings.color2, 0.4f).SetLoops(-1, LoopType.Yoyo);
        }
        Settings.hardcoreModeScore = 0;
        powerupScore = 0;
        scoreToGetFuse = 2;
        scoreTextLeft.text = scoreTextRight.text = Settings.hardcoreModeScore.ToString();
        
        timer = 0;
        Settings.speed = 0.2f;
        score_panel.SetActive(true);
        Settings.Instance.GameStarted();
    }

    public void Resume(bool active)
    {
        pauseButton.SetActive(active);
        scoreDigitalLCDLeft.SetActive(active);
        scoreDigitalLCDRight.SetActive(active);
        //capsule_L.SetActive(active);
        //capsule_R.SetActive(active);
        scoreCirclePanelLeft.gameObject.SetActive(active);
        scoreCirclePanelRight.gameObject.SetActive(active);
    }
    //public void muteBtn()
    //{
    //    creator.isMute = !creator.isMute;
    //    Camera.main.GetComponent<AudioListener>().enabled = !creator.isMute;
    //    //AudioSource[] sounds = FindObjectsOfType<AudioSource>();
    //    //foreach (AudioSource item in audioSources)
    //    //    {
    //    //    item.mute = creator.isMute;
    //    //    }
    //    //if (isMute)
    //    //    {
    //    //    Camera.main.GetComponent<AudioSource>().Play();
    //    //    } 
    //}
    public void PauseTime()
    {
        Time.timeScale = 0;
    }
    public void changeUsername(InputField field)
    {
        if (field.text.Length < 1)
        {
            field.text = username;
        }
        else if (true)//check if name already exists.
        {
            username = field.text;
        }

    }
}
