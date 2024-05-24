using System.Collections;
using System.Collections.Generic;
using static MenuManager;
using UnityEngine;

public class ShopLogic : MonoBehaviour
{
    //Shop instance reference
    public static ShopLogic shopInstance;

    private MenuManager menuManager;

    [Header("Object States")]
    public bool isOpen = false;

    private void Awake()
    {
        if (shopInstance == null)
        {
            shopInstance = this;
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            Debug.Log("Open Shop");
            isOpen = true;
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            Debug.Log("Close Shop");
            isOpen = false;
        }
    }
}
