using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewItem",menuName ="Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite InventoryIcon;
    public Object GameObjectPrefab;
}
