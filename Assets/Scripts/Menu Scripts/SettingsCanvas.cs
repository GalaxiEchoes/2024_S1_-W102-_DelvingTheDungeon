using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SettingsCanvas : MonoBehaviour
{
    // Array that stores all screen resolutions
    Resolution[] resolutions;
    // Resolution dropdown reference
    public TMPro.TMP_Dropdown dropdownResolution;
    //Fullscreen Toggle reference
    public Toggle fullScreenToggle;

    void Start()
    {
        //Sets the game to fullscreen when you start it
        Screen.fullScreen = true;
        //Gets the screen resolutions
        resolutions = Screen.resolutions;
        dropdownResolution.ClearOptions();

        List<string> options = new List<string>();

        int curretResolutionIndex = 0;
        //Creates available dropdown options
        for (int i = 0; i < resolutions.Length; i++)
        {

            int refreshRate = (int)(resolutions[i].refreshRateRatio.numerator / resolutions[i].refreshRateRatio.denominator);
            string option = resolutions[i].width + " x " + resolutions[i].height + "@" + refreshRate + "Hz";
                
            options.Add(option);
            

            // Checks if resolution is the current screen resolution
            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height && resolutions[i].refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
            {
                curretResolutionIndex = i;
            }
        }
        //Adds the options and sets the resolution dropdown
        dropdownResolution.AddOptions(options);
        dropdownResolution.value = curretResolutionIndex;
        dropdownResolution.RefreshShownValue();

        if(fullScreenToggle != null)
        {
            fullScreenToggle.isOn = Screen.fullScreen;
            fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
        }  
    }

    //Sets the screen resolution based on the selected dropdown option
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        AdjustCanvasScaler();
    }

    //sets the game to fullscreen based on the toggle option
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    //Adjusts the canvas scaller to the UI scaling
    void AdjustCanvasScaler()
    {
        CanvasScaler canvasScaler = FindObjectOfType<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
    }
}
