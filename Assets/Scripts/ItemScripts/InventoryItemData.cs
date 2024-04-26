using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentTag { None, Weapon, Head, Chest, Arms }

[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
public class InventoryItemData : ScriptableObject
{
    public string itemName;
    [TextArea(4, 4)]
    public string Description;

    public Sprite sprite;
    public int MaxStackSize;
    public EquipmentTag itemTag;

    //if the item can be equipped
    public GameObject equipmentPrefab;
}

