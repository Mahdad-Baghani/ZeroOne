using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class RadioButtonMaker : MonoBehaviour {
    public List<GameObject> selectionBoxes;
    public Button[] allButtons;
	// Use this for initialization
	void Start () {
        //allButtons = GetComponentsInChildren<Button>();
        //for (int i = 0; i < allButtons.Length; i++)
        //{
        //    print("Current i: " + i);
        //    allButtons[i].onClick.AddListener(delegate { Clicked(i-1); });
        //    print("Listener " + i + " added.");
        //}
        allButtons[0].onClick.AddListener(delegate { Clicked(0); });
        allButtons[1].onClick.AddListener(delegate { Clicked(1); });
	}
	
    public void Clicked(int index)
    {
        print("index is: "+ index);
        for (int i = 0; i < allButtons.Length; i++)
        {
            //Unselect(allButtons[i].gameObject);
            Unselect(i);
        }
        //allButtons[index].gameObject
        Select(index);
    }
    void Unselect(GameObject btn)
    {
        //btn.SetActive(false);
        //btn.transform.DOLocalRotate(new Vector3(0, 0, 7), 0.1f).SetLoops(-1,LoopType.Yoyo);
    }
    void Select(GameObject btn)
    {
        //btn.transform.DOKill();
        //btn.transform.localEulerAngles = Vector3.zero;
    }
    void Unselect(int index)
    {
        selectionBoxes[index].SetActive(false);
        //btn.SetActive(false);
        //btn.transform.DOLocalRotate(new Vector3(0, 0, 7), 0.1f).SetLoops(-1,LoopType.Yoyo);
    }
    void Select(int index)
    {
        selectionBoxes[index].SetActive(true);
        //btn.transform.DOKill();
        //btn.transform.localEulerAngles = Vector3.zero;
    }
}
