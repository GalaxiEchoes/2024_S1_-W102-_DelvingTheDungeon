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
    // Mouse slot stores image of item, amount in stack, and slot containing item
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount;
    public ISlot AssignedInventorySlot;

    // Mouse keeps track of what slot the item previously belonged in
    public InventorySlot_UI PreviousInventorySlot;
    public EquipmentSlot_UI PreviousEquipmentSlot;

    // Transforms to determine where to drop item onto ground drom inventory
    private Transform _playerTransform;
    private Transform _orientation;
    public Camera cam;

    private void Awake()
    {
        // Sets mouse item to be clear
        ClearSlot();

        // Finds player information to get position to determine where to drop item
        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _orientation = _playerTransform.Find("Orientation");
    }

    // Update the slot to match current items data
    public void UpdateMouseSlot(ISlot invSlot)
    {
        AssignedInventorySlot.AssignItem(invSlot);
        ItemSprite.sprite = invSlot.ItemData.sprite;
        ItemCount.text = invSlot.StackSize.ToString();
        ItemSprite.color = Color.white;
    }

    // Update what slot the mouse collected item from
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

    // Update to move item according to mouse position
    private void Update()
    {
        if (AssignedInventorySlot.ItemData != null)
        {
            Vector3 mousePos = Mouse.current.position.value;
            mousePos.z = Camera.main.farClipPlane * 1.3f;
            Vector3 worldPoint = cam.ScreenToWorldPoint(mousePos);

            transform.position = worldPoint;

            // Checks if Q key pressed
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

                    // Clears mouse slot of item
                    ClearSlot();
                }
            }
        }
    }

    // Empty mouse slot as required
    public void ClearSlot()
    {
        AssignedInventorySlot.ClearSlot();
        ItemCount.text = "";
        ItemSprite.color = Color.clear;
        ItemSprite.sprite = null;
    }
}
