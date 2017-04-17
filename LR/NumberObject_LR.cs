using DG.Tweening;
using UnityEngine;
using System.Collections;

public class NumberObject_LR : MonoBehaviour
{
    public float movement;
    Rigidbody2D rigidBody2D;
    public bool isBeingUsed;
    public Vector3 startPoint;
    public creator_LR creatorLR;
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        transform.DORotate(new Vector3(0, 0, 180), 1.2f).SetLoops(-1).SetEase(Ease.Linear).SetId("rotatingNumbers");
        //print("movement= " + movement);
        //print("speed= " + creator.speed);
    }


    //void Update()
    //{
    //    //transform.position = new Vector3(transform.position.x, transform.position.y + movement, transform.position.z);
    //}
    public void FakeDestroy()
    {
        isBeingUsed = false;
        rigidBody2D.isKinematic = true;
        transform.position = new Vector3(-4.19f, 0f, -11);// z being -11 vanishes the object from camera's view because of it's depth.
    }
    public void FakeCreate(bool isTop)
    {
        //print("Spawn: " + name);
        isBeingUsed = true;
        transform.position = startPoint;
        rigidBody2D.isKinematic = false;
        if (isTop)
        {
            //print(tag = "Top");
            rigidBody2D.gravityScale = Settings.speed;
        }
        else
        {
            //print(tag = "Bottom");
            rigidBody2D.gravityScale = -Settings.speed;
        }
        
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        //print("hit: " + other.name);
        if (other != null)
        {
            if ((name.Contains("Zero") && other.name.Contains("Zero")))
            {
                if (name.Contains("LEFT"))
                {
                    creatorLR.incScore(0,true); 
                }
                else
                {
                    creatorLR.incScore(0, false); 
                }
                FakeDestroy();
                //Destroy(this.gameObject);
                //print("hit zero");
            }
            else if (name.Contains("One") && other.name.Contains("One"))
            {
                if (name.Contains("RIGHT"))
                {
                    creatorLR.incScore(1, false);
                }
                else
                {
                    creatorLR.incScore(1, true);
                }
                FakeDestroy();
                //Destroy(this.gameObject);
                //print("hit one");
                //one_SFX.GetComponent<AudioSource>().Play();
            }
            else if (other.name.Contains("Obstacle"))
            {

            }
            else
            {
                FakeDestroy();
                //Destroy(this.gameObject);
                creatorLR.decScore();
            } 
        }

    }
}
