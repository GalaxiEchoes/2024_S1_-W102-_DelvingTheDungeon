using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FurnitureSpawner;

[CreateAssetMenu(menuName = "Furniture/FurniturePrefabData")]
[Serializable]public class FurniturePrefabData : ScriptableObject
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public Vector3Int size;
    [SerializeField] public Style style;
    [SerializeField] public HallType hallType;
}
