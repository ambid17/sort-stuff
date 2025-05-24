using System.Collections.Generic;
using UnityEngine;

public class CurrencyController : Singleton<CurrencyController>
{
    public List<PooledObject> textPool;
    public PooledObject textPrefab;
    public Transform textParent;

    private float bonusTimer = 0f;
    private const float BONUS_DURATION = 10f;

    private int bonusTier => bonusCounter / BONUS_PER_TIER;
    private int bonusCounter = 0;
    private const int BONUS_PER_TIER = 3;
    private const float TIME_BONUS_FOR_SORT = 4F;


    void Start()
    {
        PopulateObjectPool();
    }

    // 6 objects

    void Update()
    {
        // 3s - 1/60
        bonusTimer -= Time.deltaTime;
        if (bonusTimer <= 0)
        {
            if(bonusCounter > 0)
            {
                bonusCounter = Mathf.Max(0, bonusCounter - BONUS_PER_TIER);
            }
            bonusTimer = BONUS_DURATION;
        }

        UpdateSlider();
    }

    void UpdateSlider()
    {
        UiManager.Instance.bonusCountText.text = bonusCounter.ToString();
        if (bonusCounter > 0)
        {
            UiManager.Instance.BonusBar.mainSlider.value = bonusTimer / BONUS_DURATION;
        }
        else
        {
            UiManager.Instance.BonusBar.mainSlider.value = 0;
        }

        var tierInfo = GetTierInfo(bonusTier);
        UiManager.Instance.BonusBar.mainSlider.colors = new UnityEngine.UI.ColorBlock() 
        { 
            normalColor = tierInfo.color,
            highlightedColor = tierInfo.color,
            pressedColor = tierInfo.color,
            selectedColor = tierInfo.color,
            colorMultiplier = 1,
        };
    }

    public void SortComplete(Sortable sortable)
    {
        bonusCounter++;
        bonusTimer = Mathf.Min(BONUS_DURATION, bonusTimer + TIME_BONUS_FOR_SORT);

        var tierInfo = GetTierInfo(bonusTier);
        UnlockManager.Instance.AddCurrency(tierInfo.currencyValue);
        

        var text = GetPooledObject();
        text.transform.position = sortable.transform.position + new Vector3(0, 3, 3);
        text.gameObject.SetActive(true);
        text.SetTier(bonusTier);
    }

    private PooledObject GetPooledObject()
    {
        foreach (var text in textPool)
        {
            if (!text.gameObject.activeInHierarchy)
            {
                return text;
            }
        }
        var newText = Instantiate(textPrefab, textParent);
        textPool.Add(newText);
        return newText;
    }

    void PopulateObjectPool()
    {
        for (int i = 0; i < 20; i++)
        {
            var textInstance = Instantiate(textPrefab, textParent);
            textInstance.gameObject.SetActive(false);
            textPool.Add(textInstance);
        }
    }

    public static TierInfo GetTierInfo(int tier)
    {
        switch (tier)
        {
            case 0:
                return new TierInfo(Color.white, 24, 1);
            case 1:
                return new TierInfo(Color.green, 25, 2);
            case 2:
                return new TierInfo(Color.yellow, 26, 3);
            case 3:
                return new TierInfo(Color.red, 27 , 5);
            default:
                var purpleColor = new Color(.71f, .27f, .8f, 1);
                return new TierInfo(purpleColor, 28, 10);
        }
    }
}

public struct TierInfo
{
    public Color color;
    public int fontSize;
    public int currencyValue;
    public TierInfo(Color color, int fontSize, int currencyValue)
    {
        this.color = color;
        this.fontSize = fontSize;
        this.currencyValue = currencyValue;
    }
}
