using UnityEngine;

public enum ItemType
{
    Powerup,
    Item
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int cost;
    public Sprite icon;
    public int id;
    public ItemType itemType;
}
