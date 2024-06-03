using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopSystem : MonoBehaviour
{
    // Shop system stores list of slots which contain items
    private List<ISlot> shopSlots;

    // Getters
    public List<ISlot> ShopSlots => shopSlots;
    public int ShopSize => ShopSlots.Count;

    // Listener to check when a slot in the shop system has changed
    public UnityAction<ISlot> OnShopSlotChanged;

    // Initialise shop system slots with items
    public ShopSystem(InventoryItemData[] items)
    {
        shopSlots = new List<ISlot>();

        foreach (var item in items)
        {
            ISlot slot = new ISlot(item, 1); 
            shopSlots.Add(slot);
        }
    }
}
