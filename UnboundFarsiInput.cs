using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnboundFarsiInput : MonoBehaviour
{
    InputField inputField;
    Text coverText;
	// Use this for initialization
	void Start ()
    {
        inputField = GetComponent<InputField>();
        coverText = transform.FindChild("CoverText").gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ConvertToFarsi()
    {
        inputField.text.faConvert();
        coverText.text = "" + coverText.text.faConvert();
    }
}
