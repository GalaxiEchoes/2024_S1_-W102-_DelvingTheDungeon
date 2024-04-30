using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int inventorySize;
    [SerializeField] protected InventorySystem inventorySystem;
    [SerializeField] protected EquipmentInventorySystem equipmentInventorySystem;

    public InventorySystem InventorySystem => inventorySystem;
    public EquipmentInventorySystem EquipmentInventorySystem => equipmentInventorySystem;

    protected virtual void Awake()
    {

        inventorySystem = new InventorySystem(inventorySize);
        equipmentInventorySystem = new EquipmentInventorySystem(4);
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