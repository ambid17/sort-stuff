using UnityEngine;

public enum ItemType
{
    Powerup,
    Item,
    WallSkin,
    BowlSkin,
    Upgrade
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public int cost;
    public Sprite icon;
    public ItemType itemType;
    public SortableObject sortableObject;
}

[CreateAssetMenu(fileName = "New Skin", menuName = "Skin")]
public class Skin : Item
{
    public Material material;
}
