using UnityEngine;
using Backtory.Core.Public;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;

public enum Gender
{
    female = 0,
    male = 1,
    none = -1
}
public class RegistrationMenu : MonoBehaviour, IRegistrationObserver
{
    // UI fields
    [SerializeField]
    private Button RegisterBtn, SelectMaleGenderBtn, SelectFemaleGenderBtn;
    [SerializeField]
    private InputField nameTxt, emailTxt;
    [SerializeField]
    private Text PersianNameTxt;

    private Gender userGender = Gender.none;
    [SerializeField] private Image nameErrorIndicator, emailErrorIndicator, genderErrorIndicator;

    #region UI funx
    private void Awake()
    {
        BacktoryBackend.instance.InitializeBacktoryRegistrationObserver(this as IRegistrationObserver);
        //PlayerPrefs.DeleteAll(); // just for the sake of argument!!! and ofcourse debug, delete it in production!!
        this.gameObject.SetActive(false);
        CheckIfUserIsRegistered();
        HookupPersianToInputFields();
        SetUpButtons();
    }

    private void SetUpButtons()
    {
        RegisterBtn.onClick.AddListener(() => RegisterUser());
        SelectMaleGenderBtn.onClick.AddListener(() => userGender = Gender.male);
        SelectFemaleGenderBtn.onClick.AddListener(() => userGender = Gender.female);
    }

    private void HookupPersianToInputFields()
    {
        ConvertToFarsi(nameTxt, (str) => PersianNameTxt.text = str.faConvert());
    }
    private void ConvertToFarsi(InputField inpField, UnityAction<string> onValueChangedEvent)
    {
        if (inpField != null)
        {
            inpField.onValueChanged.RemoveAllListeners();
            inpField.onValueChanged.AddListener(onValueChangedEvent);
        }
    }
    private void LoadRegistration()
    {
        DollyTheMenuIn();
    }
    private void DollyTheMenuIn()
    {
        gameObject.SetActive(true);
        transform.DOLocalMoveX(10, 0.6f).SetEase(Ease.InBack);
    }
    private void HideRegistrationUI()
    {
        transform.DOLocalMoveX(-777, 0.6f).SetEase(Ease.OutBack).OnComplete(() => { print("asd"); gameObject.SetActive(false); });
        this.gameObject.SetActive(false);
    }

    private void ShowErrorIndicator(Image errorInd)
    {
        errorInd.gameObject.SetActive(true);
        errorInd.DOFade(1, 0.2f).SetLoops(3).OnComplete(() => errorInd.DOFade(0,0.2f));
        //errorInd.CrossFadeAlpha(255f, 1f, true);
        //yield return new WaitForSeconds(1f);
        //errorInd.CrossFadeAlpha(0, 1f, true);
        //yield return new WaitForSeconds(1f);
    }

    #endregion

    #region network login & registration stuff
    private void CheckIfUserIsRegistered()
    {
        try
        {
            string username = PlayerPrefs.GetString("bitwarUser", "UNDEFINED");
            //Debug.Log(username);
            if (username == null || username == "UNDEFINED")
            {
                LoadRegistration();
            }
            else
            {
                Login_n_SkipRegistration();
            }

        }
        catch (Exception)
        {
            // at this stage, we know that the problem has occured because of inablity to read playerPrefs
            // , therefore player is not registered or registration info is missing
            LoadRegistration();
        }

    }

    private void Login_n_SkipRegistration()
    {
        string user = PlayerPrefs.GetString("bitwarUser");
        string pass = PlayerPrefs.GetString("bitwarPass");
        #region Debug n stuff
        //Debug.Log("username: " + user + " pass : " + pass + " with score of : " + highScore.ToString());
        #endregion
        if (user == "" || user == null || pass == "" || pass == null)
        {
            // if so, we now there's been a corruption in saved data, therefore we have to do it again
            LoadRegistration();
            return;
        }
        BacktoryBackend.instance.Login(user, pass);
        // callback comes in a separate func... heyfesh
    }
    private void RegisterUser()
    {
        if (nameTxt.text == "")
        {
            //throw new System.Exception("please enter a valid user name");
            ShowErrorIndicator(nameErrorIndicator);
        }
        if (emailTxt.text == "")
        {
            //throw new System.Exception("please enter an email");
            ShowErrorIndicator(emailErrorIndicator);
        }
        if (!TestEmail.instance.IsEmail(emailTxt.text))
        {
            //throw new System.Exception("please enter a valid email addres");
            ShowErrorIndicator(emailErrorIndicator);
        }
        if (userGender == Gender.none)
        {
            //throw new System.Exception("Please select your gender");
            ShowErrorIndicator(genderErrorIndicator);
        }
        else
        {
            // callback comes in a separate func... heyfesh
            BacktoryBackend.instance.RegisterUser(userGender.ToString(), nameTxt.text, emailTxt.text);
        }

    }

    private void SavePlayer(string userName, string password)
    {
        if (userName == null && password == null)
        {
            throw new System.Exception("username is not valid");
        }
        PlayerPrefs.SetString("bitwarUser", userName);
        PlayerPrefs.SetString("bitwarPass", password);
        PlayerPrefs.SetInt("bitwayHighscore", 0);
    }

    // # revision
    public void SkipLogin()
    {
        UIController.instance.AskYesNoQuestion("you won't be able to compete with others\n" +
            "and your high scores won't be measured\nDo you want to continue?",
            (/*if answered yes*/) =>
            {
                // #revision: go on with guest login
                UIController.instance.CloseYesNoQ();
                BacktoryBackend.instance.LoginAsGuest();
                return;
            },
            (/*if answered no*/) =>
            {
                UIController.instance.CloseYesNoQ();
                return;
            });

    }

    public void RecieveRegistrationStatus(IBacktoryResponse<BacktoryUser> registrationResponse)
    {
        if ((BacktoryHttpStatusCode)registrationResponse.Code == BacktoryHttpStatusCode.Created)
        {
            //Debug.Log("hello " + registrationResponse.Body.Username.faConvert() + " !\nGo and fight; for glory... for honor.");
            BacktoryBackend.instance.Login(nameTxt.text, emailTxt.text);
            SavePlayer(nameTxt.text, emailTxt.text);
            
            HideRegistrationUI();
        }
        else if ((BacktoryHttpStatusCode)registrationResponse.Code == BacktoryHttpStatusCode.Conflict)
        {
            // # revision
            //UIController.instance.ThrowDialogBox("There already exists a user with the name " + registrationResponse.Body.Username + "\nTry a different Username", UIController.instance.CloseDialogBox, UIDialogType.Error);
        }
        else if ((BacktoryHttpStatusCode)registrationResponse.Code == BacktoryHttpStatusCode.InternetAccessProblem)
        {
            // # revision
            //UIController.instance.ThrowDialogBox("Check your intenet connection", UIController.instance.CloseDialogBox, UIDialogType.Error);
        }
    }
    public void RecieveLoginStatus(IBacktoryResponse<object> resp)
    {
        //  so backtory returns null as of now for resp
        // donno why
        // go to menu
        // we kinda have to send an event just as a workaround for this wierd functionality of backend provider
        int normalSc = PlayerPrefs.GetInt("NormalScore");
        int hardcoreSc = PlayerPrefs.GetInt("HardScore");

        BacktoryBackend.instance.SendPlayerStats(normalSc, hardcoreSc);

    }

    #endregion
}
