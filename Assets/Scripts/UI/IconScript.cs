using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;


public class IconScript : MonoBehaviour, /*IBeginDragHandler, IDragHandler, IEndDragHandler,*/ IPointerClickHandler
{
    //Parent Slot as this object is an Icon
    Slot ParentSlot;
    Canvas canvas;
    bool isDragging;
    Image image;
    InventoryBaseClass parentInventory;

    private void Awake()
    {
        ParentSlot = transform.parent.GetComponent<Slot>();
        canvas = GetComponent<Canvas>();
        parentInventory = GetComponentInParent<InventoryBaseClass>();
        image = GetComponent<Image>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (isDragging)
            if (parentInventory.visible)
                gameObject.transform.position = Input.mousePosition;
            else ResetItemPosition();
        image.raycastTarget = parentInventory.visible;

    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left)
        {
            if (!isDragging && !ParentSlot.IsSlotEmpty())
            {
                isDragging = true;
                canvas.sortingOrder += 1;
            }
            else
                DropItem();
        }
    }


    private void DropItem()
    {
        PointerEventData ped = new PointerEventData(null);

        //Set required parameters, in this case, mouse position
        ped.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast it
        EventSystem.current.RaycastAll(ped, results);

        //Remove all Icon results so it won't interfere our process
        results.RemoveAll(x => x.gameObject.tag.Equals("Icon"));

        if (results.Count <= 0 && !ParentSlot.transform.parent.name.Equals(Constants.VENDOR_INVENTORY) && !ParentSlot.transform.parent.name.Equals(Constants.LOOT_INVENTORY))
        {
            ConfirmationPanel.instance.CurrentState = ConfirmationPanel.ConfirmationPanelState.Delete;
            ConfirmationPanel.instance.DisplayConfirmationPanel(ParentSlot);

        }
        else
        {
            var DropTarget = results.Where((x) => x.gameObject.tag.Equals("VendorInventory") || x.gameObject.tag.Equals("PlayerInventory") || x.gameObject.tag.Equals("Slot")).FirstOrDefault();

            if (DropTarget.gameObject)
                if (!DropTarget.gameObject.GetComponentInParent<Looting>())
                {

                    if (DropTarget.gameObject.GetComponentInParent<PlayerInventory>())
                    {
                        Slot DropSlot = DropTarget.gameObject.GetComponent<Slot>();
                        if (gameObject.GetComponentInParent<Looting>())
                        {
                            //print("Got here");
                            if (PlayerInventory.instance.CanAddItem(ParentSlot.item))
                            {
                                PlayerInventory.instance.AddItem(ParentSlot.item, GetFirstEmptySlot(DropTarget.gameObject.GetComponentInParent<PlayerInventory>()));
                                EnemyData.CurrentLootingEnemy.LootInventory.RemoveItem(ParentSlot, ref EnemyData.CurrentLootingEnemy.AllPossibleItems, ref EnemyData.CurrentLootingEnemy.ChanceOfItemDrop);
                                ParentSlot.EmptySlot();
                            }
                            else MessageManager.instance.DisplayMessage("Not Enough Space in Inventory");
                        }
                        else if (gameObject.GetComponentInParent<VendorInventory>())
                        {
                            ConfirmationPanel.instance.CurrentState = ConfirmationPanel.ConfirmationPanelState.Buy;
                            ConfirmationPanel.instance.DisplayConfirmationPanel(ParentSlot);

                        }
                        else if (DropSlot && DropSlot.IsSlotEmpty())
                        {
                            DropTarget.gameObject.GetComponent<Slot>().FillSlot(ParentSlot.item, ParentSlot.quantity.text);

                            if (!ParentSlot.IsSlotEmpty())
                                ParentSlot.EmptySlot();

                        }
                    }
                    else if (DropTarget.gameObject.GetComponentInParent<VendorInventory>())
                    {
                        if (gameObject.GetComponentInParent<PlayerInventory>())
                        {
                            ConfirmationPanel.instance.CurrentState = ConfirmationPanel.ConfirmationPanelState.Sell;
                            ConfirmationPanel.instance.DisplayConfirmationPanel(ParentSlot);
                        }
                    }
                }
                else ResetItemPosition();
        }
        if (canvas.sortingOrder >= 2)
            canvas.sortingOrder -= 1;
        ResetItemPosition();

    }


    private Slot GetFirstEmptySlot(InventoryBaseClass inventory)
    {
        List<Slot> slots = inventory.GetAllSlots();
        foreach (Slot slot in slots)
        {
            if (slot.IsSlotEmpty())
                return slot;
        }

        return null;
    }

    private void ResetItemPosition()
    {
        isDragging = false;
        gameObject.transform.position = ParentSlot.transform.position;

    }
}
