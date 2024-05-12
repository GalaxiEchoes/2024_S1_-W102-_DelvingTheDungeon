using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InteractableTracker : MonoBehaviour
{
    public List<ChestLogic> chests;

    public void LoadInteractables(List<bool> chestStates)
    {
        for(int i = 0; i < chestStates.Count; i++)
        {
            chests[i].SetIsOpen(chestStates[i]);
        }
    }

    public void SaveInteractables(List<bool> chestStates)
    {
        chestStates.Clear();
        chestStates.Capacity = chests.Count;

        for (int i = 0; i < chests.Count; i++)
        {
            chestStates.Add(chests[i].IsOpen == true);
        }
    }
}
