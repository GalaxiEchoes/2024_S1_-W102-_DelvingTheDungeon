using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] private InventorySlot_UI[] slots;
    [SerializeField] private EquipmentSlot_UI[] equipmentSlots;

    protected override void Start()
    {
        base.Start();

        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.InventorySystem;
            equipmentInventorySystem = inventoryHolder.EquipmentInventorySystem;

            inventorySystem.OnInventorySlotChanged += UpdateSlot;
            equipmentInventorySystem.OnInventorySlotChanged += UpdateEquipmentSlot;
        }
        else Debug.LogWarning($"No inventory assigned to {this.gameObject}");

        AssignSlot(inventorySystem);
        AssignSlot(equipmentInventorySystem);
    }

    //Assign inventory slot uis to the inventorysystem dictionary
    public override void AssignSlot(InventorySystem invToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, ISlot>();

        if (slots.Length != inventorySystem.InventorySize) Debug.Log($"Inventory slots out of sync on {this.gameObject}");
        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(InventorySystem.InventorySlots[i]);
        } 
    }

    //Assign equipment slot uis to the equipmentsystem dictionary
    public override void AssignSlot(EquipmentInventorySystem invToDisplay)
    {
        equipmentSlotDictionary = new Dictionary<EquipmentSlot_UI, ISlot>();

        if (equipmentSlots.Length != equipmentInventorySystem.EquipmentSize) Debug.Log($"Inventory slots out of sync on {this.gameObject}");
        for (int i = 0; i < equipmentInventorySystem.EquipmentSize; i++)
        {
            equipmentSlotDictionary.Add(equipmentSlots[i], equipmentInventorySystem.EquipmentSlots[i]);
            equipmentSlots[i].Init(equipmentInventorySystem.EquipmentSlots[i]);
        }
    }
}
