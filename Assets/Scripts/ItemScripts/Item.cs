using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotTag { None, Weapon, Head, Chest, Arms}

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public SlotTag itemTag;

    public GameObject equipmentPrefab;
}
