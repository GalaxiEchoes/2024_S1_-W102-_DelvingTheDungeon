using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class ShopSlot_UI : MonoBehaviour, IPointerEnterHandler
{
    // Each UI slot stores image of item, cost, and slot containing item
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCost;
    [SerializeField] public ISlot assignedItemSlot;

    // Button on UI element so slots can be clicked/selected
    private Button button;

    // Getters
    public ISlot AssignedItemSlot => assignedItemSlot;
    public ShopDisplay parentDisplay { get; private set; }

    private void Awake()
    {
        // Reset the slot on awake
        ClearSlot();

        // Add listener to check when a slot has been clicked
        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

        // Set the parent display to the shop display
        parentDisplay = transform.parent.GetComponent<ShopDisplay>();
    }

    // Initialise slot value 
    public void Init(ISlot slot)
    {
        assignedItemSlot = slot;
        UpdateUISlot(slot);
    }

    // Update the slot to match current items data
    public void UpdateUISlot(ISlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.sprite;
            itemSprite.color = Color.white;

            itemCost.text = "$"+slot.ItemData.cost.ToString();
        }
        else
        {
            ClearSlot();
        }
    }

    public void UpdateUISlot()
    {
        if (assignedItemSlot != null)
        {
            UpdateUISlot(assignedItemSlot);
        }
    }

    // Empty slot as required
    public void ClearSlot()
    {
        assignedItemSlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCost.text = "";
    }

    // On click of button/ui element call the slotclicked method in shopdisplay class
    public void OnUISlotClick()
    {
        parentDisplay?.SlotClicked(this);
    }

    // On hover of button/ui element call the onhover method in shopdisplay class
    public void OnPointerEnter(PointerEventData eventData)
    {
        parentDisplay?.OnHover(this);
    }
}
