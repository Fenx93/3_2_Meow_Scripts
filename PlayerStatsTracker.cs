public static class PlayerStatsTracker
{
    private static int? _currentExp, _currentExpCap, _currentLvl, _currentMoney;

    #region Public Properties
    public static int CurrentExp { 
        get => _currentExp.Value; 
        private set {
            _currentExp = value;
            SetExpText();
        }
    }

    public static int CurrentExpCap
    {
        get => _currentExpCap.Value;
        private set
        {
            _currentExpCap = value;
            SetExpText();
        }
    }

    public static int CurrentLvl
    {
        get => _currentLvl.Value;
        private set
        {
            _currentLvl = value;
            MainMenuUI.current.UpdateLevelText(_currentLvl.Value);
        }
    }

    public static int CurrentMoney
    {
        get => _currentMoney.Value;
        private set
        {
            _currentMoney = value;
            MainMenuUI.current.UpdateMoneyText(_currentMoney.Value);
        }
    }
    #endregion

    public static void SetData(int currentLvl, int currentExp, int currentExpCap, int currentMoney)
    {
        if (_currentExp == null && _currentExpCap == null && _currentLvl == null && _currentMoney == null)
        {
            CurrentExp = currentExp;
            CurrentExpCap = currentExpCap;
            CurrentLvl = currentLvl;
            CurrentMoney = currentMoney;
        }
    }

    public static void UpdateUI()
    {
        MainMenuUI.current.UpdateExpText(CurrentExp, CurrentExpCap);
        MainMenuUI.current.UpdateMoneyText(CurrentMoney);
        MainMenuUI.current.UpdateLevelText(CurrentLvl);
    }

    private static void SetExpText()
    {
        if (_currentExp.HasValue && _currentExpCap.HasValue)
        {
            MainMenuUI.current.UpdateExpText(CurrentExp, CurrentExpCap);
        }
    }

    public static void AddMoney(int moneyAmount)
    {
        CurrentMoney += moneyAmount;
    }
    public static void RemoveMoney(int moneyAmount)
    {
        CurrentMoney -= moneyAmount;
    }

    public static void AddExperience(int experienceAmount)
    {
        CurrentExp += experienceAmount;
        CheckNextLevel();
    }

    private static void CheckNextLevel()
    {
        if (CurrentExp >= CurrentExpCap)
        {
            CurrentLvl++;
            //get next experience cap
            CurrentExpCap *= 2;
        }
    }
}
