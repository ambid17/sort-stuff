using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : Singleton<ShopController>
{
    public Button backButton;
    public Button powerUpButton;
    public Button itemButton;
    public ShopItem itemPrefab;
    public Transform itemParent;
    public Transform powerUpParent;
    public TMP_Text currencyText;

    private List<ShopItem> powerups;
    private List<ShopItem> items;

    void Start()
    {
        backButton.onClick.AddListener(BackToSettings);
        powerUpButton.onClick.AddListener(() => SetShopPane(true));
        itemButton.onClick.AddListener(() => SetShopPane(false));
        PopulateItems();
        SetShopPane(true);
    }

    void BackToSettings()
    {
        UiManager.Instance.ToggleShop(false);
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

            if (item.itemType == ItemType.Powerup)
            {
                powerups.Add(shopItem);
            }
            else
            {
                items.Add(shopItem);
            }
        }
    }

    void SetShopPane(bool isPowerUps)
    {
        itemParent.gameObject.SetActive(!isPowerUps);
        powerUpParent.gameObject.SetActive(isPowerUps);

        if(isPowerUps)
        {
            foreach (var item in powerups)
            {
                item.UpdateInternal();
            }
        }
        else
        {
            foreach (var item in items)
            {
                item.UpdateInternal();
            }
        }

    }
}
