using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Looting : InventoryBaseClass
{
    public List<Item> AllPossibleItems;
    public List<int> ChanceOfItemDrop;
    public static EnemyData CurrentEnemy;

    EnemyData enemyData;
    bool CanLoot = false;
    bool happened = false;
    GameObject inventory;


    public override void Awake()
    {
        inventory = GameObject.Find(Constants.LOOT_INVENTORY);
        canvasGroup = inventory.transform.parent.GetComponent<CanvasGroup>();
        enemyData = GetComponent<EnemyData>();
        slots = new List<Slot>();

        HideInventory();

    }

    public override void Start()
    {
        slots.AddRange(inventory.transform.GetComponentsInChildren<Slot>().ToList());
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.L) && CanLoot)
        {
            Target.instance.gameObject.SetActive(canvasGroup.alpha==1);
            if (enemyData != CurrentEnemy)
            {
                AddLootingItems();
                CurrentEnemy = enemyData;
            }
            if (canvasGroup.alpha == 1) HideInventory();
            else if (canvasGroup.alpha == 0) ShowInventory();
            // PlayerData.instance.ToogleLoot();
        }
        else if (Input.GetKeyDown(KeyCode.R) && !enemyData.Alive && CanLoot)
            GatherAllItems();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerData>() && other.GetComponent<PlayerData>().Alive)
        {
            CanLoot = true;
            other.GetComponent<PlayerData>().AbleToLoot = CanLoot;
            MessageManager.instance.DisplayMessage("Press L to Loot", 5);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerData>() && other.GetComponent<PlayerData>().Alive)
        {
            CanLoot = false;
            other.GetComponent<PlayerData>().AbleToLoot = CanLoot;
            MessageManager.instance.KillMessage();
        }
    }

    public bool IsLooting()
    {
        return CanLoot;
    }

    public void GatherAllItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsSlotEmpty())
            {
                PlayerInventory.instance.AddItem(slots[i].item);

                if (AllPossibleItems.Count > 0)
                {
                    AllPossibleItems.RemoveAt(0);
                    ChanceOfItemDrop.RemoveAt(0);
                }
                slots[i].EmptySlot();
            }
        }
    }


    public void AddLootingItems()
    {
        if (AllPossibleItems.Count == ChanceOfItemDrop.Count)
        {
            this.EmptyAllSlots();
            for (int i = 0; i < AllPossibleItems.Count; i++)
            {
                if (UnityEngine.Random.Range(0, 100) <= ChanceOfItemDrop[i])
                    AddInSlot(AllPossibleItems[i]);
            }
        }
        else throw new System.Exception("Drop Array Length Does Not Match!!");
    }

    public override void RemoveItem(Slot slot)
    {
        for (int i = 0; i < AllPossibleItems.Count; i++)
        {
            if (AllPossibleItems[i] == slot.item)
            {
                AllPossibleItems.RemoveAt(i);
                ChanceOfItemDrop.RemoveAt(i);
                break;
            }
        }
        slot.EmptySlot();
    }

}
