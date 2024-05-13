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

[System.Serializable]
public struct InventorySaveData
{
    public InventorySystem InvSystem;
    public EquipmentInventorySystem EquipInvSystem;

    public InventorySaveData(InventorySystem invSystem, EquipmentInventorySystem eqipInvSystem)
    {
        InvSystem = invSystem;
        EquipInvSystem = eqipInvSystem;
    }
}