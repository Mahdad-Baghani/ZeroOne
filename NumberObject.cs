using DG.Tweening;
using UnityEngine;
using System.Collections;

public class NumberObject : MonoBehaviour
{
    public float movement;
    Rigidbody2D rigidBody2D;
    public bool isBeingUsed;
    public Vector3 startPoint;
    public creator creator;
    public float rotationTime = 1;
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        transform.DORotate(new Vector3(0, 0, 180), rotationTime).SetLoops(-1).SetEase(Ease.Linear).SetId("rotatingNumbers");
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
        gameObject.SetActive(false);
        //transform.position = new Vector3(0f, 0f, -11);// z being -11 vanishes the object from camera's view because of it's depth.
    }
    public void FakeCreate(bool isTop)
    {
        gameObject.SetActive(true);
        isBeingUsed = true;
        transform.position = startPoint;
        rigidBody2D.isKinematic = false;
        if (isTop)
        {
            tag = "Top";
            rigidBody2D.gravityScale = Settings.speed;
        }
        else
        {
            tag = "Bottom";
            rigidBody2D.gravityScale = -Settings.speed;
        }
        //print("ZO: tag, speed, start:" + tag + "," + creator.speed + "," + startPoint.ToString());
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        //print("hit: " + other.name);
        if (other != null)
        {
            //print("triggered: " + name + " with " + other.name);
            if ((name.Contains(other.name)) || (other.name.Contains(name)))
            {
                if (other.name.Contains("Zero"))
                {
                    creator.IncreseScore(0);
                }
                else
                {
                    creator.IncreseScore(1);
                }
                FakeDestroy();
                //Destroy(this.gameObject);
                //print("hit zero");
            }
            else if (other.name.Contains("Obstacle"))
            {

            }
            else
            {
                FakeDestroy();
                //Destroy(this.gameObject);
                creator.DecScore();
            }
        }

    }
}
