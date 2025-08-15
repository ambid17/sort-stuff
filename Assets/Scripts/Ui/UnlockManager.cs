using CaosCreations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UnlockManager : Singleton<UnlockManager>
{
    public SaveFile fileStateToSave;
    [Header("scriptable objects")]
    public List<Powerup> powerUpSOs;
    public List<SortableItem> itemSOs;
    public List<Skin> skinSOs;
    public List<Upgrade> upgradeSOs;
    public List<Environment> environmentSOs;
    [Header("selected skins")]
    public Skin selectedBowlSkin => skinSOs.FirstOrDefault(skin => skin.itemName == fileStateToSave.selectedBowlSkin);
    public Environment selectedEnvironment => environmentSOs.FirstOrDefault(env => env.itemName == fileStateToSave.selectedEnvironment);

    protected override void Initialize()
    {
        LoadUnlocks();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public bool TryUnlock(Item item)
    {
        if (fileStateToSave.currency < item.cost)
        {
            return false;
        }

        fileStateToSave.currency -= item.cost;
        item.isUnlocked = true;
        switch (item)
        {
            case Upgrade upgrade:
                ApplyUpgrade(upgrade);
                break;
        }
        Save();
        return true;
    }

    public void AddCurrency(int currency)
    {
        fileStateToSave.currency += currency;
        UiManager.Instance.hudPanel.currencyText.text = $"{fileStateToSave.currency}";
    }

    public void Save()
    {
        try
        {
            fileStateToSave.unlockedItemNames = itemSOs.Where(i => i.isUnlocked).Select(i => i.itemName).ToList();
            fileStateToSave.unlockedPowerUpNames = powerUpSOs.Where(i => i.isUnlocked).Select(i => i.itemName).ToList();
            fileStateToSave.unlockedSkinNames = skinSOs.Where(i => i.isUnlocked).Select(i => i.itemName).ToList();
            fileStateToSave.unlockedEnvironmentNames = environmentSOs.Where(i => i.isUnlocked).Select(i => i.itemName).ToList();
            fileStateToSave.unlockedUpgradeNames = upgradeSOs.Where(i => i.isUnlocked).Select(i => i.itemName).ToList();
            var fileData = JsonConvert.SerializeObject(fileStateToSave);
            string filePath = Path.Combine(Application.persistentDataPath, "unlocks.json");

            File.WriteAllText(filePath, fileData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving: {e.Message}\n{e.StackTrace}");
        }
    }

    void LoadUnlocks()
    {
        fileStateToSave = new SaveFile();
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, "unlocks.json");
            if (File.Exists(filePath))
            {
                var fileContents = File.ReadAllText(filePath);
                fileStateToSave = JsonConvert.DeserializeObject<SaveFile>(fileContents);
                LoadItems();
                LoadPowerups();
                LoadSkins();
                LoadEnvironments();
                LoadUpgrades();
            }
            else
            {
                Debug.LogWarning("Unlocks file not found, creating a new one.");
            }

            UiManager.Instance.hudPanel.currencyText.text = $"{fileStateToSave.currency}";
            UiManager.Instance.shopPanel.currencyText.text = $"{fileStateToSave.currency}";
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading: {e.Message}\n{e.StackTrace}");
        }
    }

    void LoadItems()
    {
        foreach (var name in fileStateToSave.unlockedItemNames)
        {
            var item = itemSOs.FirstOrDefault(i => i.itemName == name);
            item.isUnlocked = true;
        }
    }

    void LoadPowerups()
    {
        foreach (var name in fileStateToSave.unlockedPowerUpNames)
        {
            var powerup = powerUpSOs.FirstOrDefault(i => i.itemName == name);
            powerup.isUnlocked = true;
        }
    }

    public void LoadSkins()
    {
        foreach (var name in fileStateToSave.unlockedSkinNames)
        {
            var skin = skinSOs.FirstOrDefault(i => i.itemName == name);
            skin.isUnlocked = true;
            if (skin.itemName == fileStateToSave.selectedBowlSkin)
            {
                ApplySkin(skin);
            }
        }
    }

    public void LoadEnvironments()
    {
        foreach (var name in fileStateToSave.unlockedEnvironmentNames)
        {
            var environment = environmentSOs.FirstOrDefault(i => i.itemName == name);
            environment.isUnlocked = true;
            if (environment.itemName == fileStateToSave.selectedEnvironment)
            {
                ApplyEnvironment(environment);
            }
        }
    }

    void LoadUpgrades()
    {
        foreach (var name in fileStateToSave.unlockedUpgradeNames)
        {
            var upgrade = upgradeSOs.FirstOrDefault(i => i.itemName == name);
            upgrade.isUnlocked = true;
            ApplyUpgrade(upgrade);
        }
    }

    public void ApplySkin(Skin skin)
    {
        fileStateToSave.selectedBowlSkin = skin.itemName;
        GameManager.EventService.Dispatch(new BowlSkinSelectedEvent());
    }

    public void ApplyEnvironment(Environment environment)
    {
        fileStateToSave.selectedEnvironment = environment.itemName;
        GameManager.EventService.Dispatch(new EnvironmentSelectedEvent());
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case GameplayUpgradeType.BonusBarDuration:
                CurrencyController.Instance.bonusDurationMultiplier *= 1.5f;
                break;
            case GameplayUpgradeType.BonusBarTier:
                CurrencyController.Instance.bonusTierModiier++;
                break;
            case GameplayUpgradeType.BonusBarSpeed:
                CurrencyController.Instance.bonusBarSpeedModifier++;
                break;
            case GameplayUpgradeType.StickyFingers:
                GameManager.Instance.forceMultiplier = 40f;
                break;
        }
    }

    public bool IsUpgradeUnlocked(GameplayUpgradeType type)
    {
        return upgradeSOs.Where(so => so.upgradeType == type).First().isUnlocked;
    }
}
