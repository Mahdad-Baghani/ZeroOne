using UnityEngine;
using Backtory.Core.Public;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;
using DG.Tweening;

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
        RegisterBtn.onClick.AddListener(() => StartCoroutine(RegisterUser()));
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
    private void LoadRegistration()
    {
        this.gameObject.SetActive(true);
        DollyTheMenuIn();
    }
    private void Login_n_SkipRegistration()
    {
        string user = PlayerPrefs.GetString("bitwarUser");
        string pass = PlayerPrefs.GetString("bitwarPass");
        int highScore = PlayerPrefs.GetInt("bitwarHighscore");
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
        BacktoryBackend.instance.SendPlayerStats(highScore, 0);
        //GoToMainMenu();
    }
    private void DollyTheMenuIn()
    {
        gameObject.SetActive(true);
        transform.DOLocalMoveX(10, 0.6f).SetEase(Ease.InBack);
    }

    private void HideRegistrationUI()
    {
        transform.DOLocalMoveX(-777, 0.6f).SetEase(Ease.OutBack).OnComplete(() => { print("asd"); gameObject.SetActive(false); });
    }

    private IEnumerator RegisterUser()
    {
        bool errorAccoured = false;
        if (nameTxt.text == "")
        {
            //throw new System.Exception("please enter a valid user name");
            ShowErrorIndicator(nameErrorIndicator);
            errorAccoured = true;
        }
        if (emailTxt.text == "")
        {
            //throw new System.Exception("please enter an email");
            ShowErrorIndicator(emailErrorIndicator);
            errorAccoured = true;
        }
        if (!TestEmail.instance.IsEmail(emailTxt.text))
        {
            //throw new System.Exception("please enter a valid email addres");
            ShowErrorIndicator(emailErrorIndicator);
            errorAccoured = true;
        }
        if (userGender == Gender.none)
        {
            //throw new System.Exception("Please select your gender");
            ShowErrorIndicator(genderErrorIndicator);
            errorAccoured = true;
            
        }
        if (errorAccoured)
        {
            yield break;
        }
        else
        {
            // network.Register(nameTxt.text, emailTxt.text);
            //if successful
            //string username = PlayerPrefs.GetString("bitwarUser");
            BacktoryBackend.instance.RegisterUser(userGender.ToString(), nameTxt.text, emailTxt.text);
            yield return new WaitForSeconds(5f);//WHY
            BacktoryBackend.instance.Login(nameTxt.text, emailTxt.text);
            yield return new WaitForSeconds(5f);//WHY
            BacktoryBackend.instance.SendPlayerStats(77, 0);
            SavePlayer(nameTxt.text, emailTxt.text);
            //GoToMainMenu();
        }

    }
    private void GoToMainMenu()
    {
        this.gameObject.SetActive(false);
        //UIController.instance.MainMenu.SetActive(true);
        //SceneManager.LoadScene(1); // main game scene
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
            //UIController.instance.ThrowDialogBox("hello " + registrationResponse.Body.Username.faConvert() + " !\nGo and fight; for glory... for honor.", UIController.instance.CloseDialogBox, UIDialogType.Information);
            Debug.Log("hello " + registrationResponse.Body.Username.faConvert() + " !\nGo and fight; for glory... for honor.");
            
            HideRegistrationUI();
        }
        else if ((BacktoryHttpStatusCode)registrationResponse.Code == BacktoryHttpStatusCode.Conflict)
        {
            //UIController.instance.ThrowDialogBox("There already exists a user with the name " + registrationResponse.Body.Username + "\nTry a different Username", UIController.instance.CloseDialogBox, UIDialogType.Error);
            Debug.Log("There already exists a user with the name " + registrationResponse.Body.Username + "\nTry a different Username");
        }
        else if ((BacktoryHttpStatusCode)registrationResponse.Code == BacktoryHttpStatusCode.InternetAccessProblem)
        {
            //UIController.instance.ThrowDialogBox("Check your intenet connection", UIController.instance.CloseDialogBox, UIDialogType.Error);
            Debug.Log("Check your intenet connection");
        }
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
}
