using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EquipmentInventorySystem
{
    [SerializeField] private List<ISlot> equipmentSlots;

    public List<ISlot> EquipmentSlots => equipmentSlots;
    public int EquipmentSize => EquipmentSlots.Count;

    public UnityAction<ISlot> OnInventorySlotChanged;

    public EquipmentInventorySystem(int size)
    {
        equipmentSlots = new List<ISlot>(size);

        for (int i = 0; i < size; i++)
        {
            equipmentSlots.Add(new ISlot());
        }
    }
}
