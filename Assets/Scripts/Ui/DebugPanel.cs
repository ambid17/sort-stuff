using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : UiPanel
{
    public TMP_Text debugText;
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(Back);
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        var color = type switch
        {
            LogType.Error => "<color=red>",
            LogType.Assert => "<color=yellow>",
            LogType.Warning => "<color=orange>",
            LogType.Log => "<color=white>",
            LogType.Exception => "<color=purple>",
            _ => "<color=white>"
        };

        var toLog = $"{logString}\n";
        if (type == LogType.Error || type == LogType.Exception)
        {
            toLog += $"{stackTrace}\n";
        }

        debugText.text += $"{color}{toLog}</color>\n";
    }

    void Back()
    {
        UiManager.Instance.ShowPanel(UiPanelType.PauseMenu);
    }
}
