using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

[System.Serializable]

public class InventoryBaseClass : MonoBehaviour
{

    [SerializeField]
    protected List<Slot> slots;
    protected Dictionary<Item, int> items;

    protected CanvasGroup canvasGroup;
    public bool visible = false;

    public virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        HideInventory();
    }

    public virtual void Start()
    {
        items = new Dictionary<Item, int>();
        slots = new List<Slot>();
        slots.AddRange(gameObject.transform.GetComponentsInChildren<Slot>().ToList());
    }

    //Quantities update
    public virtual bool AddItem(Item item)
    {
        if (items.ContainsKey(item))
        {
            items[item] += 1;
            UpdateSlots(item);
            return true;
        }
        else if (CanAddItem(item))
        {
            items.Add(item, 1);
            AddInSlot(item);
            return true;
        }
        else MessageManager.instance.DisplayMessage("Not Enough Space in Inventory");
        return false;
    }

    public bool AddItem(Item item, Slot slot)
    {
        if (items.ContainsKey(item))
        {
            items[item] += 1;
            UpdateSlots(item);
            return true;
        }
        else if (CanAddItem(item))
        {
            items.Add(item, 1);
            AddInSlot(item, slot);
            return true;
        }
        else MessageManager.instance.DisplayMessage("Not Enough Space in Inventory");
        return false;
    }

    public bool AddItem(List<Item> SavedItems, List<int> SavedQuantities, List<int> SavedSlots)
    {
        if (SavedItems.Count == SavedQuantities.Count)
        {
            int SlotsIncrementer = 0;

            for (int i = 0; i < SavedItems.Count; i++)
            {
                for (int j = 0; j < SavedQuantities[i]; j++)
                    AddItem(SavedItems[i], slots[SavedSlots[SlotsIncrementer]]);
                SlotsIncrementer++;
            }
        }

        return false;
    }

    public virtual void RemoveItem(Slot slot)
    {
        if (items.ContainsKey(slot.item) && items[slot.item] > 1)
        {
            items[slot.item] -= 1;
            UpdateSlots(slot.item);
        }
        else
        {
            items.Remove(slot.item);
            slot.EmptySlot();
        }
    }

    //Remove item?
    protected virtual void AddInSlot(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsSlotEmpty())
            {
                slots[i].FillSlot(item);
                break;
            }
        }
    }
    protected void AddInSlot(Item item, Slot slot)
    {
        slot.FillSlot(item);
    }

    public void UpdateSlots(Item item)
    {
        foreach (Slot slot in slots)
        {
            if (slot.item == item)
            {
                slot.UpdateQuantity(items[item]);
            }
        }
    }

    public void EmptyAllSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsSlotEmpty())
                slots[i].EmptySlot();
        }

    }

    public void ShowInventory()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        visible = true;
    }

    public void HideInventory()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        visible = false;
    }

    public List<Slot> GetAllSlots()
    {
        return slots;
    }

    public List<int> GetAllOccupiedSlots()
    {
        List<int> resultSlots = new List<int>();
        for (int i = 0; i < slots.Count; i++)
            if (!slots[i].IsSlotEmpty())
                resultSlots.Add(i);

        return resultSlots;
    }

    public bool CanAddItem(Item item)
    {
        if (items.ContainsKey(item))
            return true;
        else return items.Keys.Count < slots.Count;
    }


}
