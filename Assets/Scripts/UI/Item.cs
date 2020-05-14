using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewItem", menuName = "Create Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite InventoryIcon;
    public float SellingPrice;
    public float BuyingPrice;

    [SerializeField]
    public Statistics ItemStats;
    //
    //
    //
    //
    //ADD STATS TO THE ITEMS
    //
    //
    //
}
