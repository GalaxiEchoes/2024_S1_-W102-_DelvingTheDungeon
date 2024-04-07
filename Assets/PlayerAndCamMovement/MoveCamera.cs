using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private float smoothingSpeed;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, cameraPosition.position, smoothingSpeed * Time.deltaTime);
    }
}
