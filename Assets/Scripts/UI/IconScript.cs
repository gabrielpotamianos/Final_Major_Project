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

    private void Awake()
    {
        ParentSlot = transform.parent.GetComponent<Slot>();
        canvas = GetComponent<Canvas>();
    }



    void FixedUpdate()
    {
        if (isDragging)
            gameObject.transform.position = Input.mousePosition;

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

        if (results.Count <= 0)
        {
            ConfirmationPanel.instance.CurrentState=ConfirmationPanel.ConfirmationPanelState.Delete;
            ConfirmationPanel.instance.DisplayConfirmationPanel(ParentSlot);
            
        }

        var possibleSlot = results.FirstOrDefault(x => x.gameObject.tag.Equals("Slot"));

        Slot resultSlot;
        if (possibleSlot.gameObject && !possibleSlot.gameObject.transform.parent.name.Equals(Constants.LOOT_INVENTORY) && possibleSlot.gameObject.GetComponent<Slot>().IsSlotEmpty())
        {
            resultSlot = possibleSlot.gameObject.GetComponent<Slot>();
            if (resultSlot.transform.parent.name.Equals(Constants.PLAYER_INVENTORY))
            {
                if (ParentSlot.transform.parent.name.Equals(Constants.LOOT_INVENTORY))
                {
                    PlayerInventory.instance.AddItem(ParentSlot.item, resultSlot);
                    Looting.CurrentEnemy.LootInventory.RemoveItem(ParentSlot);
                    ParentSlot.EmptySlot();
                }
                else if (ParentSlot.transform.parent.name.Equals(Constants.VENDOR_INVENTORY))
                {
                    ConfirmationPanel.instance.CurrentState = ConfirmationPanel.ConfirmationPanelState.Buy;
                    ConfirmationPanel.instance.DisplayConfirmationPanel(ParentSlot);
                }
                else
                {
                    resultSlot.FillSlot(ParentSlot.item, ParentSlot.quantity.text);

                    if (!ParentSlot.IsSlotEmpty())
                        ParentSlot.EmptySlot();
                }

            }
            else if (resultSlot.transform.parent.name.Equals(Constants.VENDOR_INVENTORY))
            {
                if (ParentSlot.transform.parent.name.Equals(Constants.PLAYER_INVENTORY))
                {
                    ConfirmationPanel.instance.CurrentState = ConfirmationPanel.ConfirmationPanelState.Sell;
                    ConfirmationPanel.instance.DisplayConfirmationPanel(ParentSlot);
                }
            }
        }
        else
        {
            isDragging = false;
            gameObject.transform.position = ParentSlot.transform.position;
        }
        gameObject.transform.position = ParentSlot.transform.position;
        if (canvas.sortingOrder >= 2)
            canvas.sortingOrder -= 1;
        isDragging = false;


    }
}
