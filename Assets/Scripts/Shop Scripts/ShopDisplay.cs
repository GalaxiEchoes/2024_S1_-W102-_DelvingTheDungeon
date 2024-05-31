using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopDisplay : MonoBehaviour
{
    // List of items to be sold in the shop
    [SerializeField] InventoryItemData[] shopItems;

    // Slots that contain the items to be bought
    [SerializeField] ShopSlot_UI[] slots;

    // Current player who holds inventory
    [SerializeField] Player currentPlayer;

    // Selected item information
    [Header("Selected Item Information")]
    [SerializeField] Image selectedItemImage;
    protected InventoryItemData selectedItemData;
    [SerializeField] TextMeshProUGUI selectedItemDescriptionArea;
    private Sprite initialSprite;

    [Header("Text Areas")]
    // Purchase response message (error or success)
    [SerializeField] TextMeshProUGUI responseMessageArea;
    // Area to show description of item
    [SerializeField] TextMeshProUGUI itemDescriptionArea;
    // Area to show players money
    [SerializeField] TextMeshProUGUI playerMoneyArea;


    // Store the slot information
    protected Dictionary<ShopSlot_UI, ISlot> slotDictionary;
    protected ShopSystem shopSystem;

    // Getters
    public ShopSystem ShopSystem => shopSystem;
    public Dictionary<ShopSlot_UI, ISlot> SlotDictionary => slotDictionary;

    public void Start()
    {
        // Assign items to the shop system
        shopSystem = new ShopSystem(shopItems);
        shopSystem.OnShopSlotChanged += UpdateSlot;

        // Assign the shop system to the slots
        AssignSlot(shopSystem);
        initialSprite = selectedItemImage.sprite;

        playerMoneyArea.text = currentPlayer.money.ToString();
    }

    //Assign shop slot uis to the shop slot dictionary
    public void AssignSlot(ShopSystem invToDisplay)
    {
        slotDictionary = new Dictionary<ShopSlot_UI, ISlot>();

        if (slots.Length != shopSystem.ShopSize) Debug.Log($"Shop slots out of sync on {this.gameObject}");
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
        // If the slot clicked contains an item, set this item as the selected item's data
        if (clickedUISlot.AssignedItemSlot.ItemData != null)
        {
            selectedItemData = clickedUISlot.AssignedItemSlot.ItemData;
            selectedItemImage.sprite = selectedItemData.sprite;
            selectedItemDescriptionArea.text = "Selected Item: " + selectedItemData.itemName +" $"+selectedItemData.cost;
            responseMessageArea.text = "";
        }
    }

    // Method called on button click to purchase selected item
    public void OnPurchase()
    {
        // Check the player has selected an item to buy
        if (selectedItemData != null)
        {
            // Check the player has enough money to buy item
            if (currentPlayer.money > selectedItemData.cost)
            {
                var inventory = currentPlayer.transform.GetComponent<InventoryHolder>();

                // Check the player's inventory exists to add item to
                if (!inventory)
                {
                    return;
                }
                else
                {
                    //Check there is space in inventory for item
                    if (inventory.InventorySystem.AddToInventory(selectedItemData, 1))
                    {
                        // There is space so add to inventory
                        responseMessageArea.color = Color.green;
                        responseMessageArea.text = "Success! " + selectedItemData.itemName + " added to inventory";
                        currentPlayer.minusMoney(selectedItemData.cost);

                        // Update player money text to represent current money
                        playerMoneyArea.text = currentPlayer.money.ToString();
                    }
                    else
                    {
                        // There is no space so print error message
                        responseMessageArea.color = Color.red;
                        responseMessageArea.text = "Sorry! You do not have space in your inventory to purchase item!";
                    }

                }
            }
            else // Player cannot afford item - Print error message
            {
                responseMessageArea.color = Color.red;
                responseMessageArea.text = "Sorry! You do not have enough money to purchase " + selectedItemData.itemName;
            }

        }
        else // Player has not selected an item to buy - Print error message
        {
            responseMessageArea.color = Color.red;
            responseMessageArea.text = "Please select an item to purchase!";
        }
    }

    // Reset the selected item to purchase called when shop is closed
    public void clearSelectedItem()
    {
        selectedItemData = null;
        selectedItemImage.sprite = initialSprite;
        selectedItemDescriptionArea.text = "Select Item to Purchase";
        responseMessageArea.text = "";
    }
}
