using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.XR;
using UnityEngine.ProBuilder.Shapes;
using Cinemachine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private TextMeshPro UseText;
    [SerializeField] private TextMeshProUGUI ThirdPersonUseText;
    [SerializeField] private Transform Camera;
    [SerializeField] private float MaxUseDistance = 5f;
    [SerializeField] private LayerMask UseLayers;
    [SerializeField] private LayerMask EnemyLayers;
    [SerializeField] private InventoryHolder Inventory;
    [SerializeField] private Player playerData;

    [SerializeField] private Transform CameraPos;
    [SerializeField] private CameraStyleManager CameraStyleManager;
    [SerializeField] private CinemachineFreeLook ThirdPersonCam;
    [SerializeField] private CinemachineFreeLook CombatCam;
    [SerializeField] private Vector3 CurrentCam;

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
                if (door.IsOpen)
                {
                    door.Close();
                }
                else
                {
                    door.Open(CameraPos.transform.position);
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
                chest.Open(Inventory);
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
        switch (CameraStyleManager.currentStyle)
        {
            case CameraStyleManager.CameraStyle.FirstPersonCam:
                CurrentCam = Camera.transform.forward;
                break;
            case CameraStyleManager.CameraStyle.ThirdPersonCam:
                CurrentCam = ThirdPersonCam.Follow.position - ThirdPersonCam.transform.position;
                CurrentCam.Normalize();
                break;
            default:
                CurrentCam = CombatCam.Follow.position - CombatCam.transform.position;
                CurrentCam.Normalize();
                break;
        }

        if (Physics.Raycast(CameraPos.transform.position, CurrentCam, out RaycastHit hit, MaxUseDistance, UseLayers))
        {
            if (hit.collider.TryGetComponent<DoorLogic>(out DoorLogic door))
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
            else
            {
                ThirdPersonUseText.SetText("Use (E)");
                UseText.SetText("Use (E)");
            }

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
        }


        if (Input.GetMouseButtonDown(0))
        {
            OnAttack();
        }

    } 
}
