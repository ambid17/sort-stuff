using UnityEngine;
using UnityEngine.UI;

public class WinPanel : UiPanel
{
    public Button nextButton;

    void Start()
    {
        nextButton.onClick.AddListener(Next);
    }

    void Next()
    {
        UiManager.Instance.ShowPanel(UiPanelType.NewGame);
    }
}
