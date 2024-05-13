using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FurnitureSpawner;

[Serializable] public class Furniture
{
    public List<bool> chestStates;
    public List<bool> chestLockStates;
    [NonSerialized] public GameObject currentInstance;
    [SerializeField] public FurniturePrefabData prefabData;
    [SerializeField] public Vector3Int pos;
    [SerializeField] public float angle;
    [SerializeField] public Vector3Int scale;

    public Furniture(Vector3Int pos, float angle, Vector3Int scale, Furniture furniture)
    {
        chestStates = new List<bool>();
        chestLockStates = new List<bool>();
        prefabData = furniture.prefabData;
        this.pos = pos;
        this.angle = angle;
        this.scale = scale;
    }

    public Furniture DeepCopy()
    {
        return new Furniture(Vector3Int.zero, 0f, Vector3Int.one,this);
    }

    public void LoadInteractables()
    {
        InteractableTracker prefabTr = currentInstance.GetComponent<InteractableTracker>();

        if(prefabTr != null)
        {
            prefabTr.LoadInteractables(chestStates, chestLockStates);
        }
    }

    public void SaveInteractables()
    {
        InteractableTracker prefabTr = currentInstance.GetComponent<InteractableTracker>();

        if (prefabTr != null)
        {
            prefabTr.SaveInteractables(chestStates, chestLockStates);
        }
    }
}

