using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor : NPC
{
    public List<Item> items;
    bool InsideTrigger = false;
    VendorInventory inventory;


    void Start()
    {
        inventory = GameObject.FindObjectOfType<VendorInventory>();
    }

    void Update()
    {
        if (InsideTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (!inventory.visible)
            {
                if(inventory.occupiedSlots!=items.Count)
                    AddItemsOnInventory();
                inventory.ShowInventory();
                PlayerInventory.instance.ShowInventory();
            }
            else
            {
                PlayerInventory.instance.HideInventory();
                inventory.HideInventory();

            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            MessageManager.instance.DisplayMessage("Press E to Open Shop");
            InsideTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            MessageManager.instance.KillMessage();
            InsideTrigger = false;
            inventory.HideInventory();
            PlayerInventory.instance.HideInventory();
            inventory.EmptyAllSlots();
            inventory.occupiedSlots=0;

        }
    }

    void AddItemsOnInventory()
    {
        foreach (Item item in items)
        {
            inventory.AddItem(item);
        }
    }
}
