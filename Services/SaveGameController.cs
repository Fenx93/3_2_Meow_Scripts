using EasyMobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using static InventorySettings;

public class SaveGameController : MonoBehaviour
{
    public static SaveGameController Instance;

    public static bool SavedGameExists
        => SaveGameMediator.mySavedGame != null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        GameServices.UserLoginSucceeded += OpenData;
        GameServices.UserLoginFailed += DisplayLoginError;
        //SaveGameMediator.OnLoadDataUpdate += LoadData; - Do not load on setup with a loading screen
    }

    public void OpenData()
    {
        try
        {
            SaveGameMediator.OpenSavedGame();
        }
        catch (Exception)
        {
            NativeUI.Alert("Saved Data load failed!", "Can't load saved data.");
        }
    }

    private void DisplayLoginError()
    {
        NativeUI.Alert("PlayStore Login failed!", "Can't load saved data.");
    }

    public static void LoadData()
    {
        try
        {
            Debug.Log("Trying to retrieve data!");
            if (!SavedGameExists)
            {
                Debug.LogWarning($"Saved game does not exist!");
                return;
            }
            SaveGameMediator.ReadSavedGame();
        }
        catch (Exception e)
        {
            Debug.LogError($"Load Data Error:{e}");
        }
    }

    public static void SaveData()
    {
        try
        {
            if (!SavedGameExists)
            {
                Debug.LogWarning($"Saved game does not exist!");
                return;
            }
            var playerStats = PlayerStatsTracker.GetPlayerStats();
            var settings = SettingsMenu.Instance.GetSaveSettings();
            SettingsStorage.Instance.Settings = settings;
            var allItems = InventorySettings.Tabs.SelectMany(x => x.GetAllStorableItems()).ToArray();
            //var testAllUnlockedItems = allItems.Where(x=> x.status == ItemStatus.unlocked).ToArray();
            StorableItem[] storableItems =
                EquipedItemsStorage.Instance.selectedItems.
                    Select(x => new StorableItem(x.Key, x.Value, ItemStatus.unlocked)).ToArray();

            MainSave mainSave = new MainSave(playerStats, settings, storableItems, allItems);
            var data = ToByteArray(mainSave);
            SaveGameMediator.WriteSavedGame(data);
        }
        catch(Exception e)
        {
            Debug.LogError($"Save Data Error:{e}");
        }

    }

    private static byte[] ToByteArray<T>(T obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        using MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }
}

#region Serializable Save Objects

[System.Serializable]
public class MainSave
{
    public PlayerStats savedPlayerStats;
    public SaveSettings saveSettings;
    public StorableItem[] savedSelectedItems;
    public StorableItem[] allitems;

    public MainSave(PlayerStats playerStats, SaveSettings saveSettings, StorableItem[] selectedItems, StorableItem[] allitems)
    {
        this.savedPlayerStats = playerStats;
        this.saveSettings = saveSettings;
        this.savedSelectedItems = selectedItems;
        this.allitems = allitems;
    }

    public Dictionary<CharacterPart, string> GetSelectedItems(StorableItem[] selectedItems)
        => selectedItems.ToDictionary(s => s.part, s => s.id);
}

[System.Serializable]
public class PlayerStats
{
    public int currentExp, currentExpCap, currentLvl, currentMoney;

    public PlayerStats(int currentExp, int currentExpCap, int currentLvl, int currentMoney)
    {
        this.currentExp = currentExp;
        this.currentExpCap = currentExpCap;
        this.currentLvl = currentLvl;
        this.currentMoney = currentMoney;
    }
}

[System.Serializable]
public class SaveSettings
{
    public float musicLevel, sfxLevel;
    public int selectedLanguage;
    public SaveSettings(float musicLevel, float sfxLevel, int selectedLanguage)
    {
        this.musicLevel = musicLevel;
        this.sfxLevel = sfxLevel;
        this.selectedLanguage = selectedLanguage;
    }
}


[System.Serializable]
public struct StorableItem {
    public InventorySettings.CharacterPart part;
    public string id;
    public ItemStatus status;

    public StorableItem(InventorySettings.CharacterPart part, string id, ItemStatus status)
    {
        this.part = part;
        this.id = id;
        this.status = status;
    }
}

#endregion