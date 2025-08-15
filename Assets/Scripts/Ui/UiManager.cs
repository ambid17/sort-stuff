using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum Difficulty
{
    Easy, Medium, Hard
}

public enum UiPanelType
{
    NewGame, Shop, Hud, PauseMenu, Win, Settings, Debug
}

public class UiManager : Singleton<UiManager>
{
    public NewGamePanel newGamePanel;
    public ShopPanel shopPanel;
    public HudPanel hudPanel;
    public PauseMenuPanel pausePanel;
    public WinPanel winPanel;
    public SettingsPanel settingsPanel;
    public DebugPanel debugPanel;

    void Start()
    {
        ShowPanel(UiPanelType.NewGame);
    }

    void Update()
    {
        
    }

    public void ShowPanel(UiPanelType panelType)
    {
        newGamePanel.Toggle(panelType == UiPanelType.NewGame);
        shopPanel.Toggle(panelType == UiPanelType.Shop);
        hudPanel.Toggle(panelType == UiPanelType.Hud);
        pausePanel.Toggle(panelType == UiPanelType.PauseMenu);
        winPanel.Toggle(panelType == UiPanelType.Win);
        settingsPanel.Toggle(panelType == UiPanelType.Settings);
        debugPanel.Toggle(panelType == UiPanelType.Debug);
    }
}
