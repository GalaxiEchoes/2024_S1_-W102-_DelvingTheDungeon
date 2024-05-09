using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject volumeSettings;
    public GameObject keybindSettings;
    public GameObject settingsMenu;


    public void OnVolumeButtonClick()
    {
        SwitchToVolumeMenu();
    }

    public void OnBackButtonClick()
    {
        SwitchToSettingsMenu();
    }

    public void OnKeyBindButtonClick()
    {
        SwitchToKeybindMenu();
    }

    private void SwitchToVolumeMenu()
    {
        volumeSettings.SetActive(true);
        keybindSettings.SetActive(false);
        settingsMenu.SetActive(false);
    }

    private void SwitchToSettingsMenu()
    {
        volumeSettings.SetActive(false);
        keybindSettings.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void SwitchToKeybindMenu()
    {
        volumeSettings.SetActive(false);
        keybindSettings.SetActive(true);
        settingsMenu.SetActive(false);
    }
}