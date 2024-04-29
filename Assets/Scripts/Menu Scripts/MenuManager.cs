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
    public GameObject inventoryScreen;


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
        else if (InputManager.instance.InventoryOpen)
        {
            if (!PauseManager.instance.IsPaused)
            {
                InventoryPause();
            }
        }
        else if (InputManager.instance.InventoryClose && inventoryScreen.activeSelf)
        {
            if (PauseManager.instance.IsPaused)
            {
                Unpause();
            }
        }
        else if (InputManager.instance.UIMenuCloseInput && pauseScreen.activeSelf)
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

    public void InventoryPause()
    {
        PauseManager.instance.PauseGame();
        SwitchToInventoryScreen();
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
        SaveGameManager.TryLoadData();
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
        inventoryScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SwitchToPauseScreen()
    {
        mainMenuScreen.SetActive(false);
        pauseScreen.SetActive(true);
        inventoryScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SwitchToMainMenuScreen()
    {
        mainMenuScreen.SetActive(true);
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Add logic to save the current game state and update to playfab database
        SaveGameManager.SaveData();
    }

    private void SwitchToInventoryScreen()
    {
        mainMenuScreen.SetActive(false);
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Add logic to save the current game state and update to playfab database
        SaveGameManager.SaveData();
    }

    private void ExitGame()
    {
        //Add logic to save the current game state and update to playfab database
        SaveGameManager.SaveData();
        Debug.Log("Game Ended");
        Application.Quit();
    }
}
