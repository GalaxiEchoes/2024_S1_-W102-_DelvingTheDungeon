using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject playerLogin;
    public GameObject mainMenu;
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
        playerLogin.SetActive(false);
        mainMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SwitchToMain()
    {
        settingsMenu.SetActive(false);
        settingsManager.SetActive(false);
        playerLogin.SetActive(false);
        mainMenu.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(IsLoggedIn())
        {
            settingsMenu.SetActive(false);
            settingsManager.SetActive(false);
            playerLogin.SetActive(false);
            mainMenu.SetActive(true);
        }
        else
        {
            settingsMenu.SetActive(false);
            settingsManager.SetActive(false);
            playerLogin.SetActive(true);
            mainMenu.SetActive(false);
        }
        
    }
}
