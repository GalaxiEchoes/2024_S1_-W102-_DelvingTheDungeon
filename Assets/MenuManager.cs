using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject mainMenuScreen;

    // Start is called before the first frame update
    void Start()
    {
        Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.MenuOpenCloseInput)
        {
            if (!PauseManager.instance.IsPaused)
            {
                Pause();
            }

        }
        else if (InputManager.instance.UIMenuCloseInput)
        {


            if (PauseManager.instance.IsPaused)
            {
                Unpause();
            }
        }
    }

    public void Pause()
    {
        PauseManager.instance.PauseGame();
        SwitchToPauseScreen();
    }

    public void Unpause()
    {
        PauseManager.instance.UnpauseGame();
        SwitchToGameScreen();
    }

    public void OnPlayButtonClick()
    {
        Unpause();
    }

    public void OnMainMenuButtonClick()
    {
        SwitchToMainMenuScreen();
    }

    public void OnContinueButtonClick()
    {
        SwitchToPauseScreen();
    }

    public void OnExitButtonClick()
    {
        ExitGame();
    }

    private void SwitchToGameScreen()
    {
        mainMenuScreen.SetActive(false);
        pauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SwitchToPauseScreen()
    {
        mainMenuScreen.SetActive(false);
        pauseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SwitchToMainMenuScreen()
    {
        mainMenuScreen.SetActive(true);
        pauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Add logic to save the current game state and update to playfab database
    }

    private void ExitGame()
    {
        //Add logic to save the current game state and update to playfab database
        Debug.Log("Game Ended");
        Application.Quit();
    }
}
