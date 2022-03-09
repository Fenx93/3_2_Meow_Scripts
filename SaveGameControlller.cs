using EasyMobile;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveGameControlller : MonoBehaviour
{
    public static SaveGameControlller Instance;
    private byte[] testLoadData;
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
        SaveGameMediator.OnLoadDataUpdate += LoadData;
    }

    public void OpenData()
    {
        //ManualSaveGameManager.OpenSavedGame("randomName");
        SaveGameMediator.OpenSavedGame();
    }

    public void DisplayLoginError()
    {
        //ManualSaveGameManager.OpenSavedGame("randomName");
        NativeUI.Alert("PlayStore Login failed!", "Can't load saved data.");
    }

    public void LoadData()
    {
        try
        {
            Debug.Log("Trying to retrieve data!");
            var returnedData = testLoadData;//SaveGameMediator.ReadSavedGame();
            MainSave mainSave = FromByteArray<MainSave>(returnedData);

            PlayerStatsTracker.SetData(mainSave.savedPlayerStats);
            SettingsMenu.Instance.LoadSettings(mainSave.saveSettings);

            InventorySettings.Instance.LoadItemUnlocks(mainSave.allitems);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error:{e}");
        }
    }

    public void SaveData()
    {
        try
        {
            var playerStats = PlayerStatsTracker.GetPlayerStats();
            var settings = SettingsMenu.Instance.GetSaveSettings();
            var allItems = InventorySettings.Tabs.SelectMany(x => x.GetAllStorableItems()).ToArray();
            StorableItem[] storableItems = 
                MainMenuController.current.selectedItems.
                    Select(x => new StorableItem(x.Key, x.Value, ItemStatus.unlocked)).ToArray();

            MainSave mainSave = new MainSave(playerStats, settings, storableItems, allItems);
            var data = ToByteArray(mainSave);
            testLoadData = data;
            SaveGameMediator.WriteSavedGame(data);
        }
        catch(System.Exception e)
        {
            Debug.LogError($"Error:{e}");
        }

    }



    private byte[] ToByteArray<T>(T obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        using MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }

    private T FromByteArray<T>(byte[] data)
    {
        if (data == null)
            return default;
        BinaryFormatter bf = new BinaryFormatter();
        using MemoryStream ms = new MemoryStream(data);
        object obj = bf.Deserialize(ms);
        return (T)obj;
    }
}


//Things to store - player stats : currentExp, currentExpCap, currentLvl, currentMoney;
// Settings: selected language, soundVolume, musicVolume
// Selected items: color, secondaryColor, hat, clothes
// Unlocked items ids
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
