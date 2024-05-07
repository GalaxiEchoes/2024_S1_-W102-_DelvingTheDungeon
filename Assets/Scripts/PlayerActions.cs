using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.XR;
using UnityEngine.ProBuilder.Shapes;
using PlayFab.EconomyModels;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private TextMeshPro UseText;
    [SerializeField] private Transform Camera;
    [SerializeField] private float MaxUseDistance = 5f;
    [SerializeField] private LayerMask UseLayers;
    [SerializeField] private LayerMask EnemyLayers;
    [SerializeField] private InventoryHolder Inventory;
    [SerializeField] private Player playerData;
    [SerializeField] private SwitchCameraStyle CameraSwitcher;

    public Transform playerTransform;
    public Transform orientation;

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
            playerData = player.GetComponent<Player>();
        Inventory = player.GetComponent<InventoryHolder>();
    }

    public void OnInteract()
    {
        //if (Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit, MaxUseDistance, UseLayers))
        if (Physics.Raycast(playerTransform.position, orientation.forward, out RaycastHit hit, MaxUseDistance, UseLayers))
        {
            if (hit.collider.TryGetComponent<DoorLogic>(out DoorLogic door))
            {
                if (door.IsOpen)
                {
                    door.Close();
                }
                else
                {
                    door.Open(Camera.transform.position);
                }
            }
            else if (hit.collider.TryGetComponent<StartStairLogic>(out StartStairLogic logic))
            {
                logic.LoadPrevLevel(Camera.transform.position);
            }
            else if (hit.collider.TryGetComponent<EndStairLogic>(out EndStairLogic endLogic))
            {
                endLogic.LoadNextLevel(Camera.transform.position);
            }
            else if (hit.collider.TryGetComponent<ShopLogic>(out ShopLogic shop))
            {
                shop.Open();
            }
            else if (hit.collider.TryGetComponent<ChestLogic>(out ChestLogic chest))
            {
                chest.Open(Camera.transform.position, Inventory);
            }
        }
    }

    public void OnAttack()
    {
        Debug.Log("Attack");
        if (Physics.Raycast(playerTransform.position, orientation.forward, out RaycastHit hit, MaxUseDistance, EnemyLayers))
        {
            if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Debug.Log("Hit Enemy");
                enemy.TakeDamage(playerData.attack);
            }
        }
    }

    void Update()
    {
        if (Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit, MaxUseDistance, UseLayers))
        {
            if (hit.collider.TryGetComponent<DoorLogic>(out DoorLogic door))
            {
                if (door.IsOpen)
                {
                    UseText.SetText("Close \"E\"");
                }
                else
                {
                    UseText.SetText("Open \"E\"");
                }
            }
            else
            {
                UseText.SetText("Use \"E\"");
            }

            UseText.gameObject.SetActive(true);
            UseText.transform.position = hit.point - (hit.point - Camera.position).normalized * 0.2f;
            UseText.transform.rotation = Quaternion.LookRotation((hit.point - Camera.position).normalized);
        }
        else
        {
            UseText.gameObject.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnAttack();
        }
    } 
}
