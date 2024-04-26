using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseInventoryItem;

    protected InventorySystem inventorySystem;
    protected EquipmentInventorySystem equipmentInventorySystem;
    protected Dictionary<InventorySlot_UI, ISlot> slotDictionary;
    protected Dictionary<EquipmentSlot_UI, ISlot> equipmentSlotDictionary;

    public InventorySystem InventorySystem => inventorySystem;
    public EquipmentInventorySystem EquipmentInventorySystem => equipmentInventorySystem;
    public Dictionary<InventorySlot_UI, ISlot> SlotDictionary => slotDictionary;
    public Dictionary<EquipmentSlot_UI, ISlot> EquipmentSlotDictionary => equipmentSlotDictionary;

    protected virtual void Start()
    {

    }

    public abstract void AssignSlot(InventorySystem invToDisplay);
    public abstract void AssignSlot(EquipmentInventorySystem invToDisplay);

    protected virtual void UpdateSlot(ISlot updatedSlot)
    {
        foreach (var slot in SlotDictionary)
        {
            if (slot.Value == updatedSlot)
            {
                slot.Key.UpdateUISlot(updatedSlot);
            }
        }
    }

    protected virtual void UpdateEquipmentSlot(ISlot updatedSlot)
    {
        foreach (var slot in EquipmentSlotDictionary)
        {
            if (slot.Value == updatedSlot)
            {
                slot.Key.UpdateUISlot(updatedSlot);
            }
        }
    }

    //Method responsible for clicking on inventory slots and moving items between clicked slots
    public void SlotClicked(InventorySlot_UI clickedUISlot)
    {
        // Clicked slot has an item - mouse doesn't have an item - pick up that item
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
            mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
            clickedUISlot.ClearSlot();
            return;
        }

        //Clicked slot doesn't have an item - mouse does have an item - place the mouse item into the empty slot
        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseInventoryItem.ClearSlot();
            return;
        }


        //Both slots have an item... combine them if same or swap them if different
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;
            if (isSameItem && clickedUISlot.AssignedInventorySlot.RoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();
                return;
            }
            else if (isSameItem &&
                !clickedUISlot.AssignedInventorySlot.RoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack))
            {
                if (leftInStack < 1) SwapSlots(clickedUISlot); //Stack is full so swap the items
                else //Slot is not at max capacity, so take what's needed from the mouse inventory
                {
                    int remainingOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;
                    clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                    clickedUISlot.UpdateUISlot();

                    var newItem = new ISlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remainingOnMouse);
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.UpdateMouseSlot(newItem);
                    return;
                }
            }
            else if (!isSameItem)
            {
                SwapSlots(clickedUISlot);
                return;
            }
        }
    }

    //Swap clicked ui slot with item on mouse
    private void SwapSlots(InventorySlot_UI clickedUISlot)
    {
        var clonedSlot = new ISlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
        mouseInventoryItem.ClearSlot();

        mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        clickedUISlot.ClearSlot();
        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();
    }

    //Method for clicking on equipment slots
    public void SlotClicked(EquipmentSlot_UI clickedUISlot)
    {
        Debug.Log("Equipment slot clicked");
    }
}
