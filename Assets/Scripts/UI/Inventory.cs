using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

[System.Serializable]

public class Inventory : MonoBehaviour
{
    [SerializeField]
    public GameObject inventory;
    private GameObject player;
    private PlayerData playerData;

    [SerializeField]
    public List<Slot> slots;

    #region Singleton
    public static Inventory instance;

    private void Awake()
    {
        inventory.SetActive(false);
        if (instance != null)
        {
            Debug.LogError("You have MORE than ONE INVENTORY instances!");
            return;
        }
        instance = this;
    }
    #endregion

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(SelectCharacter.SelectedGameObject);
        player = GameObject.FindGameObjectWithTag("Warrior");
        playerData = player.GetComponent<PlayerData>();

        slots = new List<Slot>();
        slots.AddRange(inventory.transform.GetComponentsInChildren<Slot>().ToList());
        inventory.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && playerData.defaultStats.Alive)
            inventory.SetActive(!inventory.activeSelf);


    }

    //Quantities update
    public void AddItem(Item item)
    {
        AddInSlot(item);
    }

    public void RemoveItem(Slot slot)
    {
        // Instantiate(item, player.transform.position + player.transform.forward,Space.World);
        slot.EmptySlot();
    }

    //Remove item?
    public void AddInSlot(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            //Crash on slot with many items (quantities)!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (slots[i].IsSlotEmpty())
            {
                slots[i].FillSlot(item);
                break;
            }
        }
    }


}
