using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject Register;
    public GameObject Login;
    public GameObject MainMenu;
    public GameObject settingsManager;
    public GameObject settingsMenu;

    public void SetLoggedIn(bool value)
    {
        PlayerPrefs.SetInt("IsLoggedIn", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsLoggedIn()
    {
        return PlayerPrefs.GetInt("IsLoggedIn", 0) == 1;
    }

    public void ClearLoginStatus()
    {
        PlayerPrefs.DeleteKey("IsLoggedIn");
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        ClearLoginStatus();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
        {
            ClearLoginStatus();
        }
    }

    public void OnLoginButtonClick()
    {
        SwitchToLogin();
    } 

    public void OnRegisterButtonClick()
    {
        SwitchToRegister();
    }

    public void OnMenuButtonClick()
    {
        SwitchToMain();
    }

    public void OnSettingsButtonClick()
    {
        SwitchToSettingsScreen();
    }

    public void OnBackButtonClick()
    {
        SwitchToMain();
    }

    private void SwitchToSettingsScreen()
    {
        settingsMenu.SetActive(true);
        settingsManager.SetActive(true);
        Login.SetActive(false);
        Register.SetActive(false);
        MainMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SwitchToLogin()
    {
        settingsMenu.SetActive(false);
        settingsManager.SetActive(false);
        Login.SetActive(true);
        Register.SetActive(false);
        MainMenu.SetActive(false);
    }

    private void SwitchToRegister()
    {
        settingsMenu.SetActive(false);
        settingsManager.SetActive(false);
        Login.SetActive(false);
        Register.SetActive(true);
        MainMenu.SetActive(false);
    }

    private void SwitchToMain()
    {
        settingsMenu.SetActive(false);
        settingsManager.SetActive(false);
        Login.SetActive(false);
        Register.SetActive(false);
        MainMenu.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(IsLoggedIn())
        {
            settingsMenu.SetActive(false);
            settingsManager.SetActive(false);
            Register.SetActive(false);
            Login.SetActive(false);
            MainMenu.SetActive(true);
        }
        else
        {
            settingsMenu.SetActive(false);
            settingsManager.SetActive(false);
            Register.SetActive(true);
            Login.SetActive(false);
            MainMenu.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
