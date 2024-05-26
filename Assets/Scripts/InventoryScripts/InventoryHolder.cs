using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable] public class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int inventorySize;
    public InventorySystem InventorySystem;
    public EquipmentInventorySystem EquipmentInventorySystem;

    protected virtual void Awake()
    {
        InventorySystem = new InventorySystem(inventorySize);
        EquipmentInventorySystem = new EquipmentInventorySystem(4);
    }
}