using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class IconScript :MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //Parent Slot as this object is an Icon
    Slot ParentSlot;

    private void Awake()
    {
        ParentSlot = transform.parent.GetComponent<Slot>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Canvas>().sortingOrder += 1;
    }


    public void OnDrag(PointerEventData eventData)
    {
        //Update Position on dragging the Icon
        transform.position = Input.mousePosition;
    }



    public void OnEndDrag(PointerEventData eventData)
    {

        //Destroy object if no results are retrieved
        bool destroyObject = true;


        //Create the PointerEventData with null for the EventSystem
        PointerEventData ped = new PointerEventData(null);

        //Set required parameters, in this case, mouse position
        ped.position = Input.mousePosition;

        //Create list to receive all results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast it
        EventSystem.current.RaycastAll(ped, results);

        //Remove all Icon results so it won't interfere our process
        results.RemoveAll(x => x.gameObject.tag.Equals("Icon"));



        //Check results
        foreach (RaycastResult result in results)
        {
            //If it is the inventory we are raycasting - do not destroy de object -
            if (result.gameObject.tag.Equals("Inventory"))
            {
                destroyObject = false;
            }

            //If it is a slot means changing position in inventory
            else if (result.gameObject.tag.Equals("Slot"))
            {
                //Get the Result Slot Component
                Slot SlotResult = result.gameObject.GetComponent<Slot>();

                //Check if the new slot is empty
                if (SlotResult.IsSlotEmpty())
                {
                    //Remove item from parent slot and add to the new one
                    SlotResult.FillSlot(ParentSlot.item);
                    ParentSlot.EmptySlot();
                }

                //Do not destroy object
                destroyObject = false;
            }
        }


        //If no results were gathered - Remove Object from parent slot
        if (destroyObject)
            Inventory.instance.RemoveItem(ParentSlot);

        // Else just snap it to the slot position
        transform.localPosition = Vector3.zero;

        GetComponent<Canvas>().sortingOrder -= 1;


    }

}
