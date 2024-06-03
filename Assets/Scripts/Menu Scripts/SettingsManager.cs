using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject volumeSettings;
    public GameObject keybindSettings;
    public GameObject settingsMenu;
    public GameObject brightnessSettings;

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

    public void OnBrightnessButtonClick()
    {
        SwitchToBrightnessMenu();
    }


    private void SwitchToVolumeMenu()
    {
        volumeSettings.SetActive(true);
        keybindSettings.SetActive(false);
        settingsMenu.SetActive(false);
        brightnessSettings.SetActive(false);
    }

    private void SwitchToSettingsMenu()
    {
        volumeSettings.SetActive(false);
        keybindSettings.SetActive(false);
        settingsMenu.SetActive(true);
        brightnessSettings.SetActive(false);
    }

    public void SwitchToKeybindMenu()
    {
        volumeSettings.SetActive(false);
        keybindSettings.SetActive(true);
        settingsMenu.SetActive(false);
        brightnessSettings.SetActive(false);
    }

    private void SwitchToBrightnessMenu()
    {
        volumeSettings.SetActive(false);
        keybindSettings.SetActive(false);
        settingsMenu.SetActive(false);
        brightnessSettings.SetActive(true);
    }
}
