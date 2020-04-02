using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Slot : MonoBehaviour
{
    [SerializeField]
    public Item item;


    public Image icon;

    public void Awake()
    {
        icon = transform.GetChild(0).GetComponent<Image>();
    }

    public void FillSlot(Item item)
    {
        if (icon)
            icon.sprite = item.InventoryIcon;
        this.item = item;
        Color setAlpha = icon.color;
        setAlpha.a = 1;
        icon.color = setAlpha;
        //Add quantity
    }

    public void EmptySlot()
    {
        Color setAlpha = icon.color;
        setAlpha.a = 0;
        icon.color = setAlpha;
        icon.sprite = null;
        item = null;
    }

    public bool IsSlotEmpty()
    {
        if (item!=null)
            return false;
        else return true;
    }
}
