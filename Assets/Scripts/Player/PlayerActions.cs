using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.XR;
using UnityEngine.ProBuilder.Shapes;
using Cinemachine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    [Header("Interact UI tip texts")]
    [SerializeField] private TextMeshPro UseText;
    [SerializeField] private TextMeshProUGUI ThirdPersonUseText;

    [Header("Interaction Detection and Handling")]
    [SerializeField] private float MaxUseDistance = 5f;
    [SerializeField] private LayerMask UseLayers;
    [SerializeField] private LayerMask EnemyLayers;
    [SerializeField] private InventoryHolder Inventory;
    [SerializeField] private Player playerData;

    [Header("Interact Raycast Origin Handling")]
    [SerializeField] private Transform Camera;
    [SerializeField] private Transform CameraPos;
    [SerializeField] private CameraStyleManager CameraStyleManager;
    [SerializeField] private CinemachineFreeLook ThirdPersonCam;
    [SerializeField] private CinemachineFreeLook CombatCam;
    [SerializeField] private Vector3 CurrentCamPos;

    [Header("Unlock Handling")]
    [SerializeField] private float loadTimer = 1.0f;
    [SerializeField] private float maxUnlockTimerValue = 1.0f;
    [SerializeField] private Image radialLoadingUI = null;
    private bool updateLoadingTimer = false;

    private void Awake()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (camera != null)
            Camera = camera.transform;

        GameObject useText = GameObject.FindGameObjectWithTag("UseText");
        if (useText != null)
            UseText = useText.GetComponent<TextMeshPro>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerData = player.GetComponent<Player>();
            Inventory = player.GetComponent<InventoryHolder>();
        }
    }

    public void OnInteract()
    {
        if (Physics.Raycast(CameraPos.transform.position, Camera.transform.forward, out RaycastHit hit, MaxUseDistance, UseLayers))
        {
            if (hit.collider.TryGetComponent<DoorLogic>(out DoorLogic door))
            {
                //Only allows player to open door if it is unlocked
                if (!door.IsLocked)
                {
                    if (door.IsOpen)
                    {
                        door.Close();
                    }
                    else
                    {
                        door.Open(CameraPos.transform.position);
                    }
                }
            }
            else if (hit.collider.TryGetComponent<StartStairLogic>(out StartStairLogic logic))
            {
                logic.LoadPrevLevel(CameraPos.transform.position);
            }
            else if (hit.collider.TryGetComponent<EndStairLogic>(out EndStairLogic endLogic))
            {
                endLogic.LoadNextLevel(CameraPos.transform.position);
            }
            else if (hit.collider.TryGetComponent<ShopLogic>(out ShopLogic shop)) // Checks if interacting with shop
            {
                if (shop.isOpen) //If shops open then close the shop
                {
                    shop.Close(); 
                }
                else if (!PauseManager.instance.IsPaused) //If game not paused, i.e. not on any menu screen, then open the shop
                {
                    shop.Open(); 
                }
            }
            else if (hit.collider.TryGetComponent<ChestLogic>(out ChestLogic chest))
            {
                if (!chest.IsLocked)
                {
                    chest.Open(Inventory);
                }
            }
        }
    }

    public void Update()
    {
        //Handles the change between first and third person object interaction positions
        switch (CameraStyleManager.currentStyle)
        {
            case CameraStyleManager.CameraStyle.FirstPersonCam:
                CurrentCamPos = Camera.transform.forward;
                break;
            case CameraStyleManager.CameraStyle.ThirdPersonCam:
                CurrentCamPos = ThirdPersonCam.Follow.position - ThirdPersonCam.transform.position;
                CurrentCamPos.Normalize();
                break;
            default:
                CurrentCamPos = CombatCam.Follow.position - CombatCam.transform.position;
                CurrentCamPos.Normalize();
                break;
        }

        //Finds what object we are looking at to set the correct text
        if (Physics.Raycast(CameraPos.transform.position, CurrentCamPos, out RaycastHit hit, MaxUseDistance, UseLayers))
        {
            if (hit.collider.TryGetComponent<DoorLogic>(out DoorLogic door))
            {
                if(door.IsLocked)
                {
                    ThirdPersonUseText.SetText("Unlock Hold (E)");
                    UseText.SetText("Unlock Hold (E)");
                    HandleLoading(ref door.IsLocked);
                }
                else
                {
                    if (door.IsOpen)
                    {
                        ThirdPersonUseText.SetText("Close (E)");
                        UseText.SetText("Close (E)");
                    }
                    else
                    {
                        ThirdPersonUseText.SetText("Open (E)");
                        UseText.SetText("Open (E)");
                    }
                }
                
            }
            else if(hit.collider.TryGetComponent<ChestLogic>(out ChestLogic chest))
            {
                if (chest.IsLocked)
                {
                    ThirdPersonUseText.SetText("Unlock Hold (E)");
                    UseText.SetText("Unlock Hold (E)");
                    HandleLoading(ref chest.IsLocked);
                }
                else
                {
                    if (chest.IsOpen)
                    {
                        ThirdPersonUseText.SetText("");
                        UseText.SetText("");
                    }
                    else
                    {
                        ThirdPersonUseText.SetText("Open (E)");
                        UseText.SetText("Open (E)");
                    }
                }
            }
            else if(hit.collider.TryGetComponent<TrapLogic>(out TrapLogic trap))
            {
                if (trap.isActive)
                {
                    ThirdPersonUseText.SetText("Disarm (E)");
                    UseText.SetText("Disarm (E)");
                }
                else
                {
                    ThirdPersonUseText.SetText("Arm Trap (E)");
                    UseText.SetText("Arm Trap (E)");
                }
                HandleLoading(ref trap.isActive);
            }
            else if(hit.collider.TryGetComponent<ShopLogic>(out ShopLogic shop))
            {
                if (shop.isOpen)
                {
                    ThirdPersonUseText.SetText("");
                    UseText.SetText("");
                }
                else
                {
                    ThirdPersonUseText.SetText("Open Shop (E)");
                    UseText.SetText("Open Shop (E)");
                }
            }
            else
            {
                //Not doors
                ThirdPersonUseText.SetText("Use (E)");
                UseText.SetText("Use (E)");

                UpdateTimer();
            }

            //Decides which text to place on screen depending on player viewpoint
            if (CameraStyleManager.currentStyle == CameraStyleManager.CameraStyle.FirstPersonCam)
            {
                UseText.gameObject.SetActive(true);
                ThirdPersonUseText.gameObject.SetActive(false);
                UseText.transform.position = hit.point - (hit.point - CameraPos.position).normalized * 0.2f;
                UseText.transform.rotation = Quaternion.LookRotation((hit.point - CameraPos.position).normalized);
            }
            else
            {
                UseText.gameObject.SetActive(false);
                ThirdPersonUseText.gameObject.SetActive(true);
            }
        }
        else
        {
            ThirdPersonUseText.gameObject.SetActive(false);
            UseText.gameObject.SetActive(false);

            UpdateTimer();
        }
    }

    public void UpdateTimer()
    {
        if (!updateLoadingTimer)
        {
            updateLoadingTimer = true;
        }

        if (updateLoadingTimer)
        {
            loadTimer += Time.deltaTime;
            radialLoadingUI.fillAmount = loadTimer;

            if (loadTimer >= maxUnlockTimerValue)
            {
                loadTimer = maxUnlockTimerValue;
                radialLoadingUI.fillAmount = maxUnlockTimerValue;
                radialLoadingUI.enabled = false;
                updateLoadingTimer = false;
            }
        }
    }

    public void HandleLoading(ref bool objectState)
    {
        //Handles Unlocking the door
        if (InputManager.instance.InteractBeingHeld)
        {
            updateLoadingTimer = false;
            loadTimer -= Time.deltaTime;
            radialLoadingUI.enabled = true;
            radialLoadingUI.fillAmount = loadTimer;

            if (loadTimer <= 0)
            {
                loadTimer = maxUnlockTimerValue;
                radialLoadingUI.fillAmount = maxUnlockTimerValue;
                radialLoadingUI.enabled = false;
                objectState = !objectState;
            }
        }
        else
        {
            UpdateTimer();
        }

        if (InputManager.instance.InteractReleased)
        {
            updateLoadingTimer = true;
        }
    }
}
