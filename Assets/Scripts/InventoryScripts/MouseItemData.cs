using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MouseItemData : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount;
    public ISlot AssignedInventorySlot;
    public InventorySlot_UI PreviousInventorySlot;
    public EquipmentSlot_UI PreviousEquipmentSlot;

    private Transform _playerTransform;

    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemCount.text = "";

        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
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
            transform.position = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
            {
                ClearSlot();
            }

            if (Input.GetKey(KeyCode.Q))
            {
                if (AssignedInventorySlot.ItemData.ItemPrefab != null)
                {
                    // Increase Y position by adding an offset value (e.g., 3f)
                    Vector3 dropPosition = _playerTransform.position + _playerTransform.forward * 3f;
                    dropPosition.y += 0.35f;

                    Instantiate(AssignedInventorySlot.ItemData.ItemPrefab, dropPosition, Quaternion.identity);
                    ClearSlot();
                }

                Debug.Log("q key pressed");
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
