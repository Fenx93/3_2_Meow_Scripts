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

    public RewardsSpinMainMenuUI GetCurrent()
    {
        if (current == null)
        {
            current = this;
        }
        return current;
    }

    public void Initiate()
    {
        rewardsSpinTitle.SetActive(false);
        buttonWithAds.SetActive(false);
        buttonWithoutAds.SetActive(false);
        UpdateButtons(PlayerStatsTracker.EnoughForSpin());
    }
    void Update()
    {
        if (AdManager.current != null /*&& !AdManager.current.RewardedAdsAvailable()*/)
        {
            UpdateButtons(PlayerStatsTracker.EnoughForSpin());
        }
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
            if (PlayerStatsTracker.EnoughForAdSpin() 
                && AdManager.current != null && AdManager.current.RewardedAdsAvailable())
                buttonWithAds.SetActive(true);
            else
                rewardsSpinTitle.SetActive(false);
        }
    }

    public void ShowRewardsSpin(bool enabled)
    {
        rewardsSpinPanel.SetActive(enabled);
    }

    public void CheckForMoneyAndStartRewardsSpin()
    {
        if (PlayerStatsTracker.EnoughForSpin())
        {
            PlayerStatsTracker.RemoveMoney(100);
            ShowRewardsSpin(true);
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
