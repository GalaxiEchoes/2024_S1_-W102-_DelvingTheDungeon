using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class InventorySlot_UI : MonoBehaviour, IPointerEnterHandler
{
    // Each inventory slot stores image of item, amount in stack, and slot containing item
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private ISlot assignedInventorySlot;

    // Button on UI element so slots can be clicked/selected
    private Button button;

    // Getters
    public ISlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay { get; private set; }

    private void Awake()
    {
        // Reset the slot on awake
        ClearSlot();

        // Add listener to check when a slot has been clicked
        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

        // Set the parent display to the inventory display
        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();
    }

    // Initialise slot value 
    public void Init(ISlot slot)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    // Update the slot to match current items data
    public void UpdateUISlot(ISlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.sprite;
            itemSprite.color = Color.white;

            if (slot.StackSize > 1) itemCount.text = slot.StackSize.ToString();
            else itemCount.text = "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void UpdateUISlot()
    {
        if (assignedInventorySlot != null) UpdateUISlot(assignedInventorySlot);
    }

    // Empty slot as required
    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = "";
    }

    // On click of button/ui element call the slotclicked method in inventorydisplay class
    public void OnUISlotClick()
    {
        ParentDisplay?.SlotClicked(this);
    }

    // On hover of button/ui element call the onhover method in inventorydisplay class
    public void OnPointerEnter(PointerEventData eventData)
    {
        ParentDisplay?.OnHover(this);
    }
}
