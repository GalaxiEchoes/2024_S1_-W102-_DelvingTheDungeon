using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EquipmentSlot_UI : MonoBehaviour
{
    // Each equipment slot stores image of item, amount in stack, and slot containing item
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private ISlot assignedEquipmentSlot;

    // Button on UI element so slots can be clicked/selected
    private Button button;

    // Getters
    public ISlot AssignedEquipmentSlot => assignedEquipmentSlot;
    public InventoryDisplay ParentDisplay { get; private set; }

    private void Awake()
    {
        // Reset the slot on awake
        ClearSlot();

        // Add listener to check when a slot has been clicked
        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

        // Set the parent display to the inventory display
        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();
    }

    // Initialise slot value 
    public void Init(ISlot slot)
    {
        assignedEquipmentSlot = slot;
        UpdateUISlot(slot);
    }

    // Update the slot to match current items data
    public void UpdateUISlot(ISlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.sprite;
            itemSprite.color = Color.white;

            itemCount.text = "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void UpdateUISlot()
    {
        if (assignedEquipmentSlot != null) UpdateUISlot(assignedEquipmentSlot);
    }

    // Empty slot as required
    public void ClearSlot()
    {
        assignedEquipmentSlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = "";
    }

    // On click of button/ui element call the slotclicked method in  inventorydisplay class
    public void OnUISlotClick()
    {
        ParentDisplay?.SlotClicked(this);
    }
}
