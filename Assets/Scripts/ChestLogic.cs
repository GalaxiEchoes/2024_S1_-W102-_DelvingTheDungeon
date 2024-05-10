using System.Collections;
using System.Collections.Generic;
using Random = System.Random;
using UnityEngine;
using UnityEngine.UIElements;
using System;

[Serializable]
public class ChestLogic : MonoBehaviour
{
    [Header("Rotation Configs")]
    [SerializeField] private float RotationAmount = 75f;
    [SerializeField] private Vector3 StartRotation;
    [SerializeField] private Coroutine AnimationCoroutine;
    [SerializeField] private bool IsRunning = false;
    [SerializeField] private Random rand;
    [NonSerialized] private InventoryHolder inventoryHolder;

    [SerializeField] public bool IsOpen = false;
    [SerializeField] GameObject[] possibleItems;
    [SerializeField] GameObject[] items;

    private void Awake()
    {
        rand = new Random();
        StartRotation = transform.localRotation.eulerAngles;
        items = new GameObject[4];

        float chance;
        int index = 0;
        
        do
        {
            chance = (float)rand.NextDouble();
            int nextItem = rand.Next(0, possibleItems.Length);
            items[index] = possibleItems[nextItem];
            index++;

        } while (chance <= 0.8f && index < 3);
    }
    
    public void SetIsOpen(bool isOpen)
    {
        IsOpen = isOpen;
        transform.localRotation = Quaternion.Euler(-RotationAmount, 0, 0); 
    }

    public void Open(InventoryHolder inventory)
    {
        inventoryHolder = inventory;
        if (!IsRunning && !IsOpen)
        {
            IsRunning = true;
            AnimationCoroutine = StartCoroutine(DoRotationOpen());
        }
    }

    private IEnumerator DoRotationOpen()
    { 
        Quaternion startRotation = transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(new(startRotation.x - RotationAmount, 0, 0));

        float time = 0;
        while(time < 1)
        {
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, time);
            yield return null;
            time+= Time.deltaTime;
        }

        EventAfterRotation();
    }

    void EventAfterRotation()
    {
        IsOpen = true;
        foreach (GameObject item in items)
        {
            if(item != null)
            {
                GameObject go = Instantiate(item, Vector3.zero, Quaternion.Euler(0, 0, 0));
                ItemPickUp ipu = go.GetComponent<ItemPickUp>();

                if (inventoryHolder.InventorySystem.AddToInventory(ipu.ItemData, 1))
                {
                    Destroy(go);
                }
                //Else Close if no space
            }
        }
    }
}
