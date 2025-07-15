using System.Collections.Generic;

public class SaveFile
{
    public List<string> unlockedItemNames;
    public List<string> unlockedPowerUpNames;
    public List<string> unlockedSkinNames;
    public List<string> unlockedUpgradeNames;
    public string selectedWallSkin;
    public string selectedBowlSkin;
    public int currency;

    public SaveFile()
    {
        unlockedItemNames = new List<string>();
        unlockedPowerUpNames = new List<string>();
        unlockedSkinNames = new List<string>();
        unlockedUpgradeNames = new List<string>();
        selectedWallSkin = string.Empty;
        selectedBowlSkin = string.Empty;
        currency = 0;
    }
}
