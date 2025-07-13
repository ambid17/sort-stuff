using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePanel : UiPanel
{
    public TMP_Text titleText;
    public Button startButton;
    public Button shopButton;
    public SliderManager boxCountSlider;
    public SliderManager objectTypeSlider;
    public SliderManager objectCountSlider;

    void Start()
    {
        titleText.text = "Sort some stuff";
        startButton.onClick.AddListener(StartGame);
        boxCountSlider.mainSlider.onValueChanged.AddListener(SetBoxCount);
        objectTypeSlider.mainSlider.onValueChanged.AddListener(SetTypeCount);
        objectCountSlider.mainSlider.onValueChanged.AddListener(SetObjectCount);
        shopButton.onClick.AddListener(() => UiManager.Instance.ShowPanel(UiPanelType.Shop));
    }

    private void StartGame()
    {
        GameManager.Instance.StartGame();
        UiManager.Instance.ShowPanel(UiPanelType.Hud);
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
}
