using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Looting : InventoryBaseClass
{
    public static Looting instance;

    public override void Awake()
    {
        if (instance != null)
        {
            Debug.LogError(Constants.LOOTING_SINGLETON);
            return;
        }
        instance = this;

        canvasGroup = transform.GetComponent<CanvasGroup>();
        slots = new List<Slot>();

        HideInventory();

    }

    public override void Start()
    {
        slots.AddRange(transform.GetComponentsInChildren<Slot>().ToList());
    }


    public void AddLootingItems(ref List<Item> allItems, ref List<int> allDropChances)
    {
        if (allItems.Count == allDropChances.Count)
        {
            this.EmptyAllSlots();
            for (int i = 0; i < allItems.Count; i++)
            {
                if (UnityEngine.Random.Range(0, 100) <= allDropChances[i])
                    AddInSlot(allItems[i]);
            }
        }
        else throw new System.Exception("Drop Array Length Does Not Match!!");
    }

    public void GatherAllItems(ref List<Item> allItems, ref List<int> allDropChances)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsSlotEmpty())
            {
                if (PlayerInventory.instance.AddItem(slots[i].item))
                {
                    if (allItems.Count > 0)
                    {
                        allItems.RemoveAt(0);
                        allDropChances.RemoveAt(0);
                    }
                    slots[i].EmptySlot();
                }
                else
                    break;

            }
        }
    }

    public void RemoveItem(Slot slot, ref List<Item> allItems, ref List<int> allDropChances)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] == slot.item)
            {
                allItems.RemoveAt(i);
                allDropChances.RemoveAt(i);
                break;
            }
        }
        slot.EmptySlot();
    }

}
