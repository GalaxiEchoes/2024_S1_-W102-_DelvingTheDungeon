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

/*i am trying to serialize a class in unity into a json and the class had a List of ScriptableObject that doesn't want to serialize maybe because of the prefab it references, I have to be able to serialize the subclass only. I was told that this was the correct way so that i could serialize the super class and still have it link to*/