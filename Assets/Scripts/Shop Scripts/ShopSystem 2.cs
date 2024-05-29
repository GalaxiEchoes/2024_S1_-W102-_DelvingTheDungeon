using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopSystem : MonoBehaviour
{
    private List<ISlot> shopSlots;

    public List<ISlot> ShopSlots => shopSlots;
    public int ShopSize => ShopSlots.Count;

    public UnityAction<ISlot> OnShopSlotChanged;

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
