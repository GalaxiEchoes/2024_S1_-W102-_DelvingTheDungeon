using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot_UI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private ISlot assignedInventorySlot;

    private Button button;

    public ISlot AssignedInventorySlot => assignedInventorySlot;
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
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    //update the slot to match current items data
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

    //empty slot as required
    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = "";
    }

    //On click of button/ui element call the slotclicked method in inventorydisplay class
    public void OnUISlotClick()
    {
        ParentDisplay?.SlotClicked(this);
    }

    //On hover of button/ui element call the onhover method in inventorydisplay class
    public void OnPointerEnter(PointerEventData eventData)
    {
        ParentDisplay?.OnHover(this);
        Debug.Log("The cursor entered the selectable UI element.");
    }
}
