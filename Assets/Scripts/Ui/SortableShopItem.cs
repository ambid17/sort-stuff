using CaosCreations;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SortableShopItem : ShopItem
{
    public override void SetItem(Item item)
    {
        this.item = item;
        UpdateInternal();
    }

    public override void UpdateInternal()
    {
        itemNameText.text = item.itemName;
        iconContainer.SetActive(item.icon != null);
        itemIcon.sprite = item.icon;
        SetupButton();
    }

    protected override void SetupButton()
    {
        // ModernUI doesn't have this set up on Awake
        if (purchaseButton.buttonVar == null)
        {
            return;
        }

        purchaseButton.normalText.text = item.cost.ToString();
        purchaseButton.buttonVar.onClick.RemoveAllListeners();

        if (!item.isUnlocked)
        {
            if (item.cost > UnlockManager.Instance.fileStateToSave.currency)
            {
                purchaseButton.buttonVar.interactable = false;
                purchaseButton.normalText.color = Color.red;
            }
            else
            {
                purchaseButton.buttonVar.interactable = true;
                purchaseButton.buttonVar.onClick.AddListener(OnUnlockClicked);
            }
        }
        else
        {
            purchaseButton.buttonVar.interactable = false;
            purchaseButton.normalText.text = "Unlocked";
        }
    }

    public override void OnUnlockClicked()
    {
        var didUnlock = UnlockManager.Instance.TryUnlock(item);
        if (didUnlock)
        {
            UiManager.Instance.shopPanel.currencyText.text = $"{UnlockManager.Instance.fileStateToSave.currency}";
            UpdateInternal();
        }
    }
}
