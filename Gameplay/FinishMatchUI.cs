using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static InventorySettings;

public class FinishMatchUI : MonoBehaviour
{
    [SerializeField] private GameObject endGamePanel, returnToMenuButton, gainMoreButton, coinIcon;
    [SerializeField] private TextMeshProUGUI matchResultText, gainedMoneyText, gainedEXPText;
    [SerializeField] private GameObject addableCoins, addableExp;

    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI moneyText, expText;

    [Header("EXP Slider")]
    [SerializeField] private Slider expSlider;

    [Header("Level UP")]
    [SerializeField] private GameObject levelUpPanel;

    [Header("Rewards Spin")]
    [SerializeField] private GameObject rewardsSpinPanel;
    [SerializeField] private GameObject itemUnlockedPanel;
    [SerializeField] private Image unlockedItemIcon, unlockedItemIconImage;

    [Header("Exit to Main Menu")]
    [SerializeField] private string sceneName;

    public static FinishMatchUI current;
    private MatchEndLogicController matchEndLogic;
    private void Awake()
    {
        current = this;
        addableCoins.SetActive(false);
        addableExp.SetActive(false);
        ShowLevelUP(false);
        PlayerStatsTracker.UpdateUI();
    }

    private void Start()
    {
        matchEndLogic = GetComponent<MatchEndLogicController>();
    }

    public void ShowEndGamePanel(bool show)
    {
        endGamePanel.SetActive(show);
    }

    public void ShowGameEndMessage(string message, int money, int exp)
    {
        ShowEndGamePanel(true);
        matchResultText.text = message;

        gainedMoneyText.enabled = false;
        coinIcon.SetActive(false);
        gainedEXPText.enabled = false;
        EnableEarnMoreButton(false);
        returnToMenuButton.SetActive(false);

        gainedMoneyText.text = "+" + money;
        gainedEXPText.text = "+" + exp + "EXP";

        matchEndLogic.AddMoney(money);
        matchEndLogic.AddExperience(exp);

        // Gained stats appear one after the other
        StartCoroutine(nameof(Countdown), 0);

    }


    public void ShowLevelUP(bool enabled)
    {
        levelUpPanel.SetActive(enabled);
    }

    public void ShowRewardsSpin(bool enabled)
    {
        rewardsSpinPanel.SetActive(enabled);
        ShowLevelUP(false);
    }

    public void ShowUnlockedItemPanel(bool enabled)
    {
        itemUnlockedPanel.SetActive(enabled);
    }
    public void SetUnlockedItemPanel(TabItem item, CharacterPart part)
    {
        var tmpro = itemUnlockedPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (item is SpriteTabItem it)
        {
            var keyString = part.ToString().ToLower();
            tmpro.text = $"{LocalisationSystem.GetLocalisedValue("new_item_unlocked")} {LocalisationSystem.GetLocalisedValue(keyString)}";
            unlockedItemIcon.color = InventorySettings.itemQualities[item.quality];
            unlockedItemIconImage.sprite = it.sprite;
        }
        else
        {
            tmpro.text = LocalisationSystem.GetLocalisedValue("new_color_unlocked");
            unlockedItemIconImage.color = ((ColorTabItem)item).color;
        }

    }

    public void EnableEarnMoreButton(bool enabled)
    {
        gainMoreButton.SetActive(enabled);
    }

    #region Coroutines

    public IEnumerator ShowAddedAnimation(float duration, bool isMoney, int amount)
    {
        GameObject gObject;
        gObject = isMoney ? addableCoins : addableExp;
        float elapsedTime = 0;

        gObject.SetActive(true);

        //Position
        var rectTransform = gObject.GetComponent<RectTransform>();
        var startPos = rectTransform.localPosition;
        var endPosition = rectTransform.localPosition;
        endPosition = new Vector3(endPosition.x, 0f, endPosition.z);

        //Update text
        var text = gObject.GetComponent<TextMeshProUGUI>();
        text.text = "+" + amount;

        //Colour
        var startColour = text.color;
        var tempCol = startColour;
        tempCol.a = 0;
        var endColour = tempCol;

        while (elapsedTime < duration)
        {
            rectTransform.localPosition = Vector3.Lerp(startPos, endPosition, (elapsedTime / duration));
            text.color = Color.Lerp(startColour, endColour, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gObject.SetActive(false);

        if (isMoney)
        {
            PlayerStatsTracker.AddMoney(amount);
        }
        else
        {
            PlayerStatsTracker.AddExperience(amount);
        }
    }

    private IEnumerator Countdown(int status)
    {
        float duration = 2f;
        while (duration > 1f)
        {
            duration -= (Time.deltaTime * 1.5f);
            yield return null;
        }
        CountdownEnded(status);
    }

    private IEnumerator Countdown(int status, float duration)
    {
        while (duration > 1f)
        {
            duration -= (Time.deltaTime * 1.5f);
            yield return null;
        }
        CountdownEnded(status);
    }

    private void CountdownEnded(int status)
    {
        switch (status)
        {
            case 0:
                gainedMoneyText.enabled = true;
                coinIcon.SetActive(true);
                StartCoroutine(nameof(Countdown), 1);
                break;
            case 1:
                gainedEXPText.enabled = true;
                StartCoroutine(nameof(Countdown), 2);
                break;
            case 2:
                // make getDouble button appear after gain animation
                if (AdManager.current!= null 
                    && AdManager.current.AdsAvailable())
                {
                    EnableEarnMoreButton(true);
                }
                StartCoroutine(Countdown(3, 4f));
                break;
            case 3:
                // make ReturnToMenu button appear after few seconds
                returnToMenuButton.SetActive(true);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Update Stats Texts
    public void UpdateLevelText(int level)
    {
        levelText.text = level.ToString();
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString();
        StartCoroutine(CoroutineHelper.SmoothlyChangeColorAndFade(moneyText, moneyText.color, Color.cyan, moneyText.color, 1f, 3f));
    }

    public void UpdateExpText(int currentExp, int expCap)
    {
        expText.text = currentExp + "/" + expCap;
        UpdateExpSlider(currentExp, expCap);
        StartCoroutine(CoroutineHelper.SmoothlyChangeColorAndFade(expText, expText.color, Color.cyan, expText.color, 1f, 3f));
    }

    public void UpdateExpSlider(int currentExp, int expCap)
    {
        expSlider.value = (float)currentExp / (float)expCap;
        var fill = expSlider.GetComponentInChildren<Image>();
        StartCoroutine(CoroutineHelper.SmoothlyChangeColorAndFade(new ImageAdapter(fill), fill.color, Color.cyan, fill.color, 1f, 3f));
    }

    public void ExitAndShowAds()
    {
        var adManager = AdManager.current;

        if (PlayerStatsTracker.AdsEnabled() && adManager.InterstitialAdReady())
        {
            adManager.ShowInterstitialAd();
        }
        else
        {
            ExitToMainMenu();
        }
    }

    public void ExitToMainMenu()
    {
        SaveGameController.SaveData();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    #endregion
}
