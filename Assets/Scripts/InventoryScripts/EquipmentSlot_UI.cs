using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot_UI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private ISlot assignedEquipmentSlot;

    private Button button;

    public ISlot AssignedEquipmentlot => assignedEquipmentSlot;
    public InventoryDisplay ParentDisplay { get; private set; }

    private void Awake()
    {
        ClearSlot();

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();
    }

    public void Init(ISlot slot)
    {
        assignedEquipmentSlot = slot;
        UpdateUISlot(slot);
    }

    public void UpdateUISlot(ISlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.sprite;
            itemSprite.color = Color.white;

            itemCount.text = "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void UpdateUISlot()
    {
        if (assignedEquipmentSlot != null) UpdateUISlot(assignedEquipmentSlot);
    }

    public void ClearSlot()
    {
        assignedEquipmentSlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = "";
    }

    public void OnUISlotClick()
    {
        ParentDisplay?.SlotClicked(this);
    }
}
