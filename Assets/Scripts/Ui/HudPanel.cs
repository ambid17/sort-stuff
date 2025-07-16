using Michsky.UI.ModernUIPack;
using System.Collections;
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
    public TMP_Text bonusPopupText;

    void Start()
    {
        panel.gameObject.SetActive(false);

        pauseButton.onClick.AddListener(TogglePauseMenu);
#if UNITY_EDITOR
        bonusCountText.gameObject.SetActive(true);
#endif
        bonusPopupText.gameObject.SetActive(false);
    }

    protected override void AfterEnable()
    {
        currencyText.text = $"{UnlockManager.Instance.fileStateToSave.currency}";
    }

    public void SetRemaining()
    {
        remainingText.text = $"Remaining: {GameManager.Instance.remainingCount}";
    }

    public void TogglePauseMenu()
    {
        UiManager.Instance.ShowPanel(UiPanelType.PauseMenu);
    }

    public IEnumerator ShowBonusPopup(string text, float duration = 1f)
    {
        bonusPopupText.text = text;
        bonusPopupText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        bonusPopupText.gameObject.SetActive(false);
    }
}
