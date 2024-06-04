using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

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

        // Simulate setting up the dependencies required by the Start method
        //brightness.brightness = ScriptableObject.CreateInstance<PostProcessProfile>(); // Simulate PostProcessProfile assignment
        brightness.brightnessSlider = new GameObject().AddComponent<Slider>(); // Simulate Slider assignment
        brightness.brightnessSlider.value = 1.0f; // Set initial value for the slider
    }

    [Test]
    public void AdjustBrightness_UpdatesExposureCorrectly()
    {
        // Call the Start method explicitly to simulate its execution
        brightness.Start();

        // Change the brightness
        float newBrightness = 0.5f;
        brightness.AdjustBrightness(newBrightness);

        // Verify that the exposure value is updated correctly
        Assert.AreEqual(newBrightness, brightness.GetExposureKeyValue());
    }
}


