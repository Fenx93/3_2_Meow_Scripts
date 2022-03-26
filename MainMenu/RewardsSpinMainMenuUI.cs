using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventorySettings;

public class RewardsSpinMainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonWithAds, buttonWithoutAds, rewardsSpinTitle;
    [SerializeField] private GameObject rewardsSpinPanel;
    [SerializeField] private GameObject itemUnlockedPanel;
    [SerializeField] private Image unlockedItemIcon, unlockedItemIconImage;

    public static RewardsSpinMainMenuUI current;
    void Awake()
    {
        current = this;
    }
    private void Start()
    {
        rewardsSpinTitle.SetActive(false);
        buttonWithAds.SetActive(false);
        buttonWithoutAds.SetActive(false);
        UpdateButtons(PlayerStatsTracker.EnoughForSpin());
        AdManager.current.OnAdsAvailable += UpdateButtons;
    }

    public void UpdateButtons(bool enoughMoney)
    {
        if (rewardsSpinTitle != null)
        {
            rewardsSpinTitle.SetActive(true);
            buttonWithoutAds.SetActive(enoughMoney);
            if (enoughMoney)
                return;
            buttonWithAds.SetActive(false);
            if (AdManager.current.AdsAvailable() && !enoughMoney)
                buttonWithAds.SetActive(true);
            else
                rewardsSpinTitle.SetActive(false);
        }
    }

    public void ShowRewardsSpin(bool enabled)
    {
        rewardsSpinPanel.SetActive(enabled);
    }

    public void CheckForMoneyAndStartRewardsSpin(bool enabled)
    {
        if (PlayerStatsTracker.EnoughForSpin())
        {
            PlayerStatsTracker.RemoveMoney(100);
            ShowRewardsSpin(enabled);
        }
        else
        {
            AdManager.current.ShowRewardedAd(RewardedAdType.freeSpin);
        }
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
}
