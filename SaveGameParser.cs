using EasyMobile;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveGameParser : MonoBehaviour
{
    //Things to store - player stats : currentExp, currentExpCap, currentLvl, currentMoney;
    // Unlocked items ids
    // Selected items: color, secondaryColor, hat, clothes
    //Settings -selected language, soundVolume, musicVolume
    private void Awake()
    {
        GameServices.UserLoginSucceeded += OpenData;
        GameServices.UserLoginFailed += DisplayLoginError;
        SaveGameManager.OnLoadDataUpdate += LoadStatsData;
    }

    public void OpenData()
    {
        //ManualSaveGameManager.OpenSavedGame("randomName");
        SaveGameManager.OpenSavedGame();
    }

    public void DisplayLoginError()
    {
        //ManualSaveGameManager.OpenSavedGame("randomName");
        NativeUI.Alert("PlayStore Login failed!", "Can't load saved data.");
    }

    public void LoadStatsData()
    {
        try
        {
            Debug.Log("Trying to retrieve data!");
            var returnedData = SaveGameManager.ReadSavedGame();
            PlayerStats playerStatsObj = FromByteArray<PlayerStats>(returnedData);
            PlayerStatsTracker.SetData(playerStatsObj);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error:{e}");
        }
    }

    public void LoadSavedData(byte[] data)
    {
        PlayerStats playerStatsObj = FromByteArray<PlayerStats>(data);
        PlayerStatsTracker.SetData(playerStatsObj);
    }

    public void SaveStatsData()
    {
        try
        {
            var playerStatsObj = PlayerStatsTracker.GetPlayerStatsObject();
            var data = ToByteArray(playerStatsObj);
            SaveGameManager.WriteSavedGame(data);
        }
        catch(System.Exception e)
        {
            Debug.LogError($"Error:{e}");
        }

    }

    public byte[] ToByteArray<T>(T obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        using MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }

    public T FromByteArray<T>(byte[] data)
    {
        if (data == null)
            return default;
        BinaryFormatter bf = new BinaryFormatter();
        using MemoryStream ms = new MemoryStream(data);
        object obj = bf.Deserialize(ms);
        return (T)obj;
    }
}
