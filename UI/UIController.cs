using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public enum UIDialogType
{
    Information, 
    Exclamation, 
    Error
}
public class UIController : MonoBehaviour
{
    public static UIController instance;
    public GameObject MainMenu;
    
    // fields

    [HeaderAttribute("UI Generic Panels")]
    [SpaceAttribute(5f)]
    public GameObject genericYesNoModal;
    public Button genericYesBtn;
    public Button genericNoBtn;

    [SpaceAttribute(5f)]
    public GameObject genericDialogModal;
    public Image informationImg, exclamationImg, errorImg;
    public Button genericOkBtn;





    [HeaderAttribute("UI Effects vars")]
    public float fadeFactor;
    public float waitFactor;
    [SerializeField]
    [RangeAttribute(1f, 200f)]
    private float progressBarSpeed;


    // methods
    void Awake()
    {
        // #revision: the next line is just test-code
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void GoFrom_To(GameObject from, GameObject nextPanel)
    {
        if (nextPanel != null && from != null)
        {
            from.SetActive(false);
            nextPanel.SetActive(true);
        }
    }
    public void IfClick_GoTo(Button button, UnityAction someEvent)
    {
        try
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(someEvent);
        }
        catch (System.Exception err)
        {
            ThrowDialogBox(err.Message, CloseDialogBox, UIDialogType.Error);
            throw;
        }
        // debug
        // print(button.name);
    }
    public void Enable_DisableUI(GameObject enableThis, params GameObject[] disableThese)
    {
        try
        {
            enableThis.SetActive(true);
            if (disableThese == null) return;

            foreach (GameObject obj in disableThese)
            {
                obj.SetActive(false);
            }
        }
        catch (System.Exception err)
        {
            ThrowDialogBox("V_UIController: Enable_DisableUI(): " + err.Message, CloseDialogBox, UIDialogType.Error);
            throw;
        }
    }
    public void Disable_EnableUI(GameObject disableThis, params GameObject[] enableThese)
    {
        try
        {
            if (disableThis == null)
            {
                ThrowDialogBox("V_UIController: Disable_EnableUI(): item to disable is null, dude", CloseDialogBox, UIDialogType.Error);
            }
            disableThis.SetActive(false);
            if (enableThese == null) return;

            foreach (GameObject obj in enableThese)
            {
                obj.SetActive(true);
            }
        }
        catch (System.Exception err)
        {
            ThrowDialogBox("V_UIController: Disable_EnableUI(): " + err.Message, CloseDialogBox, UIDialogType.Error);
            throw;
        }
    }
    public void FadeIn(Image image)
    {
        StartCoroutine(_FadeIn(image));
    }

    private IEnumerator _FadeIn(Image image)
    {
        CanvasRenderer tmpRndr = image.GetComponent<CanvasRenderer>();
        if (tmpRndr == null)
        {
            print("V_UIController: FadeIn: Cannot get the CanvasRenderer on " + image.name);
        }
        else
        {
            tmpRndr.SetAlpha(0.1f);
            image.CrossFadeAlpha(120f, fadeFactor, true);
            yield return new WaitForSeconds(fadeFactor);
        }
    }
    public void FadeOut(Image image)
    {
        StartCoroutine(_FadeOut(image));
    }
    private IEnumerator _FadeOut(Image image)
    {
        CanvasRenderer tmpRndr = image.GetComponent<CanvasRenderer>();
        if (tmpRndr == null)
        {
            print("V_UIController: FadeOut: cannot get te CanvasRenderer on " + image.name);
        }
        else
        {
            tmpRndr.SetAlpha(120f);
            image.CrossFadeAlpha(0.1f, fadeFactor, true);
            yield return new WaitForSeconds(fadeFactor);
        }
    }
    // Returning Gamemodes: 2 Overloads
    public void AskYesNoQuestion(string question, UnityAction yesAction, UnityAction noAction)
    {
        genericYesBtn.onClick.RemoveAllListeners();
        genericYesBtn.onClick.AddListener(yesAction);

        genericNoBtn.onClick.RemoveAllListeners();
        genericNoBtn.onClick.AddListener(noAction);

        genericYesNoModal.GetComponentInChildren<Text>().text = question;
        Enable_DisableUI(genericYesNoModal);
    }
    public void CloseYesNoQ()
    {
        Disable_EnableUI(genericYesNoModal);
    }

    public void ThrowDialogBox(string err, UnityAction okAction, UIDialogType dialogType)
    {
        genericOkBtn.onClick.RemoveAllListeners();
        genericOkBtn.onClick.AddListener(okAction);

        genericDialogModal.GetComponentInChildren<Text>().text = err;
        switch (dialogType)
        {
            case UIDialogType.Information:
                informationImg.enabled = true;
                exclamationImg.enabled = false;
                errorImg.enabled = false;
                break;
            case UIDialogType.Exclamation:
                informationImg.enabled = false;
                exclamationImg.enabled = true;
                errorImg.enabled = false;
                break;
            case UIDialogType.Error:
                informationImg.enabled = false;
                exclamationImg.enabled = false;
                errorImg.enabled = true;
                break;
            default:
                break;
        }
        Enable_DisableUI(genericDialogModal);
    }
    public void CloseDialogBox()
    {
        Disable_EnableUI(genericDialogModal);
    }

    public IEnumerator FillBar(Image imageAsMask, float finalValueInPercent, float backgroundBarWidth, float initialMaskWidth = 0)
    {
        // return the quickest way possible!!!
        yield return null;

        // this does the image bar effect this way:
        // 0. it enables the image! we may have needed to disable the image before, and if it is enabled already, then: no harm, no foul!
        // 1. we have an image as the mask
        // 2. this mask has a child which is the background image of the bar
        // 3. we make the mask bigger and bigger, so each time a little more of the background is shown, indicating progress in the bar
        float singleBarWidth = backgroundBarWidth / 100f;
        float finaleValueInPixel = finalValueInPercent * singleBarWidth;
        singleBarWidth = (finaleValueInPixel > initialMaskWidth) ? singleBarWidth : -singleBarWidth;
        float fixedMaskHeight = imageAsMask.rectTransform.rect.height;
        int everyOtherTime = 5; // try to make a CPU-friendly coroutine!
        int time = 0;

        imageAsMask.gameObject.SetActive(true);

        // zeroing the mask so we get a zero bar waiting to be filled
        imageAsMask.rectTransform.sizeDelta = new Vector2(0, 0);
        while (true)
        {
            time++; // increase no matter what
            if (time % everyOtherTime != 0)
            {
                continue;
            }
            initialMaskWidth += singleBarWidth * Time.deltaTime * progressBarSpeed;
            imageAsMask.rectTransform.sizeDelta = new Vector2(initialMaskWidth, fixedMaskHeight);
            // some visual Effects via coroutines!!!
            if ((initialMaskWidth > finaleValueInPixel) && singleBarWidth > 0 || ((initialMaskWidth < finaleValueInPixel) && singleBarWidth < 0))
            {
                yield break;
            }
        }
    }
}
