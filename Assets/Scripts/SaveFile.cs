using System.Collections.Generic;

public class SaveFile
{
    public List<string> unlockedItemNames;
    public List<string> unlockedPowerUpNames;
    public List<string> unlockedSkinNames;
    public List<string> unlockedEnvironmentNames;
    public List<string> unlockedUpgradeNames;
    public string selectedEnvironment;
    public string selectedBowlSkin;
    public int currency;

    public SaveFile()
    {
        unlockedItemNames = new List<string>();
        unlockedPowerUpNames = new List<string>();
        unlockedSkinNames = new List<string>();
        unlockedEnvironmentNames = new List<string>();
        unlockedUpgradeNames = new List<string>();
        selectedEnvironment = string.Empty;
        selectedBowlSkin = string.Empty;
        currency = 0;
    }
}
