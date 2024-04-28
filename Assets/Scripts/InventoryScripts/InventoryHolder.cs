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

    private void Start()
    {
        SaveGameManager.data.playerInventory = new InventorySaveData(inventorySystem, equipmentInventorySystem);
    }

    protected virtual void Awake()
    {
        SaveLoad.OnLoadGame += LoadInventory;

        inventorySystem = new InventorySystem(inventorySize);
        equipmentInventorySystem = new EquipmentInventorySystem(4);
    }

    protected void LoadInventory(SaveData saveData)
    {
        if (saveData.playerInventory.InvSystem != null)
        {
            this.inventorySystem = saveData.playerInventory.InvSystem;
        }

        if (saveData.playerInventory.EquipInvSystem != null)
        {
            this.equipmentInventorySystem = saveData.playerInventory.EquipInvSystem;
        }
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