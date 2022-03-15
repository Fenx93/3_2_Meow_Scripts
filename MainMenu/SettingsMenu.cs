using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static LocalisationSystem;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer musicMixer, sfxMixer;
    [SerializeField] private Slider musicSlider, sfxSlider;
    [SerializeField] private TMP_Dropdown languagesDropdown;
    [SerializeField] private GameObject disableAdsButton;

    private float musicVolume, sfxVolume;

    public static SettingsMenu Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowDisableAdsButton();
        SelectDefaultLanguage();
    }

    public void ShowDisableAdsButton()
    {
        disableAdsButton.SetActive(PlayerStatsTracker.AdsEnabled());
    }

    public void SelectDefaultLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                SetLanguage((int)Language.English);
                break;
            case SystemLanguage.German:
                SetLanguage((int)Language.German);
                break;
            case SystemLanguage.Russian:
                SetLanguage((int)Language.Russian);
                break;
            case SystemLanguage.Lithuanian:
                SetLanguage((int)Language.Lithuanian);
                break;
            case SystemLanguage.Italian:
            case SystemLanguage.Polish:
            case SystemLanguage.Portuguese:
            case SystemLanguage.Spanish:
            case SystemLanguage.Ukrainian:
            default:
                SetLanguage((int)Language.English);
                break;
        }
    }

    public SaveSettings GetSaveSettings()
        =>  new SaveSettings(musicVolume, sfxVolume, (int)language); 

    public void LoadSettings(SaveSettings saveSettings)
    {
        //set volume from the file
        SetMusicVolume(saveSettings.musicLevel);
        musicSlider.value = saveSettings.musicLevel;
        SetSFXVolume(saveSettings.sfxLevel);
        sfxSlider.value = saveSettings.sfxLevel;

        SetLanguage(saveSettings.selectedLanguage);

        Debug.Log("Game settings Loaded");
    }

    public void SetMusicVolume(float volumeSet)
    {
        musicVolume = volumeSet;
        musicMixer.SetFloat("musicVolume", Mathf.Log10(volumeSet) * 20);
    }

    public void SetSFXVolume(float volumeSet)
    {
        sfxVolume = volumeSet;
        sfxMixer.SetFloat("sfxVolume", Mathf.Log10(volumeSet)*20);
    }

    public void SetLanguage(Language language)
    {
        SetLanguage((int)language);
    }

    public void SetLanguage(int selectedLanguage)
    {
        language = (Language)selectedLanguage;
        var foundTextLocaliserUIObjects = FindObjectsOfType<TextLocaliserUI>(true);
        foreach (var item in foundTextLocaliserUIObjects)
        {
            item.SetLanguageValue();
        }

        // manually update battle preparation screen
        BattlePreparationScreenController.current.SelectClassWithoutSound(0);
    }
}

