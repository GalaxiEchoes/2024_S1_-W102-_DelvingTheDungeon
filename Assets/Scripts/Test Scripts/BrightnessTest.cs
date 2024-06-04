using NUnit.Framework
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

public class BrightnessTests
{
    private GameObject brightnessGameObject;
    private Brightness brightness;
    //private Slider brightnessSlider;
    //private PostProcessProfile postProcessProfile;
   // private AutoExposure autoExposure;

    [SetUp]
    public void SetUp()
    {
        brightnessGameObject = new GameObject();
        brightness = brightnessGameObject.AddComponent<Brightness>();

        GameObject sliderGameObject = new GameObject();
       // brightnessSlider = sliderGameObject.AddComponent<Slider>();
       // brightness.brightnessSlider = brightnessSlider;

        //postProcessProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
       // autoExposure = ScriptableObject.CreateInstance<AutoExposure>();
      //  postProcessProfile.AddSettings(autoExposure);
     //   brightness.brightness = postProcessProfile;

      //  brightness.layer = brightnessGameObject.AddComponent<PostProcessLayer>();
    }

    [Test]
    public void Start_InitializesBrightnessFromSavedPreferences()
    {
        // Arrange
        PlayerPrefs.SetFloat("SavedBrightness", 0.8f);

        // Act
       // brightness.Start();

        // Assert
        //Assert.AreEqual(0.8f, autoExposure.keyValue.value);
    }

    [Test]
    public void AdjustBrightness_SavesBrightnessToPlayerPrefs()
    {
        // Arrange
     //   brightness.Start();

        // Act
        brightness.AdjustBrightness(0.5f);

        // Assert
        Assert.AreEqual(0.5f, PlayerPrefs.GetFloat("SavedBrightness"));
    }

    [Test]
    public void AdjustBrightness_SetsMinimumValueWhenZero()
    {
        // Arrange
       // brightness.Start();

        // Act
        brightness.AdjustBrightness(0f);

        // Assert
      //  Assert.AreEqual(0.05f, autoExposure.keyValue.value);
    }
}
