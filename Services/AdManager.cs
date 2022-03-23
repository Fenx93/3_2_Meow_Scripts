using EasyMobile;
using System.Collections;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    private RewardedAdType _rewardedAdType;

    public static AdManager current;
    // Start is called before the first frame update
    void Awake()
    {
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }
        current = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Events
    public event System.Action<bool> OnAdsAvailable;

    // Subscribe events
    void OnEnable()
    {
        Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;
        Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;
        Advertising.InterstitialAdCompleted += InterstitialAdCompletedHandler;
    }

    void OnDisable()
    {
        Advertising.RewardedAdCompleted -= RewardedAdCompletedHandler;
        Advertising.RewardedAdSkipped -= RewardedAdSkippedHandler;
        Advertising.InterstitialAdCompleted -= InterstitialAdCompletedHandler;
    }
    #endregion

    public bool AdsAvailable()
    {
        return Advertising.IsRewardedAdReady();
    }

    public void ShowRewardedAd(RewardedAdType adType)
    {
        if (Advertising.IsRewardedAdReady())
        {
            _rewardedAdType = adType;
            Advertising.ShowRewardedAd();
        }
    }

    public bool InterstitialAdReady()
    {
        var result = Advertising.IsInterstitialAdReady();
        Debug.Log($"Interstitial Ad is Ready: {result}");
        return result;
    }

    public void ShowInterstitialAd()
    {
        if (Advertising.IsInterstitialAdReady())
            Advertising.ShowInterstitialAd();
    }
    
    private IEnumerator RetryToGetAds()
    {
        while (!Advertising.IsRewardedAdReady())
        {
            yield return new WaitForSeconds(0.1f);
        }
        print("Ad found!");
        OnAdsAvailable?.Invoke(PlayerStatsTracker.EnoughForSpin());
        yield return null;
    }

    // Event handler called when a rewarded ad has completed
    void RewardedAdCompletedHandler(RewardedAdNetwork network, AdPlacement placement)
    {
        Debug.Log("Rewarded ad has completed. The user should be rewarded now.");
        switch (_rewardedAdType)
        {
            case RewardedAdType.freeSpin:
                PlayerStatsTracker.RemoveMoney(PlayerStatsTracker.CurrentMoney);
                RewardsSpinMainMenuUI.current.ShowRewardsSpin(true);
                break;
            case RewardedAdType.earnMore:
                MatchEndLogicController.current.DoubleEarnings();
                break;
            default:
                break;
        }
    }

    // Event handler called when a rewarded ad has been skipped
    void RewardedAdSkippedHandler(RewardedAdNetwork network, AdPlacement placement)
    {
        Debug.Log("Rewarded ad was skipped. The user should NOT be rewarded.");
    }

    // The event handler
    void InterstitialAdCompletedHandler(InterstitialAdNetwork network, AdPlacement placement)
    {
        Debug.Log("Interstitial Ad Completed!");
        FinishMatchUI.current.ExitToMainMenu();
    }
}

public enum RewardedAdType { freeSpin, earnMore }
