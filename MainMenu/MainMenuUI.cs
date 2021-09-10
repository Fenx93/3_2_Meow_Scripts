using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject gameModeSelectionScreen, battlePreparationScreen, settingsScreen;

    public void OpenGameModeSelectionScreen()
    {
        gameModeSelectionScreen.SetActive(true);
        battlePreparationScreen.SetActive(false);
        settingsScreen.SetActive(false);
    }
    public void OpenBattlePreparationScreen()
    {
        gameModeSelectionScreen.SetActive(false);
        battlePreparationScreen.SetActive(true);
        settingsScreen.SetActive(false);
    }
    public void OpenSettingsScreen()
    {
        gameModeSelectionScreen.SetActive(false);
        battlePreparationScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }
}
