using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePanel : UiPanel
{
    public TMP_Text titleText;
    public Button startButton;
    public Button shopButton;
    public RadialSlider boxCountSlider;
    public RadialSlider objectTypeSlider;
    public RadialSlider objectCountSlider;

    void Start()
    {
        titleText.text = "Sort some stuff";
        startButton.onClick.AddListener(StartGame);
        boxCountSlider.onValueChanged.AddListener(SetBoxCount);
        objectTypeSlider.onValueChanged.AddListener(SetTypeCount);
        objectCountSlider.onValueChanged.AddListener(SetObjectCount);
        shopButton.onClick.AddListener(() => UiManager.Instance.ShowPanel(UiPanelType.Shop));
    }

    private void StartGame()
    {
        GameManager.Instance.StartGame();
        UiManager.Instance.ShowPanel(UiPanelType.Hud);
    }

    public void SetBoxCount(float containerCount)
    {
        var count = Mathf.RoundToInt(boxCountSlider.SliderValue);
        if(count != GameManager.Instance.ContainerCount)
        {
            GameManager.Instance.SetContainerCount(count);
        }
    }

    public void SetTypeCount(float typeCount)
    {
        var count = Mathf.RoundToInt(objectTypeSlider.SliderValue);
        if (count != GameManager.Instance.TypeCount)
        {
            GameManager.Instance.SetTypeCount(count);
        }
    }

    public void SetObjectCount(float objectCount)
    {
        var count = Mathf.RoundToInt(objectCountSlider.SliderValue);
        if (count != GameManager.Instance.CountPerType)
        {
            GameManager.Instance.SetCountPerType(count);
        }
    }
}
