using System.Collections;
using System.Collections.Generic;
using static MenuManager;
using UnityEngine;

public class ShopLogic : MonoBehaviour
{
    //Shop instance reference
    public static ShopLogic shopInstance;

    [Header("Object States")]
    public bool isOpen = false;

    private void Awake()
    {
        if (shopInstance == null)
        {
            shopInstance = this;
        }
    }

    // Set the shop to open
    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
        }
    }

    // Set the shop to closed
    public void Close()
    {
        if (isOpen)
        {
            isOpen = false;
        }
    }
}
