using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendorInventory : InventoryBaseClass
{
    public int occupiedSlots;

    public override void Start()
    {
        base.Start();
        occupiedSlots=0;
    }

    protected override void AddInSlot(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsSlotEmpty())
            {
                slots[i].FillSlot(item,true);
                break;
            }
        }
    }

    public override bool AddItem(Item item)
    {
        this.AddInSlot(item);
        occupiedSlots++;
        return true;
    }
}
