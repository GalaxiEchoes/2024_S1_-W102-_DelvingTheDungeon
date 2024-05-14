using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using System.IO;
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
    public GameObject healthBar;
    public GameObject staminaBar;
    public GameObject moneyHUD;
    public GameObject ThirdPersonUseText;
    public int bossLevel = 5;

    private void Start()
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
        healthBar.SetActive(true);
        staminaBar.SetActive(true);
        moneyHUD.SetActive(true);
        ThirdPersonUseText.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SwitchToPauseScreen()
    {
        settingsMenu.SetActive(false);
        settingsManager.SetActive(false);
        pauseScreen.SetActive(true);
        inventoryScreen.SetActive(false);
        healthBar.SetActive(false);
        staminaBar.SetActive(false);
        moneyHUD.SetActive(false);
        ThirdPersonUseText.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SwitchToSettingsScreen()
    {
        settingsMenu.SetActive(true);
        settingsManager.SetActive(true);
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        healthBar.SetActive(false);
        staminaBar.SetActive(false);
        moneyHUD.SetActive(false);
        ThirdPersonUseText.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SwitchToMainMenuScreen()
    {
        persistenceManager.SaveWorldState();
        SceneManager.LoadScene(0);
    }

    private void SwitchToInventoryScreen()
    {
        settingsMenu.SetActive(false);
        settingsManager.SetActive(false);
        pauseScreen.SetActive(false);
        inventoryScreen.SetActive(true);
        healthBar.SetActive(false);
        staminaBar.SetActive(false);
        moneyHUD.SetActive(false);
        ThirdPersonUseText.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ExitGame()
    {
        persistenceManager.SaveWorldState();
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
