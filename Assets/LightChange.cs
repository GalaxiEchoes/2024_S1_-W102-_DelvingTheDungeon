using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightChange : MonoBehaviour
{

    public Slider brightnessSlider;
    public Light sceneLight;
    public float maxIntensity = 2.5f;
    public float minIntensity = -2.5f;


    // Update is called once per frame
    void Update()
    {
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, brightnessSlider.value);
            

        sceneLight.intensity = intensity;   
    }
}
