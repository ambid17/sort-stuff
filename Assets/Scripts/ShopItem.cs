using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Image borderImage;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public ButtonManagerBasicWithIcon purchaseButton;
    public GameObject iconContainer;
    public Image itemIcon;

    private Item item;

    public void SetItem(Item item)
    {
        this.item = item;
        UpdateInternal();
    }

    public void UpdateInternal()
    {
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        iconContainer.SetActive(item.icon != null);
        itemIcon.sprite = item.icon;
        SetupButton();
    }

    void SetupButton()
    {
        // ModernUI doesn't have this set up on Awake
        if (purchaseButton.buttonVar == null)
        {
            return;
        }

        purchaseButton.normalText.text = item.cost.ToString();
        purchaseButton.buttonVar.onClick.RemoveAllListeners();

        if (!UnlockManager.Instance.IsUnlocked(item))
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
            if(item.itemType == ItemType.BowlSkin || item.itemType == ItemType.WallSkin)
            {
                HandleUnlockedSkinItem();
            }
            else
            {
                purchaseButton.buttonVar.interactable = false;
                purchaseButton.normalText.text = "Unlocked";
            }
        }
    }

    void HandleUnlockedSkinItem()
    {
        bool isSelectedBowl = item.itemType == ItemType.BowlSkin && item.itemName == UnlockManager.Instance.selectedBowlSkin.itemName;
        bool isSelectedWall = item.itemType == ItemType.WallSkin && item.itemName == UnlockManager.Instance.selectedWallSkin.itemName;
        if (isSelectedBowl || isSelectedWall)
        {
            purchaseButton.normalText.text = "Selected";
            purchaseButton.buttonVar.interactable = false;
            borderImage.color = Color.green;
        }
        else
        {
            purchaseButton.buttonVar.interactable = true;
            purchaseButton.normalText.text = "Enable";
            purchaseButton.buttonVar.onClick.AddListener(OnSkinApply);
        }
    }

    public void OnUnlockClicked()
    {
        var didUnlock = UnlockManager.Instance.TryUnlock(item);
        if (didUnlock)
        {
            UiManager.Instance.shopPanel.currencyText.text = $"{UnlockManager.Instance.fileStateToSave.currency}";
            UpdateInternal();
        }
    }

    public void OnSkinApply()
    {
        UnlockManager.Instance.ApplySkin(item);
    }
}
