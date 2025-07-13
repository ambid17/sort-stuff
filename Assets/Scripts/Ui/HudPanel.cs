using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudPanel : UiPanel
{
    public TMP_Text remainingText;
    public TMP_Text currencyText;
    public Button pauseButton;

    public SliderManager BonusBar;
    public TMP_Text bonusCountText;

    void Start()
    {
        panel.gameObject.SetActive(false);

        pauseButton.onClick.AddListener(TogglePauseMenu);
#if UNITY_EDITOR
        bonusCountText.gameObject.SetActive(true);
#endif
    }

    public void SetRemaining()
    {
        remainingText.text = $"Remaining: {GameManager.Instance.remainingCount}";
    }

    public void TogglePauseMenu()
    {
        UiManager.Instance.ShowPanel(UiPanelType.PauseMenu);
    }
}
