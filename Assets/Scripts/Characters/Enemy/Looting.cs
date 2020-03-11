using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looting : Inventory
{
    public Item[] AllPossibleDropItems;
    public int[] ChanceOfItemDrop;

    Enemy creature;
    bool looting = false;

    public override void Awake()
    {
        creature = GetComponent<Enemy>();
    }

    public override void Start()
    {
        base.Start();
        if (AllPossibleDropItems.Length == ChanceOfItemDrop.Length)
        {
            for (int i = 0; i < AllPossibleDropItems.Length; i++)
            {
                if (Random.Range(0, 100) <= ChanceOfItemDrop[i])
                    AddInSlot(AllPossibleDropItems[i]);
            }
        }
        else throw new System.Exception("Drop Array Length Does Not Match!!");

    }

    public override void Update()
    {
        if (!creature.defaultStats.Alive)
            GetComponent<Collider>().isTrigger = true;

        print(creature.defaultStats.Alive);


        if (Input.GetKeyDown(KeyCode.L) && !creature.defaultStats.Alive && looting)
            inventory.SetActive(!inventory.activeSelf);
        else if (Input.GetKeyDown(KeyCode.R) && !creature.defaultStats.Alive && looting)
            GatherAllItems();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCombat>())
        {
            looting = true;
            other.GetComponent<PlayerCombat>().AbleToLoot = looting;
            MessageManager.instance.DisplayMessage("Press L to Loot",5);
        }
    }
    //
    //Player dies looting becomes available 
    //
    //
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerCombat>())
        {
            looting = false;
            other.GetComponent<PlayerCombat>().AbleToLoot = looting;
            MessageManager.instance.KillMessage();
        }
    }

    public bool IsLooting()
    {
        return looting;
    }

    public void GatherAllItems()
    {
        foreach(Slot slot in slots)
        {
            if (!slot.IsSlotEmpty())
            {
                Inventory.instance.AddInSlot(slot.item);
                slot.EmptySlot();
            }
        }
    }
}
