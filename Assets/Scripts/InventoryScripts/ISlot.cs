using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ISlot
{
    // Slot contains items information, stack size, and what type of equipment it is
    [SerializeField] private InventoryItemData itemData;
    [SerializeField] private int stackSize;
    [SerializeField] private EquipmentTag equipmentTag;

    // Getters
    public InventoryItemData ItemData => itemData;
    public int StackSize => stackSize;
    public EquipmentTag EquipmentTag => equipmentTag;

    // Initialise slot with item data and amount
    public ISlot(InventoryItemData source, int amount)
    {
        itemData = source;
        stackSize = amount;
    }

    public ISlot()
    {
        ClearSlot();
    }

    // Empty slot as required
    public void ClearSlot()
    {
        itemData = null;
        stackSize = -1;
    }

    // Assign type of equipment to slot
    public void AssignEquipmentTag(EquipmentTag tag)
    {
        equipmentTag = tag;
    }

    // Assign slots item data
    public void AssignItem(ISlot invSlot)
    {
        // Checks if same item and adds to stack
        if (itemData == invSlot.ItemData)
        {
            AddToStack(invSlot.stackSize);
        }
        else
        {
            itemData = invSlot.itemData;
            stackSize = 0;
            AddToStack(invSlot.stackSize);
        }
    }

    // Update slot with new data and amount
    public void UpdateInventorySlot(InventoryItemData data, int amount)
    {
        itemData = data;
        stackSize = amount;
    }

    // Checks how much space is left in slot and returns amount remaining
    public bool RoomLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = ItemData.MaxStackSize - stackSize;
        return RoomLeftInStack(amountToAdd);
    }

    // Checks if there is room in slot to add item
    public bool RoomLeftInStack(int amountToAdd)
    {
        if (stackSize + amountToAdd <= itemData.MaxStackSize) return true;
        else return false;
    }

    // Adds to slots stack
    public void AddToStack(int amount)
    {
        stackSize += amount;

    }

    // Removes from slot stack
    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
    }
}
