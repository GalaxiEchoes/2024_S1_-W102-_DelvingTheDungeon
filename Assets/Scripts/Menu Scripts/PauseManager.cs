using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    public static CinemachineBrain cinemachineBrain;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
        {
            cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        }
        
    }

    public void PauseGame()
    {
        IsPaused = true;
        cinemachineBrain.enabled = false;
        Time.timeScale = 0f;
        InputManager.PlayerInput.SwitchCurrentActionMap("UI");
    }

    public void UnpauseGame()
    {
        IsPaused = false;
        cinemachineBrain.enabled = true;
        Time.timeScale = 1f;
        InputManager.PlayerInput.SwitchCurrentActionMap("InGame");
    }
}
