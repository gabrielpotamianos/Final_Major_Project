using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Slot : MonoBehaviour
{
    [SerializeField]
    public Item item;
    public Text quantity;
    public bool empty;
    public Image icon;
    IconScript iconScript;

    public void Awake()
    {
        icon = transform.GetChild(0).GetComponent<Image>();
        quantity = icon.transform.GetChild(0).GetComponent<Text>();
        quantity.gameObject.SetActive(false);
        iconScript = icon.GetComponent<IconScript>();
    }

    public void FillSlot(Item item)
    {
        iconScript.enabled = true;
        icon.sprite = item.InventoryIcon;
        this.item = item;
        Color setAlpha = icon.color;
        setAlpha.a = 1;
        icon.color = setAlpha;
        this.quantity.gameObject.SetActive(true);
        UpdateQuantity(1);
        empty = false;
    }

    public void FillSlot(Item item, bool noQuantity)
    {
        iconScript.enabled = true;
        icon.sprite = item.InventoryIcon;
        this.item = item;
        Color setAlpha = icon.color;
        setAlpha.a = 1;
        icon.color = setAlpha;
        empty = false;
    }
    public void FillSlot(Item item, string Quantity)
    {
        iconScript.enabled = true;
        icon.sprite = item.InventoryIcon;
        this.item = item;
        Color setAlpha = icon.color;
        setAlpha.a = 1;
        icon.color = setAlpha;
        this.quantity.gameObject.SetActive(true);
        UpdateQuantity(Quantity);
        empty = false;
    }

    public void EmptySlot()
    {
        Color setAlpha = icon.color;
        setAlpha.a = 0;
        icon.color = setAlpha;
        icon.sprite = null;
        item = null;
        quantity.text = " ";
        this.quantity.gameObject.SetActive(false);
        empty = true;
        iconScript.enabled = false;

    }

    public bool IsSlotEmpty()
    {
        if (item != null)
            return false;
        else return true;
    }

    public void UpdateQuantity(int quantity)
    {
        this.quantity.text = quantity.ToString();
    }

    public void UpdateQuantity(string quantity)
    {
        this.quantity.text = quantity;
    }
}
