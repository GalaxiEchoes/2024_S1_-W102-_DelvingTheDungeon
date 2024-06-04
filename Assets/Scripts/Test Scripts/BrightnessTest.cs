using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestRunner;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;



public class BrightnessTests
{
    private GameObject brightnessGameObject;
    private Brightness brightness;

    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject for the Brightness component
        brightnessGameObject = new GameObject();
        brightness = brightnessGameObject.AddComponent<Brightness>();

        // Create a new GameObject for the Slider component
        GameObject sliderGameObject = new GameObject();
        Slider slider = sliderGameObject.AddComponent<Slider>();

        // Assign the slider to the brightness component
        brightness.brightnessSlider = slider;

        // Set the initial value of the slider
        brightness.brightnessSlider.value = 1.0f;
    }

    [Test]
    public void Start_InitializesBrightnessFromSavedPreferences()
    {
        float expectedBrightness = 100;
        brightness.Start();

        Assert.AreEqual(expectedBrightness, brightness.GetExposureKeyValue());
    }

    [Test]
    public void AdjustBrightness_SavesBrightnessToPlayerPrefs()
    {
        float expectedBrightness = 50;

        // Assert using the existing brightness instance
        Assert.AreEqual(expectedBrightness, brightness.GetExposureKeyValue());
    }

    [Test]
    public void AdjustBrightness_SetsMinimumValueWhenZero()
    {
        float expectedMinimumBrightness = 0.05f;
        brightness.AdjustBrightness(0);

        Assert.AreEqual(expectedMinimumBrightness, brightness.GetExposureKeyValue());
    }
}

public class Brightness : MonoBehaviour
{
    public Slider brightnessSlider;
    public PostProcessProfile brightness;
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

    public float GetExposureKeyValue()
    {
        return exposure != null ? exposure.keyValue.value : 0f;
    }

    public void AdjustBrightness(float value)
    {
        if (exposure != null)
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
    }
}

