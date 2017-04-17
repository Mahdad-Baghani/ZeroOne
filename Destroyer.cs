using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {
    //public string targetTag;

    void OnTriggerEnter2D(Collider2D other)
    {
        //print("collide with" + other.name);
        Destroy(other.gameObject);
    }
}
