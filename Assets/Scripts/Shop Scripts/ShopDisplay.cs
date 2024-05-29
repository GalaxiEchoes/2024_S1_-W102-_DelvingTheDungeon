using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopDisplay : MonoBehaviour
{
    [SerializeField] InventoryItemData[] shopItems; // List of items to be sold in the shop
    [SerializeField] TextMeshProUGUI itemDescriptionArea;
    [SerializeField] Image selectedItemImage;
    [SerializeField] TextMeshProUGUI selectedItemDescriptionArea;
    [SerializeField] Player currentPlayer;
    [SerializeField] private ShopSlot_UI[] slots;

    protected InventoryItemData selectedItemData;
    protected Dictionary<ShopSlot_UI, ISlot> slotDictionary;
    protected ShopSystem shopSystem;

    public ShopSystem ShopSystem => shopSystem;
    public Dictionary<ShopSlot_UI, ISlot> SlotDictionary => slotDictionary;

    protected void Start()
    {
        shopSystem = new ShopSystem(shopItems);
        shopSystem.OnShopSlotChanged += UpdateSlot;
        AssignSlot(shopSystem);
    }

    //Assign inventory slot uis to the inventorysystem dictionary
    public void AssignSlot(ShopSystem invToDisplay)
    {
        slotDictionary = new Dictionary<ShopSlot_UI, ISlot>();

        if (slots.Length != shopSystem.ShopSize) Debug.Log($"Inventory slots out of sync on {this.gameObject}");
        for (int i = 0; i < shopSystem.ShopSize; i++)
        {
            slotDictionary.Add(slots[i], shopSystem.ShopSlots[i]);
            slots[i].Init(ShopSystem.ShopSlots[i]);
        }
    }

    protected void UpdateSlot(ISlot updatedSlot)
    {
        foreach (var slot in SlotDictionary)
        {
            if (slot.Value == updatedSlot)
            {
                slot.Key.UpdateUISlot(updatedSlot);
            }
        }
    }

    //If mouse is hovering over a shop slot, show that item's description
    public void OnHover(ShopSlot_UI selectedUISlot)
    {
        if (selectedUISlot.AssignedItemSlot.ItemData != null)
        {
            itemDescriptionArea.text = selectedUISlot.AssignedItemSlot.ItemData.Description;
        }
        else
        {
            itemDescriptionArea.text = "";
        }
    }

    //Method responsible for clicking on a shop slot
    public void SlotClicked(ShopSlot_UI clickedUISlot)
    {
        if (clickedUISlot.AssignedItemSlot.ItemData != null)
        {
            selectedItemData = clickedUISlot.AssignedItemSlot.ItemData;
            selectedItemImage.sprite = selectedItemData.sprite;
            selectedItemDescriptionArea.text = "Select Item: " + selectedItemData.itemName;
        }
    }

    // Method called on button click to purchase selected item
    public void OnPurchase()
    {
        if (selectedItemData != null)
        {
            var inventory = currentPlayer.transform.GetComponent<InventoryHolder>();

            if (!inventory)
            {
                return;
            }
            else
            {
                inventory.InventorySystem.AddToInventory(selectedItemData, 1);
            }
        }
    }
}
