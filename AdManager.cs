using EasyMobile;
using System.Collections;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    private AdType _adType;

    public static AdManager current;
    // Start is called before the first frame update
    void Awake()
    {
        //if (current != null)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        current = this;
        //DontDestroyOnLoad(gameObject);
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();
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
        if (Advertising.IsRewardedAdReady())
            return true;
        else
        {
            StartCoroutine(nameof(RetryToGetAds));
            return false;
        }
    }

    public void ShowRewardedAd(AdType adType)
    {
        if (Advertising.IsRewardedAdReady())
        {
            _adType = adType;
            Advertising.ShowRewardedAd();
        }
    }

    public bool InterstitialAdReady()
    {
        var result = Advertising.IsInterstitialAdReady();
        print($"Interstitial Ad is Ready{result}");
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
        switch (_adType)
        {
            case AdType.freeSpin:
                PlayerStatsTracker.RemoveMoney(PlayerStatsTracker.CurrentMoney);
                RewardsSpinMainMenuUI.current.ShowRewardsSpin(true);
                break;
            case AdType.earnMore:
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

public enum AdType
{
    freeSpin,
    earnMore
}
