using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShopTab
{
    PowerUps,
    Items,
    Skins,
    Unlocks
}

public class ShopPanel : UiPanel
{
    public Button backButton;
    public Button powerUpButton;
    public Button itemButton;
    public ShopItem itemPrefab;
    public TMP_Text currencyText;

    public Transform itemParent;
    public Transform powerUpParent;
    public Transform skinsParent;
    public Transform unlocksParent;

    private List<ShopItem> powerups;
    private List<ShopItem> items;
    private List<ShopItem> skins;
    private List<ShopItem> unlocks;

    void Start()
    {
        backButton.onClick.AddListener(Back);
        powerUpButton.onClick.AddListener(() => SetShopTab(ShopTab.PowerUps));
        itemButton.onClick.AddListener(() => SetShopTab(ShopTab.Items));
        PopulateItems();
    }

    protected override void AfterEnable()
    {
        // Force update UI on panel enable
        SetShopTab(ShopTab.Items);
        currencyText.text = $"{UnlockManager.Instance.fileStateToSave.currency}";

    }

    void Back()
    {
        UiManager.Instance.ShowPanel(UiPanelType.NewGame);
    }

    void PopulateItems()
    {
        items = new List<ShopItem>();
        powerups = new List<ShopItem>();

        foreach (var item in UnlockManager.Instance.itemSOs)
        {
            ShopItem shopItem = Instantiate(itemPrefab, item.itemType == ItemType.Powerup ? powerUpParent : itemParent);
            shopItem.SetItem(item);
            shopItem.UpdateInternal();

            switch (item.itemType)
            {
                case ItemType.Powerup:
                    powerups.Add(shopItem);
                    break;
                case ItemType.Item:
                    items.Add(shopItem);
                    break;
                case ItemType.Skin:
                    skins.Add(shopItem);
                    break;
                case ItemType.Unlock:
                    unlocks.Add(shopItem);
                    break;
            }
        }
    }

    void SetShopTab(ShopTab shopTab)
    {
        itemParent.gameObject.SetActive(shopTab == ShopTab.Items);
        powerUpParent.gameObject.SetActive(shopTab == ShopTab.PowerUps);
        skinsParent.gameObject.SetActive(shopTab == ShopTab.Skins);
        unlocksParent.gameObject.SetActive(shopTab == ShopTab.Unlocks);

        List<ShopItem> itemsToUpdate = new List<ShopItem>();
        switch (shopTab)
        {
            case ShopTab.Items:
                itemsToUpdate = items;
                break;
            case ShopTab.PowerUps:
                itemsToUpdate = powerups;
                break;
            case ShopTab.Skins:
                itemsToUpdate = skins;
                break;
            case ShopTab.Unlocks:
                itemsToUpdate = unlocks;
                break;
        }

        foreach (var item in itemsToUpdate)
        {
            item.UpdateInternal();
        }
    }
}
