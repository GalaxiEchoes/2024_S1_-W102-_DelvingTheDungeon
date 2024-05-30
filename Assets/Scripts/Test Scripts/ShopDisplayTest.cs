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
