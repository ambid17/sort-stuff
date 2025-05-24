using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public TMP_Text itemNameText;
    public ButtonManagerBasicWithIcon purchaseButton;
    public Image itemIcon;

    private Item item;

    public void SetItem(Item item)
    {
        this.item = item;
    }

    /// <summary>
    /// This is only necessary because Awake happens before purchaseButton.buttonVar is setup by ModernUI.
    /// And we need to ensure that the item is setup first
    /// </summary>
    public void UpdateInternal()
    {
        itemNameText.text = item.itemName;
        itemIcon.sprite = item.icon;
        purchaseButton.normalText.text = item.cost.ToString();

        if (UnlockManager.Instance.IsUnlocked(item.id))
        {
            purchaseButton.buttonVar.interactable = false;
            purchaseButton.normalText.text = "Unlocked";
        }
        else if (item.cost > UnlockManager.Instance.fileStateToSave.currency)
        {
            purchaseButton.buttonVar.interactable = false;
            purchaseButton.normalText.color = Color.red;
        }
        else
        {
            purchaseButton.buttonVar.onClick.RemoveAllListeners();
            purchaseButton.buttonVar.onClick.AddListener(() => OnUnlockClicked(item.id));
        }
    }

    public void OnUnlockClicked(int itemId)
    {
        var didUnlock = UnlockManager.Instance.TryUnlock(itemId);
        if (didUnlock)
        {
            ShopController.Instance.currencyText.text = $"{UnlockManager.Instance.fileStateToSave.currency}";
        }
    }
}
