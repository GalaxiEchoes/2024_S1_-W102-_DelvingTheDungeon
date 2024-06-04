using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Brightness : MonoBehaviour
{
    public Slider brightnessSlider;
    public PostProcessProfile brightness;
    public PostProcessLayer layer;
    AutoExposure exposure;

    // Start is called before the first frame update
    public void Start()
    {
        brightness.TryGetSettings(out exposure);
        if (brightnessSlider != null)
        {
            AdjustBrightness(PlayerPrefs.GetFloat("SavedBrightness", brightnessSlider.value));
        }
        else
        {
            AdjustBrightness(PlayerPrefs.GetFloat("SavedBrightness", 1.0f));
        }
    }

    public void AdjustBrightness(float value)
    {
        if (value != 0)
        {
            exposure.keyValue.value = value;
            PlayerPrefs.SetFloat("SavedBrightness", value);
        }
        else
        {
            exposure.keyValue.value = 0.05f;
        }
    }

    public float GetExposureKeyValue()
    {
        return exposure != null ? exposure.keyValue.value : 0.0f;
    }
}
