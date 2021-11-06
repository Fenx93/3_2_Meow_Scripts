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

    [Header("Start Game Button")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI classUnlocksAtText;

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

    public void StartTraining()
    {
        MainMenuController.current.StartTraining();
    }

    public void SetStartGameButton(bool isLocked, string buttonText, int? unlocksAtLevel = null)
    {
        startGameButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        startGameButton.interactable = !isLocked;
        classUnlocksAtText.enabled = isLocked;
        if (isLocked && unlocksAtLevel.HasValue)
        {
            classUnlocksAtText.text = "Class will be unlocked at level " + unlocksAtLevel.Value;
        }
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
        CoroutineHelper.SmoothlyChangeColorAndFade(moneyText, moneyText.color, Color.white, moneyText.color, 1f, 3f);
    }

    public void UpdateExpText(int currentExp, int expCap)
    {
        expText.text = currentExp+"/"+expCap;
        UpdateExpSlider(currentExp, expCap);
        CoroutineHelper.SmoothlyChangeColorAndFade(expText, expText.color, Color.white, expText.color, 1f, 3f);
    }

    public void UpdateExpSlider(int currentExp, int expCap)
    {
        expSlider.value = (float)currentExp / (float)expCap;
    }
    #endregion
}
