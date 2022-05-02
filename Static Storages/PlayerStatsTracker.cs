
public static class PlayerStatsTracker
{
    private static int _currentExp = 0, _currentMoney = 0, 
        _currentExpCap = 75, _currentLvl = 1;

    #region Public Properties
    public static int CurrentExp { 
        get => _currentExp;
        private set {
            _currentExp = value;
            SetExpText();
        }
    }

    public static int CurrentExpCap
    {
        get => _currentExpCap;
        private set
        {
            _currentExpCap = value;
            SetExpText();
        }
    }

    public static int CurrentLvl
    {
        get => _currentLvl;
        private set
        {
            _currentLvl = value;
            if (MainMenuUI.current != null)
            {
                MainMenuUI.current.UpdateLevelText(_currentLvl);
            }
            if (FinishMatchUI.current != null)
            {
                FinishMatchUI.current.UpdateLevelText(_currentLvl);
            }
        }
    }

    public static int CurrentMoney
    {
        get => _currentMoney;
        private set
        {
            _currentMoney = value;
            if (MainMenuUI.current != null)
            {
                MainMenuUI.current.UpdateMoneyText(_currentMoney);
            }
            if (FinishMatchUI.current != null)
            {
                FinishMatchUI.current.UpdateMoneyText(_currentMoney);
            }
            if (RewardsSpinMainMenuUI.current != null)
            {
                RewardsSpinMainMenuUI.current.UpdateButtons(EnoughForSpin());
            }
        }
    }

    public static bool AdsEnabled()
        => !PurchasesController.AdsDisabled();

    public static bool EnoughForSpin() => (CurrentMoney >= 100);
    
    public static bool EnoughForAdSpin() => (CurrentMoney >= 50);

    #endregion

    public static PlayerStats GetPlayerStats()
        => new PlayerStats(CurrentExp, CurrentExpCap, CurrentLvl, CurrentMoney);

    public static void SetData(PlayerStats playerStats)
    {
        CurrentExp = playerStats.currentExp;
        CurrentExpCap = playerStats.currentExpCap;
        CurrentLvl =  playerStats.currentLvl;
        CurrentMoney =  playerStats.currentMoney;
    }

    public static void SetData(int currentLvl, 
        int currentExp, int currentExpCap, int currentMoney)
    {
        CurrentExp = currentExp;
        CurrentExpCap = currentExpCap;
        CurrentLvl = currentLvl;
        CurrentMoney = currentMoney;
    }

    public static void UpdateUI()
    {
        if (MainMenuUI.current != null)
        {
            MainMenuUI.current.UpdateExpText(CurrentExp, CurrentExpCap);
            MainMenuUI.current.UpdateMoneyText(CurrentMoney);
            MainMenuUI.current.UpdateLevelText(CurrentLvl);
        }
        if (FinishMatchUI.current != null)
        {
            FinishMatchUI.current.UpdateExpText(CurrentExp, CurrentExpCap);
            FinishMatchUI.current.UpdateMoneyText(CurrentMoney);
            FinishMatchUI.current.UpdateLevelText(CurrentLvl);
        }
    }

    public static void AddMoney(int moneyAmount)
    {
        CurrentMoney += moneyAmount;
        //SaveGameController.SaveData();
    }
    public static void RemoveMoney(int moneyAmount)
    {
        CurrentMoney -= moneyAmount;
        //SaveGameController.SaveData();
    }

    public static void AddExperience(int experienceAmount)
    {
        CurrentExp += experienceAmount;
        CheckNextLevel();
        //SaveGameController.SaveData();
    }

    private static void SetExpText()
    {
        if (MainMenuUI.current != null)
        {
            MainMenuUI.current.UpdateExpText(CurrentExp, CurrentExpCap);
        }
        if (FinishMatchUI.current != null)
        {
            FinishMatchUI.current.UpdateExpText(CurrentExp, CurrentExpCap);
        }
    }

    private static void CheckNextLevel()
    {
        while (CurrentExp >= CurrentExpCap)
        {
            CurrentLvl++;
            CurrentExpCap = CurrentLvl * 85 + (20 * (CurrentLvl - 1)); //get next experience cap
            FinishMatchUI.current.ShowLevelUP(true, CurrentLvl);
            AudioController.current.PlayCelebrationSound();
        }
    }
}
