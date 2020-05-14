using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Linq;

public class BaseHoover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static GameObject currentSelectedObject;
    public string Message;
    GameObject tooltip;
    Text ToolTipText;
    CanvasGroup canvasGroup;
    Slot slot;



    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        tooltip = GameObject.FindGameObjectWithTag("ToolTip");
        ToolTipText = tooltip.GetComponentInChildren<Text>();
        canvasGroup = tooltip.GetComponent<CanvasGroup>();
        slot = gameObject.GetComponent<Slot>();
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (currentSelectedObject == gameObject)
        {
            if (tooltip && tooltip.activeSelf)
                tooltip.transform.position = Input.mousePosition;
        }
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponentInParent<InventoryBaseClass>().visible && !slot.IsSlotEmpty())
        {
            currentSelectedObject = gameObject;
            canvasGroup.GetComponent<Canvas>().sortingOrder += 1;

            //ToolTipText.text += (slot.item) ? slot.item.name : Message;
            bool IsThisVendor = eventData.hovered.Where((x) => x.gameObject.GetComponent<VendorInventory>()).FirstOrDefault();

            GetItemStats(slot.item, IsThisVendor);
            canvasGroup.alpha = 1;
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (GetComponentInParent<InventoryBaseClass>().visible && !slot.IsSlotEmpty())
        {
            canvasGroup.alpha = 0;
            ToolTipText.text = "";
            currentSelectedObject = null;
            canvasGroup.GetComponent<Canvas>().sortingOrder -= 1;
        }
    }

    void GetItemStats(Item item, bool IsItVendor)
    {
        var a = item.ItemStats.GetStatisticsDictionary();
        foreach (var b in a)
        {
            if (b.Value > 0)
            {
                ToolTipText.text += b.Key + ":  " + b.Value;
                ToolTipText.text += Environment.NewLine;
            }
        }
        if (IsItVendor)
            ToolTipText.text += Environment.NewLine + "Buying price:    " + item.BuyingPrice;
        else ToolTipText.text += Environment.NewLine + "Selling price:    " + item.SellingPrice;


    }
}
