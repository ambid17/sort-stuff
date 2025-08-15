using System.Collections.Generic;
using UnityEngine;

public class CurrencyController : Singleton<CurrencyController>
{
    public List<PooledObject> textPool;
    public PooledObject textPrefab;
    public Transform textParent;

    private const float BONUS_DURATION = 10f;
    private const int DEFAULT_MAX_BONUS_TIER = 3;
    private const int SORTED_OBJECTS_PER_TIER = 5;
    private const float TIME_BONUS_FOR_SORT = 4F;

    // set by unlocks
    public float bonusDurationMultiplier = 1f; // increases the duration of the bonus bar when sorted objects are collected
    public int bonusTierModiier = 0; // increases the maximum bonus tier based on unlocks
    public int bonusBarSpeedModifier = 1; // increases the speed at which the bonus bar fills up when sorted objects are collected

    private float ScaledBonusDuration => BONUS_DURATION * bonusDurationMultiplier;
    private int maxBonusTier => DEFAULT_MAX_BONUS_TIER + bonusTierModiier;
    private int bonusTier => Mathf.Min(maxBonusTier, sortedObjectCounter / SORTED_OBJECTS_PER_TIER);
    private int sortedObjectCounter = 0;
    private float bonusTimer = 0f;
    private int lastBonusTier;



    void Start()
    {
        PopulateObjectPool();
    }

    void Update()
    {
        bonusTimer -= Time.deltaTime;
        if (bonusTimer <= 0)
        {
            if(sortedObjectCounter > 0)
            {
                sortedObjectCounter = Mathf.Max(0, sortedObjectCounter - SORTED_OBJECTS_PER_TIER);
            }
            bonusTimer = ScaledBonusDuration;
        }

        UpdateBonusBarSlider();
    }

    void UpdateBonusBarSlider()
    {
        UiManager.Instance.hudPanel.bonusCountText.text = sortedObjectCounter.ToString();
        if (sortedObjectCounter > 0)
        {
            UiManager.Instance.hudPanel.BonusBar.mainSlider.value = bonusTimer / ScaledBonusDuration;
        }
        else
        {
            UiManager.Instance.hudPanel.BonusBar.mainSlider.value = 0;
        }

        if(lastBonusTier != bonusTier)
        {
            var tierInfo = GetTierInfo(bonusTier);
            UiManager.Instance.hudPanel.BonusBar.mainSlider.colors = new UnityEngine.UI.ColorBlock()
            {
                normalColor = tierInfo.color,
                highlightedColor = tierInfo.color,
                pressedColor = tierInfo.color,
                selectedColor = tierInfo.color,
                colorMultiplier = 1,
            };

            lastBonusTier = bonusTier;
        }
    }

    public void SortComplete(Sortable sortable)
    {
        sortedObjectCounter += bonusBarSpeedModifier;
        bonusTimer = Mathf.Min(ScaledBonusDuration, bonusTimer + TIME_BONUS_FOR_SORT);

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
            case 4:
                return new TierInfo(Color.blue, 27, 7);
            case 5:
                return new TierInfo(Color.cyan, 28, 10);
            case 6:
                return new TierInfo(Color.magenta, 28, 15);
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
