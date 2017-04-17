using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LaserChargerSystem : MonoBehaviour
{
    bool isFacingCharger, isLaserPoweredOn;
    public GameObject charger, laser;
    public Transform topPosition, bottomPosition;
    public float chargeValue, maxChargeValue, chargeIncreaseStep, chargeDecreaseStep;
    public float timer, maxTimer, defaultMaxTimer;
    public Text chargerText;
    public Image image;
    public AudioClip chargingSound, dechargingSound;
    AudioSource audioSource;
    Color redAlpha;
    public float a, b, c;
    // Use this for initialization
    void Start()
    {
        redAlpha = Color.red;
        redAlpha.a = 0;
        audioSource = GetComponent<AudioSource>();
        isFacingCharger = false;
        defaultMaxTimer = 0.5f;
        StopLaser();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLaserPoweredOn)
        {
            timer += Time.deltaTime;
            if (timer > maxTimer)// every timer seconds do this:
            {
                timer = 0;
                
                if (isFacingCharger)
                {
                    if (maxTimer > 0.2f)
                    {
                        maxTimer -= 0.01f;
                    }// a * Mathf.Pow(chargeValue, 2) + b * chargeValue + c;
                    chargeValue += chargeIncreaseStep;
                    //play sound;
                    //audioSource.clip = chargingSound;
                    //audioSource.Play();
                }
                else
                {
                    if (maxTimer < 0.5f)
                    {
                        maxTimer += 0.01f;
                    }// a * Mathf.Pow(chargeValue, 2) + b * chargeValue + c;
                    chargeValue += chargeDecreaseStep;
                }



            }
            chargeValue = Mathf.Clamp(chargeValue, 0, maxChargeValue);
            Color clr = image.color;
            clr.a = chargeValue/100;
            image.color = clr;
            chargerText.text = chargeValue.ToString();

        }
        #region DEBUG

        if (Input.GetKeyUp(KeyCode.Z))
        {
            if (isLaserPoweredOn)
            {
                StopLaser();
            }
            else
            {
                StartLaser();
            }
        }
        #endregion
    }
    /// <summary>
    /// creates a laser at random top or bottom position.
    /// </summary>
    public void StartLaser()
    {
        Time.timeScale = 0.7f;
        isLaserPoweredOn = true;
        isFacingCharger = false;

        

        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;

        chargeValue = 0;
        charger.SetActive(true);
        //play start animation of the charger.
        //play start animation of the Laser.

        if (Random.Range(0f, 1f) < 0.5f)
        {
            //Top
            //set the position to the top.
            transform.position = topPosition.position;
            //rotate it so it faces downside.
            transform.rotation = new Quaternion(0, 0, 0, 1);
        }
        else
        {
            //Bottom

            //set the position to the bottom.
            transform.position = bottomPosition.position;
            //rotate it so it faces upside.
            transform.rotation = new Quaternion(0, 0, -180, 1);
        }
    }

    public void StopLaser()
    {
        Time.timeScale = 1;
        isLaserPoweredOn = false;
        isFacingCharger = false;
        charger.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        image.color = redAlpha;
        maxTimer = defaultMaxTimer;
        timer = 0;
        //play stop animation of the charger.
        //play sop animation of the Laser.
    }


    public void OnTriggerEnter2D()//no need to identify the collider, it only collides layer: prizeSystem.
    {
        isFacingCharger = true;
    }

    public void OnTriggerExit2D()//no need to identify the collider, it only collides layer: prizeSystem.
    {
        isFacingCharger = false;
    }
}
