using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseInventoryItem;
    [SerializeField] TextMeshProUGUI itemDescriptionArea;
    [SerializeField] Player currentPlayer;

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

    //If mouse is hovering over an inventory slot, show that item's description
    public void OnHover(InventorySlot_UI selectedUISlot)
    {
        if (selectedUISlot.AssignedInventorySlot.ItemData != null)
        {
            itemDescriptionArea.text = selectedUISlot.AssignedInventorySlot.ItemData.Description;
        }
        else
        {
            itemDescriptionArea.text = "";
        }
    }

    //Method responsible for clicking on inventory slots and moving items between clicked slots
    public void SlotClicked(InventorySlot_UI clickedUISlot)
    {
        // Clicked slot has an item - mouse doesn't have an item - pick up that item
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
            mouseInventoryItem.UpdateMousePreviousUISlot(clickedUISlot);
            mouseInventoryItem.ResetPreviousUIEquipmentSlot();
            mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
            clickedUISlot.ClearSlot();
            return;
        }

        //Clicked slot doesn't have an item - mouse does have an item - place the mouse item into the empty slot
        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            mouseInventoryItem.UpdateMousePreviousUISlot(clickedUISlot);
            mouseInventoryItem.ResetPreviousUIEquipmentSlot();
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
        // Clicked equipment slot has an item - mouse doesn't have an item - pick up that item
        if (clickedUISlot.AssignedEquipmentSlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
            //on item unequipped minus stats from player
            currentPlayer.minusStats(clickedUISlot.AssignedEquipmentSlot.ItemData.healthEffect,
                clickedUISlot.AssignedEquipmentSlot.ItemData.staminaEffect,
                clickedUISlot.AssignedEquipmentSlot.ItemData.attackEffect,
                clickedUISlot.AssignedEquipmentSlot.ItemData.defenseEffect);

            mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedEquipmentSlot);
            mouseInventoryItem.UpdateMousePreviousUISlot(clickedUISlot);
            mouseInventoryItem.ResetPreviousUIInventorySlot();
            clickedUISlot.ClearSlot();

            return;
        }

        //Clicked slot doesn't have an item - mouse does have an item - check if items tag matches equipment tag
        //if matches place one off stack from the mouse item into the empty equipment slot and keep item on mouse if leftover in stack
        if (clickedUISlot.AssignedEquipmentSlot.ItemData == null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            if (clickedUISlot.AssignedEquipmentSlot.EquipmentTag == mouseInventoryItem.AssignedInventorySlot.ItemData.itemTag) //check if same tag
            {
                if (mouseInventoryItem.AssignedInventorySlot.StackSize > 1) //Stack size of more than 1 split and place 1 item on equipment slot
                {
                    var clonedSlot = new ISlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
                    clonedSlot.RemoveFromStack(1);

                    mouseInventoryItem.AssignedInventorySlot.RemoveFromStack(mouseInventoryItem.AssignedInventorySlot.StackSize - 1);
                    clickedUISlot.AssignedEquipmentSlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                    clickedUISlot.UpdateUISlot();
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.UpdateMouseSlot(clonedSlot);

                    //on item equipped apply items stats to player
                    currentPlayer.addStats(clickedUISlot.AssignedEquipmentSlot.ItemData.healthEffect,
                        clickedUISlot.AssignedEquipmentSlot.ItemData.staminaEffect,
                        clickedUISlot.AssignedEquipmentSlot.ItemData.attackEffect,
                        clickedUISlot.AssignedEquipmentSlot.ItemData.defenseEffect);

                    return;
                }
                else //Stack size of 1 place item on equipment slot
                {
                    clickedUISlot.AssignedEquipmentSlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                    clickedUISlot.UpdateUISlot();

                    mouseInventoryItem.ClearSlot();

                    //on item equipped apply items stats to player
                    currentPlayer.addStats(clickedUISlot.AssignedEquipmentSlot.ItemData.healthEffect,
                        clickedUISlot.AssignedEquipmentSlot.ItemData.staminaEffect,
                        clickedUISlot.AssignedEquipmentSlot.ItemData.attackEffect,
                        clickedUISlot.AssignedEquipmentSlot.ItemData.defenseEffect);

                    return;
                }
            }
            else
            {
                //Return item to previous equipment slot or inventory if previous slot empty or same item?
                if (mouseInventoryItem.PreviousInventorySlot != null)
                {
                    Debug.Log("Returning to inventory slot");
                    mouseInventoryItem.PreviousInventorySlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.PreviousInventorySlot.UpdateUISlot();
                    return;
                }
                else if (mouseInventoryItem.PreviousEquipmentSlot != null)
                {
                    Debug.Log("Returning to equipment slot");
                    mouseInventoryItem.PreviousEquipmentSlot.AssignedEquipmentSlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                    //on item equipped apply items stats to player
                    currentPlayer.addStats(mouseInventoryItem.AssignedInventorySlot.ItemData.healthEffect,
                        mouseInventoryItem.AssignedInventorySlot.ItemData.staminaEffect,
                        mouseInventoryItem.AssignedInventorySlot.ItemData.attackEffect,
                        mouseInventoryItem.AssignedInventorySlot.ItemData.defenseEffect);
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.PreviousEquipmentSlot.UpdateUISlot();
                    return;
                }
            }
        }

        //Both slots have an item - swap if matching tag?
        if (clickedUISlot.AssignedEquipmentSlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            if (clickedUISlot.AssignedEquipmentSlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData)
            {
                //if mouse item is same as equipped item return
                return;
            }
            if (clickedUISlot.AssignedEquipmentSlot.EquipmentTag == mouseInventoryItem.AssignedInventorySlot.ItemData.itemTag) //check if same tag
            {
                if (mouseInventoryItem.AssignedInventorySlot.StackSize > 1) //Stack size must be 1 on mouse - if more than 1 return - can't hold 2 items on mouse!!
                {
                    return;
                }
                else //Stack size of 1 place item on equipment slot
                {
                    var clonedSlot = new ISlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);

                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedEquipmentSlot);

                    clickedUISlot.AssignedEquipmentSlot.AssignItem(clonedSlot);
                    clickedUISlot.UpdateUISlot();

                    //on item equipped apply items stats to player
                    currentPlayer.addStats(clickedUISlot.AssignedEquipmentSlot.ItemData.healthEffect,
                        clickedUISlot.AssignedEquipmentSlot.ItemData.staminaEffect,
                        clickedUISlot.AssignedEquipmentSlot.ItemData.attackEffect,
                        clickedUISlot.AssignedEquipmentSlot.ItemData.defenseEffect);

                    return;
                }
            }
        }
    }
}
