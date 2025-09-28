using CaosCreations;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : UiPanel
{
    public Button nextButton;

    public Image[] starImages;
    public Sprite unfilledStar;
    public Sprite filledStar;

    public TMP_Text timeText;
    public TMP_Text itemsSortedText;
    public TMP_Text powerupsUsedText;
    
    public const float idealTimePerSortable = 5f;

    private bool gameEnded = false;

    void Start()
    {
        nextButton.onClick.AddListener(Next);
        GameManager.EventService.Add<GameEndedEvent>(OnGameEnded);
    }

    void OnGameEnded(GameEndedEvent e)
    {
        gameEnded = true;
    }

    protected override void AfterEnable()
    {
        if (!gameEnded)
        {
            return;
        }

        ClearUI();
        StartCoroutine(PlayWinAnimation());
    }

    void ClearUI()
    {
        for (int i = 1; i <= 3; i++)
        {
            starImages[i - 1].sprite = unfilledStar;
        }

        timeText.text = $"Time:";
        itemsSortedText.text = $"Items Sorted:";
        powerupsUsedText.text = $"Powerups used:";
    }

    private IEnumerator PlayWinAnimation()
    {
        var starsEarned = GetStarsEarned();

        for (int i = 1; i <= 3; i++)
        {
            starImages[i-1].sprite = i <= starsEarned ? filledStar : unfilledStar;
            starImages[i-1].transform.DOPunchScale(Vector3.one * 1.5f, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }

        float currentTimeMillis = 0;
        while(currentTimeMillis <= CurrencyController.Instance.roundTimer)
        {
            currentTimeMillis += UnityEngine.Random.Range(100, 500);
            TimeSpan time = TimeSpan.FromMilliseconds(currentTimeMillis);
            timeText.text = $"Time: {time.ToString("mm\\:ss\\.fff")}";
            yield return new WaitForSeconds(0.1f);
        }

        timeText.text = $"Time: {TimeSpan.FromSeconds(CurrencyController.Instance.roundTimer).ToString("mm\\:ss\\.fff")}";


        int currentItemsSorted = 0;
        while (currentItemsSorted <= CurrencyController.Instance.itemsSortedThisRound)
        {
            itemsSortedText.text = $"Items Sorted: {currentItemsSorted++}";
            yield return new WaitForSeconds(0.1f);
        }

        int powerupsUsed = 0;
        while (powerupsUsed <= CurrencyController.Instance.powerupsUsedThisRound)
        {
            powerupsUsedText.text = $"Powerups used: {powerupsUsed++}";
            yield return new WaitForSeconds(0.1f);
        }
    }

    private int GetStarsEarned()
    {
        int stars = 1;
        var gameTime = CurrencyController.Instance.roundTimer;

        var totalSorted = CurrencyController.Instance.itemsSortedThisRound;
        if (gameTime <= totalSorted * idealTimePerSortable)
        {
            stars++;
        }
        if (gameTime <= totalSorted * idealTimePerSortable * 0.75f)
        {
            stars++;
        }

        return stars;
    }

    void Next()
    {
        UiManager.Instance.ShowPanel(UiPanelType.NewGame);
    }
}
