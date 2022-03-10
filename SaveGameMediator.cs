using EasyMobile;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveGameMediator
{

    // To store the opened saved game.
    private static SavedGame mySavedGame;

    public static event Action OnLoadDataUpdate;

    // Open a saved game with automatic conflict resolution
    public static void OpenSavedGame()
    {
        // Open a saved game named "My_Saved_Game" and resolve conflicts automatically if any.
            GameServices.SavedGames.OpenWithAutomaticConflictResolution(
                "My_Saved_Game", OpenSavedGameCallback);
    }


    // Open saved game callback
    public static void OpenSavedGameCallback(SavedGame savedGame, string error)
    {
        if (string.IsNullOrEmpty(error))
        {
            Debug.Log("Saved game opened successfully!");
            mySavedGame = savedGame;        // keep a reference for later operations
            OnLoadDataUpdate?.Invoke();
            //fire an event to notify SaveGameParser that it can load data
        }
        else
        {
            Debug.Log("Open saved game failed with error: " + error);
        }
    }

    public static void WriteSavedGame(byte[] data)
    {
        if (mySavedGame.IsOpen)
        {
            // The saved game is open and ready for writing
            GameServices.SavedGames.WriteSavedGameData(
                mySavedGame,
                data,
                (SavedGame updatedSavedGame, string error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        Debug.Log("Saved game data has been written successfully!");
                    }
                    else
                    {
                        Debug.LogError($"Writing saved game data failed with error: {error}");
                    }
                }
            );
        }
        else
        {
            // The saved game is not open. You can optionally open it here and repeat the process.
            Debug.LogWarning("You must open the saved game before writing to it.");
        }
    }

    public static byte[] ReadSavedGame()
    {
        if (mySavedGame.IsOpen)
        {
            byte[] returnData = null;
            // The saved game is open and ready for reading
            GameServices.SavedGames.ReadSavedGameData(
                mySavedGame,
                (SavedGame game, byte[] data, string error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        Debug.Log("Saved game data has been retrieved successfully!");
                        // Here you can process the data as you wish.
                        if (data.Length > 0)
                        {
                            // Data processing
                            MainSave mainSave = FromByteArray<MainSave>(data);
                            PlayerStatsTracker.SetData(mainSave.savedPlayerStats);
                            SettingsMenu.Instance.LoadSettings(mainSave.saveSettings);

                            InventorySettings.Instance.LoadItemUnlocks(mainSave.allitems);
                        }
                        else
                        {
                            Debug.LogWarning("The saved game has no data!");
                        }
                    }
                    else
                    {
                        Debug.LogError("Reading saved game data failed with error: " + error);
                    }
                }
            );
            return returnData;
        }
        else
        {
            // The saved game is not open. You can optionally open it here and repeat the process.
            Debug.LogWarning("You must open the saved game before reading its data.");
        }
        return null;
    }


    public static T FromByteArray<T>(byte[] data)
    {
        if (data == null)
            return default;
        BinaryFormatter bf = new BinaryFormatter();
        using var ms = new System.IO.MemoryStream(data);
        object obj = bf.Deserialize(ms);
        return (T)obj;
    }
}
