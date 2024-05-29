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
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCost;
    [SerializeField] private ISlot assignedItemSlot;

    private Button button;

    public ISlot AssignedItemSlot => assignedItemSlot;
    public ShopDisplay parentDisplay { get; private set; }

    private void Awake()
    {
        ClearSlot();

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

        parentDisplay = transform.parent.GetComponent<ShopDisplay>();
    }

    public void Init(ISlot slot)
    {
        assignedItemSlot = slot;
        UpdateUISlot(slot);
    }

    //update the slot to match current items data
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

    //empty slot as required
    public void ClearSlot()
    {
        assignedItemSlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCost.text = "";
    }

    //On click of button/ui element call the slotclicked method in inventorydisplay class
    public void OnUISlotClick()
    {
        parentDisplay?.SlotClicked(this);
    }

    //On hover of button/ui element call the onhover method in inventorydisplay class
    public void OnPointerEnter(PointerEventData eventData)
    {
        parentDisplay?.OnHover(this);
    }
}
