using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class BrightnessTest
{
    private GameObject brightnessGameObject;
    private Brightness brightness;
    private GameObject camera;

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

        brightnessGameObject.AddComponent<PostProcessVolume>();
        brightness.brightness = AssetDatabase.LoadAssetAtPath<PostProcessProfile>("Assets\\Scenes\\DelvingTheDungeon_Profiles\\Brightness Profile.asset");

        //Camera
        camera = new GameObject();
        camera.AddComponent<Camera>();
        brightness.layer = camera.AddComponent<PostProcessLayer>();
    }

    [Test]
    public void Start_InitializesBrightnessFromSavedPreferences()
    {
        float expectedBrightness = 100.0f;
        PlayerPrefs.SetFloat("SavedBrightness", expectedBrightness);

        brightness.Start();

        Assert.AreEqual(expectedBrightness, brightness.GetExposureKeyValue());
    }

    [Test]
    public void AdjustBrightness_SavesBrightnessToPlayerPrefs()
    {
        float expectedBrightness = 50.0f;

        brightness.Start();
        brightness.AdjustBrightness(expectedBrightness);

        Assert.AreEqual(expectedBrightness, PlayerPrefs.GetFloat("SavedBrightness", 1.5f));
    }

    [Test]
    public void AdjustBrightness_UpdatesExposureCorrectly()
    {
        float expectedBrightness = 0.5f;
        brightness.Start();

        brightness.AdjustBrightness(expectedBrightness);

        // Assert using the existing brightness instance
        Assert.AreEqual(expectedBrightness, brightness.GetExposureKeyValue());
    }

    [Test]
    public void AdjustBrightness_SetsMinimumValueWhenZero()
    {
        float expectedMinimumBrightness = 0.05f;

        brightness.Start();
        brightness.AdjustBrightness(0);

        Assert.AreEqual(expectedMinimumBrightness, brightness.GetExposureKeyValue());
    }
}