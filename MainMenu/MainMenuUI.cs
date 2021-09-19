using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels that are turned on/off when going from screen to screen")]
    [SerializeField] private GameObject gameModeSelectionScreen;
    [SerializeField] private GameObject battlePreparationScreen, settingsScreen;
    [SerializeField] private GameObject goBackButton;

    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI moneyText, expText;

    [Header("EXP Slider")]
    [SerializeField] private Slider expSlider;

    public static MainMenuUI current;

    void Awake()
    {
        current = this;
    }

    private void Start()
    {
        OpenGameModeSelectionScreen();
    }

    #region Switch Between Screens
    public void OpenGameModeSelectionScreen()
    {
        gameModeSelectionScreen.SetActive(true);
        battlePreparationScreen.SetActive(false);
        settingsScreen.SetActive(false);
        goBackButton.SetActive(false);
    }

    public void OpenBattlePreparationScreen(int selectedGameMode)
    {
        gameModeSelectionScreen.SetActive(false);
        battlePreparationScreen.SetActive(true);
        settingsScreen.SetActive(false);
        goBackButton.SetActive(true);

        MainMenuController.current.selectedGameMode = (GameModes) selectedGameMode;
    }

    public void OpenSettingsScreen()
    {
        gameModeSelectionScreen.SetActive(false);
        battlePreparationScreen.SetActive(false);
        settingsScreen.SetActive(true);
        goBackButton.SetActive(true);
    }

    #endregion

    #region Update Stat Texts
    public void UpdateLevelText(int level)
    {
        levelText.text = level.ToString();
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }

    public void UpdateExpText(int currentExp, int expCap)
    {
        expText.text = currentExp+"/"+expCap;
        UpdateExpSlider(currentExp, expCap);
    }

    public void UpdateExpSlider(int currentExp, int expCap)
    {
        expSlider.value = (float)currentExp / (float)expCap;
    }
    #endregion
}
