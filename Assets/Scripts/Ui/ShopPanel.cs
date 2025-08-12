using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CaosTabButton = CaosCreations.TabButton;

public enum ShopTab
{
    PowerUps,
    Items,
    Skins,
    Upgrades
}

public class ShopPanel : UiPanel
{
    public Button backButton;
    public CaosTabButton powerUpButton;
    public CaosTabButton itemButton;
    public CaosTabButton skinsButton;
    public CaosTabButton upgradesButton;
    public ShopItem shopItemPrefab;
    public TMP_Text currencyText;

    public Transform itemParent;
    public Transform powerUpParent;
    public Transform skinsParent;
    public Transform upgradesParent;

    private List<ShopItem> powerups;
    private List<ShopItem> items;
    private List<ShopItem> skins;
    private List<ShopItem> upgrades;

    void Start()
    {
        backButton.onClick.AddListener(Back);
        powerUpButton.button.onClick.AddListener(() => SetShopTab(ShopTab.PowerUps));
        itemButton.button.onClick.AddListener(() => SetShopTab(ShopTab.Items));
        skinsButton.button.onClick.AddListener(() => SetShopTab(ShopTab.Skins));
        upgradesButton.button.onClick.AddListener(() => SetShopTab(ShopTab.Upgrades));
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
        skins = new List<ShopItem>();
        upgrades = new List<ShopItem>();

        foreach (var item in UnlockManager.Instance.itemSOs)
        {
            ShopItem shopItem = Instantiate(shopItemPrefab, itemParent);
            shopItem.SetItem(item);
            items.Add(shopItem);
        }

        foreach (var item in UnlockManager.Instance.powerUpSOs)
        {
            ShopItem shopItem = Instantiate(shopItemPrefab, powerUpParent);
            shopItem.SetItem(item);
            powerups.Add(shopItem);
        }

        foreach (var item in UnlockManager.Instance.skinSOs)
        {
            ShopItem shopItem = Instantiate(shopItemPrefab, skinsParent);
            shopItem.SetItem(item);
            skins.Add(shopItem);
        }

        foreach(var item in UnlockManager.Instance.environmentSOs)
        {
            ShopItem shopItem = Instantiate(shopItemPrefab, skinsParent);
            shopItem.SetItem(item);
            skins.Add(shopItem);
        }

        foreach (var item in UnlockManager.Instance.upgradeSOs)
        {
            ShopItem shopItem = Instantiate(shopItemPrefab, upgradesParent);
            shopItem.SetItem(item);
            upgrades.Add(shopItem);
        }
    }

    void SetShopTab(ShopTab shopTab)
    {
        itemParent.gameObject.SetActive(shopTab == ShopTab.Items);
        powerUpParent.gameObject.SetActive(shopTab == ShopTab.PowerUps);
        skinsParent.gameObject.SetActive(shopTab == ShopTab.Skins);
        upgradesParent.gameObject.SetActive(shopTab == ShopTab.Upgrades);

        powerUpButton.SetSelected(shopTab == ShopTab.PowerUps);
        itemButton.SetSelected(shopTab == ShopTab.Items);
        skinsButton.SetSelected(shopTab == ShopTab.Skins);
        upgradesButton.SetSelected(shopTab == ShopTab.Upgrades);

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
            case ShopTab.Upgrades:
                itemsToUpdate = upgrades;
                break;
        }

        foreach (var item in itemsToUpdate)
        {
            item.UpdateInternal();
        }
    }
}
