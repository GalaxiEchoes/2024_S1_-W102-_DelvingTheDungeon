using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EquipmentInventorySystem
{
    // List of equipment slots, visible in the inspector
    [SerializeField] private List<ISlot> equipmentSlots;

    // Getters
    public List<ISlot> EquipmentSlots => equipmentSlots;
    public int EquipmentSize => EquipmentSlots.Count;

    // Unity event action to notify when an inventory slot changes
    public UnityAction<ISlot> OnInventorySlotChanged;

    // Initialize the equipment inventory system with a slots of a specified size
    public EquipmentInventorySystem(int size)
    {
        equipmentSlots = new List<ISlot>(size);

        for (int i = 0; i < size; i++)
        {
            equipmentSlots.Add(new ISlot());
        }

        //None, Weapon = 0, Head = 1, Chest = 2, Arms = 3
        equipmentSlots[0].AssignEquipmentTag(EquipmentTag.Weapon);
        equipmentSlots[1].AssignEquipmentTag(EquipmentTag.Head);
        equipmentSlots[2].AssignEquipmentTag(EquipmentTag.Chest);
        equipmentSlots[3].AssignEquipmentTag(EquipmentTag.Arms);
    }
}
