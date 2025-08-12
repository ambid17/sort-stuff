using UnityEngine;

public enum GameplayUpgradeType
{
    GoldInjection,
    RainingMoney,
    StickyFingers,
    BonusBarDuration,
    BonusBarTier,
    BonusBarSpeed
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class Upgrade : Item
{
    public GameplayUpgradeType upgradeType;
}
