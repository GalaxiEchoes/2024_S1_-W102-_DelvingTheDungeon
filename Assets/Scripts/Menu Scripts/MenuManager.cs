//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEditor;
//using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PersistenceManager;
using System.Linq;

public class MenuManager : MonoBehaviour
{
    public PersistenceManager persistenceManager;
    public GameObject pauseScreen;
    public GameObject inventoryScreen;
    public GameObject settingsManager;
    public GameObject settingsMenu;
    public GameObject InGameDisplay;

    //Shop canvas
    public GameObject shopScreen;

    public int bossLevel = 5;

    private void Start()
    {
        Unpause();
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
        else if (InputManager.instance.InventoryOpen)
        {
            if (!PauseManager.instance.IsPaused)
            {
                InventoryPause();
            }
        }
        else if (ShopLogic.shopInstance.isOpen)
        {
            if (!PauseManager.instance.IsPaused)
            {
                ShopPause();
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
        else if (!ShopLogic.shopInstance.isOpen && shopScreen.activeSelf)
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

    private void ShopPause()
    {
        PauseManager.instance.PauseGame();
        SwitchToShopScreen();
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

    public void OnSettingsButtonClick()
    {
        SwitchToSettingsScreen();
    }

    public void OnBackButtonClick()
    {
        SwitchToPauseScreen();
    }

    private void SwitchToGameScreen()
    {
        settingsMenu.SetActive(false);
        settingsManager.SetActive(false);
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        InGameDisplay.SetActive(true);
        shopScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SwitchToPauseScreen()
    {
        settingsMenu.SetActive(false);
        settingsManager.SetActive(false);
        pauseScreen.SetActive(true);
        inventoryScreen.SetActive(false);
        InGameDisplay.SetActive(false);
        shopScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SwitchToSettingsScreen()
    {
        settingsMenu.SetActive(true);
        settingsManager.SetActive(true);
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        InGameDisplay.SetActive(false);
        shopScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SwitchToMainMenuScreen()
    {
        persistenceManager.SaveEverything();
        SceneManager.LoadScene(0);
    }

    private void SwitchToInventoryScreen()
    {
        settingsMenu.SetActive(false);
        settingsManager.SetActive(false);
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(true);
        InGameDisplay.SetActive(false);
        shopScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SwitchToShopScreen()
    {
        settingsMenu.SetActive(false);
        settingsManager.SetActive(false);
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        InGameDisplay.SetActive(false);
        shopScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ExitGame()
    {
        persistenceManager.SaveEverything();
        Debug.Log("Game Ended");
        Application.Quit();
    }

    public void StartNewGame()
    {
        persistenceManager.StartNewGame();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void StartNewRun()
    {
        persistenceManager.StartNewRun();
    }
}
