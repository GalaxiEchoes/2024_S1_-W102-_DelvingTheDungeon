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

    private void Awake()
    {
        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse over Item Slot");
    }

    //On click of button/ui element
    public void OnUISlotClick()
    {
        Debug.Log("Item slot clicked");
    }
}
