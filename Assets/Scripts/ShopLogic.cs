using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ShopLogic : MonoBehaviour
{
    public InventoryHolder shopInventory;
    public GameObject potentialInventory;
    //public GameObject[] inventoryItems;

    public void Open()
    {
        Debug.Log("Shop has been opened.");
    }

    public void Close()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        //populate the shop
        GameObject go = Instantiate(potentialInventory, Vector3.zero, Quaternion.Euler(0, 0, 0));
        ItemPickUp ipu = go.GetComponent<ItemPickUp>();

        if (shopInventory.InventorySystem.AddToInventory(ipu.ItemData, 1))
        {
            Destroy(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
