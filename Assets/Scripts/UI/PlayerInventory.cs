using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

[System.Serializable]

public class PlayerInventory : InventoryBaseClass
{
    PlayerData playerData;
    public List<Item> item;

    Text GoldText;


    #region Singleton
    public static PlayerInventory instance;

    public override void Awake()
    {
        base.Awake();
        if (instance != null)
        {
            Debug.LogError("You have MORE than ONE INVENTORY instances!");
            return;
        }
        instance = this;
    }
    #endregion

    public override void Start()
    {
        items = new Dictionary<Item, int>();
        slots = new List<Slot>();
        slots.AddRange(gameObject.transform.GetChild(0).GetComponentsInChildren<Slot>().ToList());
        playerData = GameObject.FindObjectOfType<PlayerData>();
        GoldText = transform.GetChild(2).transform.GetChild(1).GetComponent<Text>();


    }

    public void Update()
    {
        GoldText.text = playerData.gold.ToString();
        if (Input.GetKeyDown(KeyCode.I) && playerData.Alive)
        {
            if (canvasGroup.alpha == 1) HideInventory();
            else if (canvasGroup.alpha == 0) ShowInventory();
        }
    }

    public Dictionary<Item, int> GetItems()
    {
        return items;
    }
}
