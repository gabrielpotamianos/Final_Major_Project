using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor : NPC
{
    public List<Item> items;

    bool InsideTrigger = false;
    VendorInventory inventory;
    GameObject player;

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
                if (inventory.occupiedSlots != items.Count)
                    AddItemsOnInventory();
                inventory.ShowInventory();
                transform.LookAt(player.transform, transform.up);
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
            player = other.gameObject;
            MessageManager.instance.DisplayMessage(Constants.VENDOR_OPEN_SHOP_MSG);
            InsideTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Force to close the message
            MessageManager.instance.KillMessage();
            
            //Disable Interaction Box
            InsideTrigger = false;

            //Hide inventories
            inventory.HideInventory();
            PlayerInventory.instance.HideInventory();

            //Empty all slots
            inventory.EmptyAllSlots();

            //Reset the occupied slots
            inventory.occupiedSlots = 0;

            //null the reference
            player = null;

            //Close any panels that might have been left opened
            ConfirmationPanel.instance.Cancel();
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
