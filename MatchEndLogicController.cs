using UnityEngine;

public class MatchEndLogicController : MonoBehaviour
{
    public static MatchEndLogicController current;
    private FinishMatchUI matchEndUI;

    private int gainedMoney, gainedExperience;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        matchEndUI = GetComponent<FinishMatchUI>();
    }


    public void AddMoney(int money)
    {
        gainedMoney = money;
        // PlayerStatsTracker.AddMoney(money);
        StartCoroutine(matchEndUI.ShowAddedAnimation(2f, true, money));
    }

    public void AddExperience(int experience)
    {
        gainedExperience = experience;
        // PlayerStatsTracker.AddExperience(experience);
        StartCoroutine(matchEndUI.ShowAddedAnimation(2f, false, experience));
    }

    // Should be available only when there are ads
    public void EarnMore()
    {
        AdManager.current.ShowRewardedAd(AdType.earnMore);
    }

    public void DoubleEarnings()
    {
        StartCoroutine(matchEndUI.ShowAddedAnimation(2f, true, gainedMoney));
        StartCoroutine(matchEndUI.ShowAddedAnimation(2f, false, gainedExperience));
        matchEndUI.EnableEarnMoreButton(false);
    }
}
