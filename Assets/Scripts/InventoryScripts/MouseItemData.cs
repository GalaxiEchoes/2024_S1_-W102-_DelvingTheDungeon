using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class MouseItemData : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount;
    public ISlot AssignedInventorySlot;
    public InventorySlot_UI PreviousInventorySlot;
    public EquipmentSlot_UI PreviousEquipmentSlot;

    private Transform _playerTransform;
    private Transform _orientation;
    public Camera cam;

    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemCount.text = "";

        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _orientation = _playerTransform.Find("Orientation");
    }

    public void UpdateMouseSlot(ISlot invSlot)
    {
        AssignedInventorySlot.AssignItem(invSlot);
        ItemSprite.sprite = invSlot.ItemData.sprite;
        ItemCount.text = invSlot.StackSize.ToString();
        ItemSprite.color = Color.white;
    }

    public void UpdateMousePreviousUISlot(InventorySlot_UI previousSlot)
    {
        PreviousInventorySlot = previousSlot;
    }

    public void UpdateMousePreviousUISlot(EquipmentSlot_UI previousSlot)
    {
        PreviousEquipmentSlot = previousSlot;
    }

    public void ResetPreviousUIEquipmentSlot()
    {
        PreviousEquipmentSlot = null;
    }

    public void ResetPreviousUIInventorySlot()
    {
        PreviousInventorySlot = null;
    }

    private void Update()
    {
        if (AssignedInventorySlot.ItemData != null)
        {
            Vector3 mousePos = Mouse.current.position.value;
            mousePos.z = Camera.main.farClipPlane * 1.3f;
            Vector3 worldPoint = cam.ScreenToWorldPoint(mousePos);

            transform.position = worldPoint;

            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
            {
                ClearSlot();
            }

            //Change this to imputmanager
            if (Input.GetKey(KeyCode.Q))
            {
                if (AssignedInventorySlot.ItemData.ItemPrefab != null)
                {
                    //Drops item on the ground in the direction the player is facing
                    Physics.Raycast(_playerTransform.transform.position + _orientation.transform.forward * 3f, Vector3.down, out RaycastHit hit);
                    Vector3 dropPosition = hit.point + new Vector3(0, 0.4f, 0);

                    //Creates each item in slot stack
                    for(int i = 0; i < AssignedInventorySlot.StackSize; i++ ) 
                    {
                        Instantiate(AssignedInventorySlot.ItemData.ItemPrefab, dropPosition, Quaternion.identity);
                    }
                    ClearSlot();
                }
            }
        }
    }

    public void ClearSlot()
    {
        AssignedInventorySlot.ClearSlot();
        ItemCount.text = "";
        ItemSprite.color = Color.clear;
        ItemSprite.sprite = null;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
