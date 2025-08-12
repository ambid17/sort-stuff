using UnityEngine;

public enum ItemType
{
    Powerup,
    Item,
    WallSkin,
    BowlSkin,
    Upgrade
}

public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public int cost;
    public Sprite icon;
    public bool isUnlocked;
}