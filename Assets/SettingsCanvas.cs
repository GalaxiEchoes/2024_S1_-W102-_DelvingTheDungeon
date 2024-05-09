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

        for (int i = 0; i <resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " +  resolutions[i].height;
            options.Add(option);

        }

        dropdownResolution.AddOptions(options);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
