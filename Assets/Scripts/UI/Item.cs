using UnityEngine;
using UnityEngine.UI;




[CreateAssetMenu(fileName = "NewItem", menuName = "Create Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    [SerializeField]
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
