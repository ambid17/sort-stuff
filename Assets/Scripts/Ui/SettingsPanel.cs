using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanel : UiPanel
{
    public Slider VolumeSlider;
    public Button BackButton;
    public AudioMixer audioMixer;

    private const string masterVolumeParam = "Master";

    private void Awake()
    {
        LoadSettings();
    }

    void Start()
    {
        VolumeSlider.onValueChanged.AddListener(SetVolume);
        BackButton.onClick.AddListener(Back);
    }

    void SetVolume(float value)
    {
        float volumeInDecibels = ConvertToDecibel(value);
        audioMixer.SetFloat(masterVolumeParam, volumeInDecibels);
        PlayerPrefs.SetFloat(masterVolumeParam, volumeInDecibels);

    }

    void Back()
    {
        UiManager.Instance.ShowPanel(UiPanelType.PauseMenu);
    }

    void LoadSettings()
    {
        VolumeSlider.minValue = 0.0001f;
        VolumeSlider.maxValue = 1f;
        VolumeSlider.wholeNumbers = false;
        VolumeSlider.value = ConvertFromDecibel(PlayerPrefs.GetFloat(masterVolumeParam, 0f));
        AudioListener.volume = PlayerPrefs.GetFloat(masterVolumeParam, 1f);
    }

    public float ConvertToDecibel(float value)
    {
        return Mathf.Log10(value) * 20;
    }

    public float ConvertFromDecibel(float value)
    {
        return Mathf.Pow(10, value / 20);
    }
}
