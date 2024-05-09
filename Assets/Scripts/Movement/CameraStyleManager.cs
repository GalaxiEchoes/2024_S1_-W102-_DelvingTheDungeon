using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ThirdPersonCam;

public class CameraStyleManager : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private GameObject firstPersonCam;
    [SerializeField] private GameObject combatCam;
    [SerializeField] private GameObject thirdPersonCam;
    [SerializeField] private GameObject cameraPos;
    [SerializeField] private GameObject PlayerObject;

    [Header("Character Head")]
    [SerializeField] private GameObject headReference;

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
        SwitchCameraStyle(currentStyle);
    }

    void Update()
    {
        if (InputManager.instance.FirstPersonCamPressed) SwitchCameraStyle(CameraStyle.FirstPersonCam);
        if (InputManager.instance.CombatCamPressed) SwitchCameraStyle(CameraStyle.CombatCam);
        if (InputManager.instance.ThirdPersonCamPressed) SwitchCameraStyle(CameraStyle.ThirdPersonCam);
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);

        if (newStyle == CameraStyle.FirstPersonCam) { 
            headReference.transform.localScale = Vector3.zero;
            firstPersonCam.transform.position = cameraPos.transform.position; 
        }
        if (newStyle == CameraStyle.CombatCam)
        {
            headReference.transform.localScale = Vector3.one;
            combatCam.SetActive(true);
        }
        if (newStyle == CameraStyle.ThirdPersonCam)
        {
            headReference.transform.localScale = Vector3.one;
            thirdPersonCam.SetActive(true);
        }

        currentStyle = newStyle;
    }
}