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
    [SerializeField] private float unlockTimer = 1.0f;
    [SerializeField] private float maxUnlockTimerValue = 1.0f;
    [SerializeField] private Image radialUnlockUI = null;
    private bool updateUnlockTimer = false;

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
            else if (hit.collider.TryGetComponent<ShopLogic>(out ShopLogic shop))
            {
                shop.Open();
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

    public void OnAttack()
    {
        if (Physics.Raycast(CameraPos.transform.position, Camera.transform.forward, out RaycastHit hit, MaxUseDistance, EnemyLayers))
        {
            //if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Debug.Log("Hit Enemy");
                //enemy.TakeDamage(playerData.attack);
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
                    HandleUnlocking(ref door.IsLocked);
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
                    HandleUnlocking(ref chest.IsLocked);
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

        if (Input.GetMouseButtonDown(0))
        {
            OnAttack();
        }

    }

    public void UpdateTimer()
    {
        if (!updateUnlockTimer)
        {
            updateUnlockTimer = true;
        }

        if (updateUnlockTimer)
        {
            unlockTimer += Time.deltaTime;
            radialUnlockUI.fillAmount = unlockTimer;

            if (unlockTimer >= maxUnlockTimerValue)
            {
                unlockTimer = maxUnlockTimerValue;
                radialUnlockUI.fillAmount = maxUnlockTimerValue;
                radialUnlockUI.enabled = false;
                updateUnlockTimer = false;
            }
        }
    }

    public void HandleUnlocking(ref bool lockedObject)
    {
        ThirdPersonUseText.SetText("Unlock Hold (E)");
        UseText.SetText("Unlock Hold (E)");

        //Handles Unlocking the door
        if (InputManager.instance.InteractBeingHeld)
        {
            updateUnlockTimer = false;
            unlockTimer -= Time.deltaTime;
            radialUnlockUI.enabled = true;
            radialUnlockUI.fillAmount = unlockTimer;

            if (unlockTimer <= 0)
            {
                unlockTimer = maxUnlockTimerValue;
                radialUnlockUI.fillAmount = maxUnlockTimerValue;
                radialUnlockUI.enabled = false;
                lockedObject = false;
            }
        }
        else
        {
            UpdateTimer();
        }

        if (InputManager.instance.InteractReleased)
        {
            updateUnlockTimer = true;
        }
    }
}
