using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown qualityDropdown;
    public Toggle fpsToggle;
    public TMP_Dropdown fpsDropdown;
    public Toggle vSyncToggle;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Components")]
    public GameObject fpsPanel;
    public AudioMixer audioMixer;

    private const string QUALITY_KEY = "Quality";
    private const string FPSENABLED_KEY = "FPSEnabled";
    private const string FPS_KEY = "FPS";
    private const string VSYNC_KEY = "VSync";

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Start()
    {
        InitializeQuality();
        LoadSettings();

        ApplyAllSettings();
    }

    #region Initialization

    private void InitializeQuality()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
    }

    #endregion

    #region Graphics

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(QUALITY_KEY, qualityIndex);
    }

    public void SetFPSEnabled(bool enabled)
    {
        fpsPanel.SetActive(enabled);
        PlayerPrefs.SetInt(FPSENABLED_KEY, enabled ? 1 : 0);
    }

    public void SetFPS(int index)
    {
        int fps = 60;
        switch (index)
        {
            case 0:
                fps = 30;
                break;
            case 1:
                fps = 60;
                break;
            case 2:
                fps = 120;
                break;
            case 3:
                fps = 144;
                break;
            case 4:
                fps = -1;
                break;
        }
        Application.targetFrameRate = fps;
        PlayerPrefs.SetInt(FPS_KEY, index);
    }

    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        PlayerPrefs.SetInt(VSYNC_KEY, enabled ? 1 : 0);
    }

    #endregion

    #region Audio

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat(MASTER_KEY, value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    #endregion

    #region Save / Load

    private void LoadSettings()
    {
        int quality = PlayerPrefs.GetInt(QUALITY_KEY, QualitySettings.GetQualityLevel());
        bool fullenabled = PlayerPrefs.GetInt(FPSENABLED_KEY, 1) == 1;
        int fps = PlayerPrefs.GetInt(FPS_KEY, 1);
        bool vSync = PlayerPrefs.GetInt(VSYNC_KEY, 0) == 1;
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        qualityDropdown.value = quality;
        fpsToggle.isOn = fullenabled;
        fpsDropdown.value = fps;
        vSyncToggle.isOn = vSync;
        masterVolumeSlider.value = master;
        musicVolumeSlider.value = music;
        sfxVolumeSlider.value = sfx;
    }

    private void ApplyAllSettings()
    {
        SetQuality(qualityDropdown.value);
        SetFPSEnabled(fpsToggle.isOn);
        SetFPS(fpsDropdown.value);
        SetVSync(vSyncToggle.isOn);
        SetMasterVolume(masterVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetSFXVolume(sfxVolumeSlider.value);
    }

    #endregion
}