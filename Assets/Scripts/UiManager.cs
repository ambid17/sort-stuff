using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum Difficulty
{
    Easy, Medium, Hard
}

public class UiManager : Singleton<UiManager>
{
    public GameObject gameControlPanel;
    public TMP_Text titleText;
    public Button startButton;
    public SliderManager boxCountSlider;
    public SliderManager objectTypeSlider;
    public SliderManager objectCountSlider;

    public GameObject inGamePanel;
    public TMP_Text remainingText;




    void Start()
    {
        titleText.text = "Sort some stuff";
        gameControlPanel.gameObject.SetActive(true);
        inGamePanel.gameObject.SetActive(false);
        startButton.onClick.AddListener(StartGame);
        boxCountSlider.mainSlider.onValueChanged.AddListener(SetBoxCount);
        objectTypeSlider.mainSlider.onValueChanged.AddListener(SetTypeCount);
        objectCountSlider.mainSlider.onValueChanged.AddListener(SetObjectCount);
    }

    void Update()
    {
        
    }

    public void ShowWin()
    {
        titleText.text = "You won! Play again?";
        gameControlPanel.gameObject.SetActive(true);
        inGamePanel.gameObject.SetActive(false);
    }

    private void StartGame()
    {
        GameManager.Instance.StartGame();
        gameControlPanel.gameObject.SetActive(false);
        inGamePanel.gameObject.SetActive(true);
    }

    public void SetBoxCount(float containerCount)
    {
        var count = Mathf.RoundToInt(containerCount);
        GameManager.Instance.SetContainerCount(count);
    }

    public void SetTypeCount(float typeCount)
    {
        var count = Mathf.RoundToInt(typeCount);
        GameManager.Instance.SetTypeCount(count);
    }

    public void SetObjectCount(float objectCount)
    {
        var count = Mathf.RoundToInt(objectCount);
        GameManager.Instance.SetCountPerType(count);
    }

    public void SetRemaining()
    {
        remainingText.text = $"Remaining: {GameManager.Instance.remainingCount}";
    }
}
