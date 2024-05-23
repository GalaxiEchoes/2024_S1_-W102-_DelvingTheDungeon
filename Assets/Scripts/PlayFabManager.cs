using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayFabManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public CanvasManager canvasManager;
    public TMP_InputField emailInput; 
    public TMP_InputField passwordInput;

    public void RegisterButton()
    {
        //Checks if password is too short for playfab
        if(passwordInput.text.Length < 6)
        {
            text.text = "Password too short!";
            return;
        }

        var request = new RegisterPlayFabUserRequest {
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        text.text = "Registered and logged in!";
        canvasManager.SetLoggedIn(true);
        canvasManager.OnMenuButtonClick();
    }

    public void LoginButton()
    {
            var request = new LoginWithEmailAddressRequest
            {
                Email = emailInput.text,
                Password = passwordInput.text,
            };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        text.text = "Logged in!";
        Debug.Log("Successful login/account create!");
        canvasManager.SetLoggedIn(true);
        canvasManager.OnMenuButtonClick();
        //get info
    }

    void OnError(PlayFabError error)
    {
        text.text = error.ErrorMessage;
        Debug.Log("Error while logging in/creating account!");
        Debug.Log(error.GenerateErrorReport());
    }

    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest {
            Email = emailInput.text,
            TitleId = "91DB4"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        text.text = "Password reset mail sent!";
    }

    /*public void SaveGameData()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                // here
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public void GetGameData()
    {

    }*/


    // Start is called before the first frame update
    void Start()
    {
        //Login(); 
    }

    /*void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);

    }*/

    

}
/*
 public void SaveAppearance(){
    var request = new UpdateUserDataRequest {
    Data = new Dictionary<string, string> {
        {"Hat", characterEditor.Hat},
    };
    PlayFabCLientAPI.UpdateUserData(request, OnDataSend, OnError);
}

public void GetAppearance(){
    PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);

}

void OnDataRecieved(GetUserDataResult result){
    Debug.Log("Recieved user data");
}

//data saved as key value pairs like result.Data["Hat"].Value
 */