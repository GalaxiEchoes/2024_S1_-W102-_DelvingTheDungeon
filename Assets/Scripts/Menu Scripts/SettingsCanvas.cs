using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsCanvas : MonoBehaviour
{
    Resolution[] resolutions;
    public TMPro.TMP_Dropdown dropdownResolution;
    public Toggle fullScreenToggle;

    void Start()
    {
        Screen.fullScreen = true;

        resolutions = Screen.resolutions;

        dropdownResolution.ClearOptions();

        List<string> options = new List<string>();

        int curretResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                curretResolutionIndex = i;
            }
        }

        dropdownResolution.AddOptions(options);
        dropdownResolution.value = curretResolutionIndex;
        dropdownResolution.RefreshShownValue();

        if(fullScreenToggle != null)
        {
            fullScreenToggle.isOn = Screen.fullScreen;
            fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
        }

        
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        AdjustCanvasScaler();
    }


    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

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
