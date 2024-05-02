using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCuller : MonoBehaviour
{
    public Transform playerCamera; // Reference to the player's camera
    public float cullDistance = 50f; // Maximum distance at which lights will be rendered

    private Light[] lights; // Array to store references to all lights in the scene

    void Start()
    {
        lights = FindObjectsOfType<Light>();
    }

    void Update()
    {
        // Iterate through all lights in the scene
        foreach (Light light in lights)
        {
            // Calculate the distance between the light and the player's camera
            float distanceToCamera = Vector3.Distance(light.transform.position, playerCamera.position);

            // Check if the distance is greater than the cull distance
            if (distanceToCamera > cullDistance)
            {
                // If the distance exceeds the cull distance, disable the light
                light.enabled = false;
            }
            else
            {
                // If the distance is within the cull distance, enable the light
                light.enabled = true;
            }
        }
    }
}
