using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer musicMixer, sfxMixer;
    [SerializeField] private Slider musicSlider, sfxSlider;
    [SerializeField] private TMP_Dropdown languagesDropdown;

    private float musicVolume, sfxVolume;

    public static SettingsMenu Instance;

    private void Awake()
    {
        Instance = this;
    }

    public SaveSettings GetSaveSettings()
        =>  new SaveSettings(musicVolume, sfxVolume, (int)LocalisationSystem.language); 

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

    public void SetLanguage(int selectedLanguage)
    {
        LocalisationSystem.language = (LocalisationSystem.Language)selectedLanguage;
        var foundTextLocaliserUIObjects = FindObjectsOfType<TextLocaliserUI>(true);
        foreach (var item in foundTextLocaliserUIObjects)
        {
            item.SetLanguageValue();
        }

        // manually update battle preparation screen
        BattlePreparationScreenController.current.SelectClassWithoutSound(0);
    }
}

