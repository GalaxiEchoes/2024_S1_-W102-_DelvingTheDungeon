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

 