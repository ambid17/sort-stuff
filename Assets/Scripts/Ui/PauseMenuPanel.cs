using UnityEngine;
using UnityEngine.UI;

public class PauseMenuPanel : UiPanel
{
    public Button ResumeButton;
    public Button NewGameButton;
    public Button SettingsButton;
    public Button QuitButton;

    void Start()
    {
        ResumeButton.onClick.AddListener(Resume);
        NewGameButton.onClick.AddListener(NewGame);
        SettingsButton.onClick.AddListener(Settings);
        QuitButton.onClick.AddListener(Quit);
    }

    public override void AfterEnable()
    {
        GameManager.Instance.isGameRunning = false;
    }

    void Resume()
    {
        GameManager.Instance.isGameRunning = true;
        UiManager.Instance.ShowPanel(UiPanelType.Hud);
    }

    void NewGame()
    {
        GameManager.Instance.EndGame();
        UiManager.Instance.ShowPanel(UiPanelType.NewGame);
    }

    void Settings()
    {
        UiManager.Instance.ShowPanel(UiPanelType.Settings);
    }

    void Quit()
    {
        UnlockManager.Instance.Save();
        Application.Quit();
    }
}
