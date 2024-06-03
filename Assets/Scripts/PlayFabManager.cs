using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using static PersistenceManager;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

public class PlayFabManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public CanvasManager canvasManager;
    public TMP_InputField emailInput; 
    public TMP_InputField passwordInput;

    PersistenceManager manager;
    int bossLevel = 5;

    void Awake()
    {
        manager = GetComponent<PersistenceManager>();
        if (canvasManager != null)
        {
            canvasManager.SetLoggedIn(IsLoggedIn());
        }
    }

    public bool IsLoggedIn()
    {
        return PlayFabClientAPI.IsClientLoggedIn();
    }

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

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
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

    private void OnLoginSuccess(LoginResult result)
    {
        text.text = "Logged in!";
        Debug.Log("Successful login/account create!");
        canvasManager.SetLoggedIn(true);
        canvasManager.OnMenuButtonClick();
        GetGameData();
    }

    private void OnError(PlayFabError error)
    {
        if (text != null)
        {
            text.text = error.ErrorMessage;
        }
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

    private void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        text.text = "Password reset mail sent!";
    }

    public void SaveData()
    {
        SavePermanentData();
        SaveWorldData();
        SaveGameData();
    }

    public void SavePermanentData()
    {
        string dataJson = JsonUtility.ToJson(manager.permanentGameData);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"permanent_data", dataJson}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public void SaveGameData()
    {
        string dataJson = JsonUtility.ToJson(manager.gameData);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"game_data", dataJson}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public void SaveWorldData()
    {
        for (int i = 0; i <= bossLevel; i++)
        {
            if (manager.LoadWorldState(i))
            {
                string dataJson = JsonUtility.ToJson(manager.worldState);

                var request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>
                    {
                        {i + "world_state", dataJson}
                    }
                };
                PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
            }
        }
    }

    private void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Data Sent");
    }

    public void GetGameData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
    }

    private void OnDataRecieved(GetUserDataResult result)
    {
        if(result.Data != null)
        {
            if (result.Data.ContainsKey("permanent_data"))
            {
                manager.permanentGameData = JsonUtility.FromJson<PermanentGameData>(result.Data["permanent_data"].Value);
                manager.SavePermanentData();
            }

            if (result.Data.ContainsKey("game_data"))
            {
                manager.gameData = JsonUtility.FromJson<GameData>(result.Data["game_data"].Value);
                manager.SaveGameData();
            }

            for (int i = 0; i <= bossLevel; i++)
            {
                if (result.Data.ContainsKey(i + "world_state"))
                {
                    manager.worldState = JsonUtility.FromJson<WorldState>(result.Data[i + "world_state"].Value);
                    manager.SaveWorldState(i);
                }
            }
            
        }
    }

    //Deletes all data
    public void StartNewGame()
    {
        List<string> keysToDelete = new List<string>();

        for(int i = 0; i <= bossLevel; i++)
        {
            keysToDelete.Add(i + "world_state");
        }
        keysToDelete.Add("game_data");
        keysToDelete.Add("permanent_data");

        UpdateUserDataRequest request = new UpdateUserDataRequest
        {
            Data = null,
            KeysToRemove = keysToDelete
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    //Deletes world and level persistent data but not permanent data
    public void StartNewRun()
    {
        List<string> keysToDelete = new List<string>();

        for (int i = 0; i <= bossLevel; i++)
        {
            keysToDelete.Add(i + "world_state");
        }
        keysToDelete.Add("game_data");

        UpdateUserDataRequest request = new UpdateUserDataRequest
        {
            Data = null,
            KeysToRemove = keysToDelete
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }
}