using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private bool isMobile = false;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private AudioMixer musicMixer, sfxMixer;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider musicSlider, sfxSlider;
    [SerializeField] private TMP_Dropdown languagesDropdown;
    private Resolution[] resolutions;

    private float musicVolume, sfxVolume;
    private int resIndx;

    //check memory and load right state of full screen on start
    public void Awake()
    {
        //load saved game settings
        LoadGameSettings();
    }

    //get all possible resolutions for this device and add them to settings resolution dropdown bar
    private void Start()
    {
        if(!isMobile)  
        {
            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                    resIndx = currentResolutionIndex;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        //PopulateLanguagesDropdown();
    }

    public void SetMusicVolume(float volumeSet)
    {
        musicVolume = volumeSet;
        musicMixer.SetFloat("musicVolume", volumeSet);
    }

    public void SetSFXVolume(float volumeSet)
    {
        sfxVolume = volumeSet;
        sfxMixer.SetFloat("sfxVolume", volumeSet);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        PlayerPrefs.SetInt("fullScreen", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
        Screen.fullScreen = isFullScreen;
    }

    public void SetLanguage(int selectedLanguage)
    {
        LocalisationSystem.language = (LocalisationSystem.Language)selectedLanguage;
        var foundTextLocaliserUIObjects = FindObjectsOfType<TextLocaliserUI>(true);
        foreach (var item in foundTextLocaliserUIObjects)
        {
            item.SetLanguageValue();
        }
    }

    private void PopulateLanguagesDropdown()
    {
        var languages = Enum.GetValues(typeof(LocalisationSystem.Language));

        List<string> options = new List<string>();
        int selectedOption = 0;
        for (int i = 0; i < languages.Length; i++)
        {
            var language = (LocalisationSystem.Language[]) languages;
            if (language[i] == LocalisationSystem.language)
            {
                selectedOption = i;
            }
            options.Add(language[i].ToString());
        }
        languagesDropdown.ClearOptions();
        languagesDropdown.AddOptions(options);
        languagesDropdown.SetValueWithoutNotify(selectedOption);
    }

    public void Exit()
    {
        SaveGameSettings();
        Application.Quit();
    }

    private SaveSettings CreateSaveSettingsGameObject()
    {
        SaveSettings saveSettings = new SaveSettings
        {
            musicLevel = musicVolume,
            sfxLevel = sfxVolume,
            resolutionIndex = resIndx
        };

        return saveSettings;
    }

    public void SaveGameSettings()
    {
        // create instance of save settings class and give it required values
        SaveSettings save = CreateSaveSettingsGameObject();

        // create binary formater that converts class into data and saves it
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesettingssave.txt");
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("Game Settings Saved");
    }

    public void LoadGameSettings()
    {
        // search for save file
        if (File.Exists(Application.persistentDataPath + "/gamesettingssave.txt"))
        {

            // convert it from data to class
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesettingssave.txt", FileMode.Open);
            SaveSettings saveStngs = (SaveSettings)bf.Deserialize(file);
            file.Close();

            //set volume from the file
            SetMusicVolume(saveStngs.musicLevel);
            SetSFXVolume(saveStngs.sfxLevel);
            musicSlider.value = saveStngs.musicLevel;
            sfxSlider.value = saveStngs.sfxLevel;
            //SetResolution(saveStngs.resolutionIndex);
            Debug.Log("Game settings Loaded");
            
        }
        else
        {
            Debug.Log("No game settings saved!");
        }

        if (!isMobile)
        {
            // check player prefs for fullscreen key. if it doesn't exist, set basic settings
            if (!PlayerPrefs.HasKey("fullScreen"))
            {
                PlayerPrefs.SetInt("fullScreen", 0);
                fullscreenToggle.isOn = true;
                PlayerPrefs.Save();
            }
            // check player prefs for fullscreen key. if it does exist, set saved settings
            else
            {
                if (PlayerPrefs.GetInt("fullScreen") == 0)
                {
                    SetFullScreen(false);
                    fullscreenToggle.isOn = false;
                }
                else
                {
                    SetFullScreen(true);
                    fullscreenToggle.isOn = true;
                }
            }
        }
    }
}

[System.Serializable]
public class SaveSettings
{
    public float musicLevel;
    public float sfxLevel;
    public int resolutionIndex;
}
