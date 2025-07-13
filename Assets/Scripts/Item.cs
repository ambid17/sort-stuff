using UnityEngine;

public enum ItemType
{
    Powerup,
    Item,
    Skin,
    Unlock
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
