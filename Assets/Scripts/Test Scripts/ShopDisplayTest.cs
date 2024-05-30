using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ShopDisplayTest
{
    //Shop Display
    GameObject shopDisplayObject;
    ShopDisplay shopDisplay;
    GameObject shopSlotUI;

    //Items for shop
    InventoryItemData item1;
    InventoryItemData item2;
    InventoryItemData[] items;

    //Slots UI
    ShopSlot_UI slot1;
    ShopSlot_UI slot2;
    ShopSlot_UI[] slots;
    ISlot islot1;
    ISlot islot2;

    //Player
    GameObject playerObject;
    Player testPlayer;

    [SetUp]
    public void SetUp()
    {
        shopDisplayObject = new GameObject();
        shopDisplay = shopDisplayObject.AddComponent<ShopDisplay>();

        items = new InventoryItemData[2];
        //Item Setup
        item1 = ScriptableObject.CreateInstance<InventoryItemData>();
        item1.itemName = "Item 1";
        item1.cost = 50;
        items[0] = item1;

        item2 = ScriptableObject.CreateInstance<InventoryItemData>();
        item2.itemName = "Item 2";
        item2.cost = 25;
        items[1] = item2;

        shopDisplay.shopItems = items;

        //Player Setup
        playerObject = new GameObject();
        testPlayer = playerObject.AddComponent<Player>();
        testPlayer.money = 100;
        shopDisplay.currentPlayer = testPlayer;

        //Slots UI Setup
        slots = new ShopSlot_UI[2];
        slot1 = new GameObject().AddComponent<ShopSlot_UI>();
        islot1 = new ISlot();
        slot1.assignedItemSlot = islot1;
        slots[0] = slot1;

        slot2 = new GameObject().AddComponent<ShopSlot_UI>();
        islot2 = new ISlot();
        slot2.assignedItemSlot = islot2;
        slots[1] = slot2;

        shopDisplay.slots = slots;
        
    }

    [Test]
    public void AssignSlots()
    {
        shopDisplay.Start();

        Assert.IsNotNull(shopDisplay.currentPlayer);
        Assert.IsNotNull(shopDisplay.ShopSystem);
    }

    [Test]
    public void SlotClicked()
    {
        // This test should fail initially because the SlotClicked method is not implemented yet.
        Assert.Fail("Test not implemented yet.");
    }

    [Test]
    public void OnHover()
    {
        // This test should fail initially because the OnHover method is not implemented yet.
        Assert.Fail("Test not implemented yet.");
    }

    [Test]
    public void OnPurchase()
    {
        // This test should fail initially because the OnPurchase method is not implemented yet.
        Assert.Fail("Test not implemented yet.");
    }
}
