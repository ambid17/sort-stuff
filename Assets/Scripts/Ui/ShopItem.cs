using CaosCreations;
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

    protected Item item;

    public virtual void SetItem(Item item)
    {
        this.item = item;
        if(item is Skin)
        {
            GameManager.EventService.Add<BowlSkinSelectedEvent>(OnBowlSkinSelected);
        }
        if (item is Environment)
        {
            GameManager.EventService.Add<EnvironmentSelectedEvent>(OnWallSkinSelected);
        }
        UpdateInternal();
    }

    public virtual void UpdateInternal()
    {
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        iconContainer.SetActive(item.icon != null);
        itemIcon.sprite = item.icon;
        SetupButton();
    }

    protected virtual void SetupButton()
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
            if(item is Skin || item is Environment)
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
        bool isSelectedBowl = item is Skin && item.itemName == UnlockManager.Instance.selectedBowlSkin?.itemName;
        bool isSelectedWall = item is Environment && item.itemName == UnlockManager.Instance.selectedEnvironment?.itemName;
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
            purchaseButton.buttonVar.onClick.RemoveAllListeners();
            purchaseButton.buttonVar.onClick.AddListener(OnSkinApply);
            borderImage.color = Color.white;
        }
    }

    public virtual void OnUnlockClicked()
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
        if(item is Environment env)
        {
            UnlockManager.Instance.ApplyEnvironment(env);
        }
        else
        {
            UnlockManager.Instance.ApplySkin(item as Skin);
        }
    }

    private void OnBowlSkinSelected(BowlSkinSelectedEvent e)
    {
        UpdateInternal();
    }

    private void OnWallSkinSelected(EnvironmentSelectedEvent e)
    {
        UpdateInternal();
    }
}
