using System.Collections;
using System.Collections.Generic;
using Random = System.Random;
using UnityEngine;
using UnityEngine.UIElements;

public class ChestLogic : MonoBehaviour
{
    [Header("Rotation Configs")]
    [SerializeField] private float RotationAmount = 75f;

    private Vector3 StartRotation;

    private Coroutine AnimationCoroutine;
    bool IsRunning = false;

    [SerializeField] GameObject[] possibleItems;
    [SerializeField] GameObject[] items;
    Random rand;
    InventoryHolder inventoryHolder;

    private void Awake()
    {
        rand = new Random();
        StartRotation = transform.rotation.eulerAngles;
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

    public void Open(Vector3 UserPosition, InventoryHolder inventory)
    {
        inventoryHolder = inventory;
        if (!IsRunning)
        {
            IsRunning = true;
            AnimationCoroutine = StartCoroutine(DoRotationOpen());
        }
    }

    private IEnumerator DoRotationOpen()
    { 
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(new(startRotation.x - RotationAmount, 180, 0));

        float time = 0;
        while(time < 1)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, time);
            yield return null;
            time+= Time.deltaTime;
        }

        EventAfterRotation();
    }

    void EventAfterRotation()
    {
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
                //Else Close
            }
        }
    }
}
