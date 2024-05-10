using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EquipmentTag { None, Weapon, Head, Chest, Arms };

[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]

[Serializable] public class InventoryItemData : ScriptableObject
{
    public string itemName;
    [TextArea(4, 4)]
    public string Description;

    public Sprite sprite;
    public int MaxStackSize;
    public EquipmentTag itemTag;

    public GameObject ItemPrefab;

    //item stats to apply to player
    public int healthEffect;
    public int staminaEffect;
    public int attackEffect;
    public int defenseEffect;
}
