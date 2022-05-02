using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static LocalisationSystem;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer musicMixer, sfxMixer;
    [SerializeField] private Slider musicSlider, sfxSlider;
    [SerializeField] private Toggle threeSecToggle, fiveSecToggle;
    [SerializeField] private TMP_Dropdown languagesDropdown;
    [SerializeField] private GameObject disableAdsButton;
    [SerializeField] private bool isMainMenu = false;

    private float musicVolume, sfxVolume;
    private int selectedSeconds = 3; 

    public static SettingsMenu Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Initiate()
    {
        LoadSettings(SettingsStorage.Instance.Settings);
        ShowDisableAdsButton();
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
            default:
                SetLanguage((int)Language.English);
                break;
        }
    }

    public SaveSettings GetSaveSettings()
        =>  new SaveSettings(musicVolume, sfxVolume, (int)language, selectedSeconds); 

    public void LoadSettings(SaveSettings saveSettings)
    {
        if (saveSettings != null)
        {
            //set volume from the file
            SetMusicVolume(saveSettings.musicLevel);
            musicSlider.value = saveSettings.musicLevel;
            SetSFXVolume(saveSettings.sfxLevel);
            sfxSlider.value = saveSettings.sfxLevel;
            if (isMainMenu)
            {
                SetSecondFromCode(saveSettings.selectedSeconds);
            }
            else
            {
                LoadSeconds(saveSettings.selectedSeconds);
            }
            SetLanguage(saveSettings.selectedLanguage);

            Debug.Log("Game settings Loaded");
        }
        else
        {
            #if !UNITY_EDITOR
            Debug.LogError("No Save Settings loaded!");
            #endif
        }
    }
    
    #region Audio
    public void SetMusicVolume(float volumeSet)
    {
        musicVolume = volumeSet;
        musicMixer.SetFloat("musicVolume", Mathf.Log10(volumeSet) * 20);
    }

    public void SetSFXVolume(float volumeSet)
    {
        sfxVolume = volumeSet;
        sfxMixer.SetFloat("sfxVolume", Mathf.Log10(volumeSet) * 20);
    }
    #endregion

    #region TurnSeconds
    public void LoadSeconds(int seconds)
    {
        selectedSeconds = seconds;
    }
    public void SetSecondFromCode(int seconds)
    {
        if (seconds == 3 && threeSecToggle != null)
        {
            if (fiveSecToggle != null)
            {
                fiveSecToggle.isOn = false;
            }
            threeSecToggle.isOn = true;
            selectedSeconds = seconds;
        }
        else if(seconds == 5 && fiveSecToggle != null)
        {
            if (threeSecToggle != null)
            {
                threeSecToggle.isOn = false;
            }
            fiveSecToggle.isOn = true;
            selectedSeconds = seconds;
        }
    }
    public void SetSeconds(int seconds)
    {
        if (seconds == 3 && threeSecToggle != null)
        {
            if (threeSecToggle.isOn)
            {
                if (fiveSecToggle != null)
                {
                    fiveSecToggle.isOn = false;
                }
                selectedSeconds = 3;
            }
            else
            {
                if (fiveSecToggle != null)
                {
                    fiveSecToggle.isOn = true;
                }
                selectedSeconds = 5;
            }
        }
        else if(seconds == 5 && fiveSecToggle != null)
        {
            if (fiveSecToggle.isOn)
            {
                if (threeSecToggle != null)
                {
                    threeSecToggle.isOn = false;
                }
                selectedSeconds = seconds;
            }
            else
            {
                if (fiveSecToggle != null)
                {
                    threeSecToggle.isOn = true;
                }
                selectedSeconds = 3;
            }
        }
    }

    public float GetSeconds() => (float)selectedSeconds;

    #endregion

    #region Language
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

        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        if (scene.name == "MainMenuScene")
        {
            // manually update battle preparation screen
            BattlePreparationScreenController.current.SelectClassWithoutSound(0);
        }
    }
    #endregion
}

