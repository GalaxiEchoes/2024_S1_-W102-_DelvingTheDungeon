using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCanvas : MonoBehaviour
{
    Resolution[] resolutions;

    public Dropdown dropdownResolution;

    void Start()
    {
        resolutions = Screen.resolutions;

        dropdownResolution.ClearOptions();

        List<string> options = new List<string>();

        int curretResolutionIndex = 0;
        for (int i = 0; i <resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " +  resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                curretResolutionIndex = i;
            }
        }

        dropdownResolution.AddOptions(options);
        dropdownResolution.value = curretResolutionIndex;
        dropdownResolution.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }


    
}
