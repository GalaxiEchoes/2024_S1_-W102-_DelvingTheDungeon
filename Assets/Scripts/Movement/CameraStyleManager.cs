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

    [SerializeField] private GameObject neckReference;

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
            transform.position = cameraPos.transform.position;
            firstPersonCam.transform.localPosition = Vector3.zero;
            neckReference.transform.localScale = Vector3.zero;
        }
        if (newStyle == CameraStyle.CombatCam)
        {
            combatCam.SetActive(true);
            neckReference.transform.localScale = Vector3.one;
        }
        if (newStyle == CameraStyle.ThirdPersonCam)
        {
            thirdPersonCam.SetActive(true);
            neckReference.transform.localScale = Vector3.one;
        }

        currentStyle = newStyle;
    }
}
