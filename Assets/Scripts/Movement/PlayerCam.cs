using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX;
    public float sensY;

    [Header("Player References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerObj;

    //Rotation Variables
    public float xRotation;
    public float yRotation;
    private bool rotationActive;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rotationActive = true;
    }

    public float mouseX;
    public float mouseY;
    public float clampValue;

    private void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        //mouseX = Mathf.Clamp(mouseX, -clampValue, clampValue);
       // mouseY = Mathf.Clamp(mouseY, -clampValue, clampValue);

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        if (InputManager.instance.FirstPersonCamPressed)
        {
            rotationActive = !rotationActive;
        }
        if (rotationActive)
        {
            playerObj.rotation = orientation.transform.rotation;
        }
    }
}
