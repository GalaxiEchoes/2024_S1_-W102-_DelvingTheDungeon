using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InventorySystem
{
    // Slots contained in the inventory system
    [SerializeField] private List<ISlot> inventorySlots;

    // Getters
    public List<ISlot> InventorySlots => inventorySlots;
    public int InventorySize => InventorySlots.Count;

    // Action listener to check when slot has been changed
    public UnityAction<ISlot> OnInventorySlotChanged;

    // Create inventory system with slots of given size
    public InventorySystem(int size)
    {
        inventorySlots = new List<ISlot>(size);

        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new ISlot());
        }
    }

    // Add item to inventory returns true if successfully added or false if not
    public bool AddToInventory(InventoryItemData itemToAdd, int amountToAdd)
    {
        if (ContainsItem(itemToAdd, out List<ISlot> invSlot)) //Check whether item exists in inventory
        {
            foreach (var slot in invSlot)
            {
                if (slot.RoomLeftInStack(amountToAdd)) //check list for slot with room left in stack and add to this slot
                {
                    slot.AddToStack(amountToAdd);
                    OnInventorySlotChanged?.Invoke(slot);
                    return true;
                }
            }
        }

        if (HasFreeSlot(out ISlot freeSlot)) //Gets the first available slot
        {
            freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
            OnInventorySlotChanged?.Invoke(freeSlot);
            return true;
        }

        //if no free slots return false
        return false;
    }

    // Checks whether the inventory already contains the item we are adding
    public bool ContainsItem(InventoryItemData itemToAdd, out List<ISlot> invSlot)
    {
        //Check all inventory slots and where the inventory
        //slots data is equal to the item we want to add, put that slot in a list
        invSlot = InventorySlots.Where(i => i.ItemData == itemToAdd).ToList();

        return invSlot == null ? false : true;
    }

    // Checks whether inventory has a free slot
    public bool HasFreeSlot(out ISlot freeSlot)
    {
        freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);
        return freeSlot == null ? false : true;
    }
}
