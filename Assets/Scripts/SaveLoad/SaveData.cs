using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public List<string> collectedItems;
    public SerializableDictionary<string, ItemPickUpSaveData> activeItems;

    public InventorySaveData playerInventory;

    public SaveData()
    {
        collectedItems = new List<string>();
        activeItems = new SerializableDictionary<string, ItemPickUpSaveData>();
        playerInventory = new InventorySaveData();
    }
}
