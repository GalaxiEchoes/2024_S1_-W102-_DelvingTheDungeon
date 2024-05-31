using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject playerLogin;
    public GameObject mainMenu;
    public GameObject settingsManager;
    public GameObject settingsMenu;

    private bool isLoggedIn;

    public void SetLoggedIn(bool value)
    {
        isLoggedIn = value;
    }

    public bool IsLoggedIn()
    {
        return isLoggedIn;
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
