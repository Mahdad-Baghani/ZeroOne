using DG.Tweening;
using UnityEngine;
using System.Collections;

public class centerObject : MonoBehaviour {
    Animator capsuleAnimator;
    public int animFactor;
    public Transform vacume;
    Tween vacumePunch;
	void Start () {
        vacumePunch = vacume.DOPunchPosition(new Vector3(0, -1 * animFactor, 0), 0.3f).SetAutoKill(false);
        //vacumePunch.Rewind();
	}
    void Update()
    {
        if (Input.GetKey(KeyCode.Insert)) 
        {
            vacumePunch.Restart();
            //vacume.DOPunchPosition(new Vector3(0, 1, 0), 0.3f)
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        //print("vacume should punch now.");
        vacumePunch.Restart();
        //transform.DOPunchPosition(new Vector3(0, 1 , 0), 0.3f);
        //use DOtween to animate vacume
        Settings.Instance.PlaySound(Settings.SoundTypes.SuckOne);
    }
}
