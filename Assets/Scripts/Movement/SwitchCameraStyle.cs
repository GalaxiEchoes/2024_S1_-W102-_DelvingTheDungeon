using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ThirdPersonCam;

public class SwitchCameraStyle : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private GameObject firstPersonCam;
    [SerializeField] private GameObject combatCam;
    [SerializeField] private GameObject thirdPersonCam;
    [SerializeField] private GameObject cameraPos;

    [Header("Camera On Load")]
    public CameraStyle currentStyle;

    public enum CameraStyle
    {
        FirstPersonCam,
        CombatCam,
        ThirdPersonCam
    }

    void Start()
    {
        currentStyle = CameraStyle.FirstPersonCam;
    }

    void Update()
    {
        if (InputManager.instance.FirstPersonCamPressed) switchCameraStyle(CameraStyle.FirstPersonCam);
        if (InputManager.instance.CombatCamPressed) switchCameraStyle(CameraStyle.CombatCam);
        if (InputManager.instance.ThirdPersonCamPressed) switchCameraStyle(CameraStyle.ThirdPersonCam);
    }

    private void switchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);

        if (newStyle == CameraStyle.FirstPersonCam) firstPersonCam.transform.position = cameraPos.transform.position;
        if (newStyle == CameraStyle.CombatCam) combatCam.SetActive(true);
        if (newStyle == CameraStyle.ThirdPersonCam) thirdPersonCam.SetActive(true);

        currentStyle = newStyle;
    }
}
