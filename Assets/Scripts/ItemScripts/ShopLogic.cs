using System.Collections;
using System.Collections.Generic;
using static MenuManager;
using UnityEngine;

public class ShopLogic : MonoBehaviour
{
    //Shop instance reference
    public static ShopLogic shopInstance;
    public ShopDisplay shopDisplay;

    [Header("Object States")]
    public bool isOpen = false;

    private void Awake()
    {
        if (shopInstance == null)
        {
            shopInstance = this;
        }

        GameObject shop = GameObject.FindGameObjectWithTag("ShopDisplay");
        shopDisplay = shop.GetComponent<ShopDisplay>();
    }

    // Set the shop to open
    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
        }
    }

    // Set the shop to closed and clear the selected item to purchase
    public void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            shopDisplay.clearSelectedItem();
        }
    }
}
