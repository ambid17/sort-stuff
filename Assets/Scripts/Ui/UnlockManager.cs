using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
public enum GameplayUpgradeType
{
    GoldInjection,
    RainingMoney,
    StickyFingers
}

public class UnlockManager : Singleton<UnlockManager>
{
    public SaveFile fileStateToSave;
    [Header("scriptable objects")]
    public List<Item> powerUpSOs;
    public List<Item> itemSOs;
    public List<Item> skinSOs;
    public List<Item> upgradeSOs;
    [Header("unlocks")]
    public List<Item> unlockedPowerUps;
    public List<Item> unlockedItems;
    public List<Item> unlockedSkins;
    public List<Item> unlockedUpgrades;
    [Header("selected skins")]
    public Skin selectedBowlSkin;
    public Skin selectedWallSkin;

    public Dictionary<GameplayUpgradeType, bool> gameplayUpgradeStatuses = new Dictionary<GameplayUpgradeType, bool>
    {
        { GameplayUpgradeType.GoldInjection, false },
        { GameplayUpgradeType.RainingMoney, false },
        { GameplayUpgradeType.StickyFingers, false }
    };

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
        switch (item.itemType)
        {
            case ItemType.Item:
                unlockedItems.Add(item);
                break;
            case ItemType.Powerup:
                unlockedPowerUps.Add(item);
                ApplyPowerup(item);
                break;
            case ItemType.WallSkin:
            case ItemType.BowlSkin:
                unlockedSkins.Add(item);
                break;
            case ItemType.Upgrade:
                unlockedUpgrades.Add(item);
                ApplyUpgrade(item);
                break;
        }
        Save();
        return true;
    }

    public bool IsUnlocked(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Item:
                return unlockedItems.Contains(item);
            case ItemType.Powerup:
                return unlockedPowerUps.Contains(item);
            case ItemType.WallSkin:
            case ItemType.BowlSkin:
                return unlockedSkins.Contains(item);
            case ItemType.Upgrade:
                return unlockedUpgrades.Contains(item);
        }
        return false;
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
            fileStateToSave.unlockedItemNames = unlockedItems?.Select(i => i.itemName).ToList();
            fileStateToSave.unlockedPowerUpNames = unlockedPowerUps?.Select(i => i.itemName).ToList();
            fileStateToSave.unlockedSkinNames = unlockedSkins?.Select(i => i.itemName).ToList();
            fileStateToSave.unlockedUpgradeNames = unlockedUpgrades?.Select(i => i.itemName).ToList();
            var fileData = JsonConvert.SerializeObject(fileStateToSave);
            string filePath = Path.Combine(Application.persistentDataPath, "unlocks.json");

            File.WriteAllText(filePath, fileData);
        }catch(Exception e)
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
                UpdateUnlockList(unlockedItems, fileStateToSave.unlockedItemNames, itemSOs);
                UpdateUnlockList(unlockedPowerUps, fileStateToSave.unlockedPowerUpNames, powerUpSOs);
                UpdateUnlockList(unlockedSkins, fileStateToSave.unlockedSkinNames, skinSOs);
                UpdateUnlockList(unlockedUpgrades, fileStateToSave.unlockedUpgradeNames, upgradeSOs);
            }

            UiManager.Instance.hudPanel.currencyText.text = $"{fileStateToSave.currency}";
            UiManager.Instance.shopPanel.currencyText.text = $"{fileStateToSave.currency}";
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading: {e.Message}\n{e.StackTrace}");
        }
    }

    void UpdateUnlockList(List<Item> listToUpdate, List<string> names, List<Item> masterLookupList)
    {
        if (listToUpdate == null)
        {
            listToUpdate = new List<Item>();
        }
        foreach (var name in names)
        {
            var item = masterLookupList.FirstOrDefault(i => i.itemName == name);
            if (item != null)
            {
                listToUpdate.Add(item);
                switch (item.itemType)
                {
                    case ItemType.Powerup:
                        ApplyPowerup(item);
                        break;
                    case ItemType.WallSkin:
                    case ItemType.BowlSkin:
                        ApplySkin(item);
                        break;
                    case ItemType.Upgrade:
                        ApplyUpgrade(item);
                        break;
                }
            }
        }
    }

    public void ApplyPowerup(Item powerup)
    {
        
    }

    public void ApplySkin(Item skin)
    {
        if (skin.itemName.Contains("Shiny Bowls"))
        {
            CurrencyController.Instance.bonusDurationMultiplier *= 1.5f;
        }
    }

    public void ApplyUpgrade(Item upgrade)
    {
        if (upgrade.itemName.Contains("Bonus Bar Duration"))
        {
            CurrencyController.Instance.bonusDurationMultiplier *= 1.5f;
        }

        if (upgrade.itemName.Contains("Bonus Bar Tier"))
        {
            CurrencyController.Instance.bonusTierModiier++;
        }

        if (upgrade.itemName.Contains("Bonus Bar Speed"))
        {
            CurrencyController.Instance.bonusBarSpeedModifier++;
        }

        if (upgrade.itemName == "Gold Injection")
        {
            gameplayUpgradeStatuses[GameplayUpgradeType.GoldInjection] = true;
        }

        if (upgrade.itemName == "Raining Money")
        {
            gameplayUpgradeStatuses[GameplayUpgradeType.RainingMoney] = true;
        }

        if (upgrade.itemName == "Sticky Fingers")
        {
            gameplayUpgradeStatuses[GameplayUpgradeType.StickyFingers] = true;
            GameManager.Instance.forceMultiplier = 40f;
        }
    }
}
