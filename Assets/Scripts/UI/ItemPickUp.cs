using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
               // print("YOU INTERACTED WITH " + transform.name);
                //Show some UI for Interacting or Picking up

                Inventory.instance.AddItem(this.item);
                Destroy(gameObject);
            }
        }
    }
}
