using UnityEngine;

public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public int cost;
    public Sprite icon;
    public bool isUnlocked;
}