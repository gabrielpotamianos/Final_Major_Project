using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looting : Inventory
{
    public List<Item> AllPossibleDropItems;
    public List<int> ChanceOfItemDrop;

    EnemyCombat creature;
    bool looting = false;

    public override void Awake()
    {        
        creature = GetComponent<EnemyCombat>();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {

        if (Input.GetKeyDown(KeyCode.L) && !creature.enemyData.Alive && looting)
        {
            AddLootingItems();
            inventory.SetActive(!inventory.activeSelf);
            PlayerData.instance.ToogleLoot();
        }
        else if (Input.GetKeyDown(KeyCode.R) && !creature.enemyData.Alive && looting)
            GatherAllItems();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerData>() && other.GetComponent<PlayerData>().Alive)
        {
            looting = true;
            other.GetComponent<PlayerData>().AbleToLoot = looting;
            MessageManager.instance.DisplayMessage("Press L to Loot",5);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerData>() && other.GetComponent<PlayerData>().Alive)
        {
            looting = false;
            other.GetComponent<PlayerData>().AbleToLoot = looting;
            MessageManager.instance.KillMessage();
        }
    }

    public bool IsLooting()
    {
        return looting;
    }

    public void GatherAllItems()
    {
        for(int i=0;i<slots.Count;i++)
        {
            if (!slots[i].IsSlotEmpty())
            {
                Inventory.instance.AddInSlot(slots[i].item);

                if (AllPossibleDropItems.Count>0)
                {

                    AllPossibleDropItems.RemoveAt(0);
                    ChanceOfItemDrop.RemoveAt(0);
                }
                slots[i].EmptySlot();
            }
        }
    }


    public void AddLootingItems()
    {
        if (AllPossibleDropItems.Count == ChanceOfItemDrop.Count)
        {
            EmptyAllSlots();
            for (int i = 0; i < AllPossibleDropItems.Count; i++)
            {
                if (Random.Range(0, 100) <= ChanceOfItemDrop[i])
                    AddInSlot(AllPossibleDropItems[i]);
            }
        }
        else throw new System.Exception("Drop Array Length Does Not Match!!");

    }

    public override void RemoveItem(Slot slot)
    {
        for(int i=0;i<AllPossibleDropItems.Count;i++)
        {
            if (AllPossibleDropItems[i] == slot.item)
            {
                AllPossibleDropItems.RemoveAt(i);
                ChanceOfItemDrop.RemoveAt(i);
                break;
            }
        }
        base.RemoveItem(slot);
    }

}
