using UnityEngine;

public enum PowerupType
{
    Bomb,
    Magnet
}

[CreateAssetMenu(fileName = "New PowerUp", menuName = "Scriptable Objects/PowerUp")]
public class Powerup : Item
{
    public PowerupType powerUpType;
}
