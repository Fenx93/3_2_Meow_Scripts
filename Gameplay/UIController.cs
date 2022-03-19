using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable] public class MyDictionary1 : SerializableDictionary<ActionClassification, Color> { }

public class UIController : MonoBehaviour
{
    [Header("In Game UI")]
    [SerializeField] private TextMeshProUGUI timer;

    [SerializeField] private GameObject playerUI, enemyUI, settingsScreen, settingsIcon;
    private TextMeshProUGUI _playerAction, _playerEnergy, _enemyAction, _enemyEnergy;
    private Slider _playerEnergySlider, _enemyEnergySlider;

    [SerializeField] private Image pauseIcon;

    private Image[] _playerHeartHPImages, _enemyHeartHPImages;
    private Image _playerAmmoImage, _enemyAmmoImage;

    private Sprite pauseSprite, playSprite;

    #region Summoner UI fields
    private GameObject _playerSummonObject, _enemySummonObject;
    private TextMeshProUGUI _playerSummonText, _enemySummonText;
    #endregion

    #region Trapper UI fields
    private GameObject _playerTrapUIObject, _enemyTrapUIObject;
    private TextMeshProUGUI _playerTrapText, _enemyTrapText;
    #endregion
    
    #region Berserk UI fields
    private GameObject _playerBerserkObject, _enemyBerserkObject;
    private TextMeshProUGUI _playerBerserkConcentrationText, _enemyBerserkConcentrationText;
    private TextMeshProUGUI _playerBerserkDamageText, _enemyBerserkDamageText;
    #endregion

    public Button[] actionButtons;
    private bool[] buttonStatuses;

    public MyDictionary1 selectedActionColors;

    private CombatAction[] _actions;

    [Header("Action Description Panel")]
    [SerializeField] private GameObject _actionDescriptionPaneel;
    [SerializeField] private TextMeshProUGUI _actionName, _energyConsumed, _actionDescription;

    public static UIController current;

    private bool showingSettings = false;

    private void Awake()
    {
        current = this;
        AssignUI(playerUI, true);
        AssignUI(enemyUI, false);
    }

    private void Start()
    {
        print("UIController called!");
        Button btn = actionButtons[0].GetComponent<Button>();
        btn.onClick.AddListener(delegate { SelectedAction(0); });

        Button btn1 = actionButtons[1].GetComponent<Button>();
        btn1.onClick.AddListener(delegate { SelectedAction(1); });

        Button btn2 = actionButtons[2].GetComponent<Button>();
        btn2.onClick.AddListener(delegate { SelectedAction(2); });

        Button btn3 = actionButtons[3].GetComponent<Button>();
        btn3.onClick.AddListener(delegate { SelectedAction(3); });

        FinishMatchUI.current.ShowEndGamePanel(false);

        //Class Icons events
        GameplayController.current.OnAmmoIconSetup += SetupAmmoImage;
        GameplayController.current.OnAmmoUpdate += UpdateAmmoImage;

        GameplayController.current.OnSummonIconSetup += SetupSummonImage;
        GameplayController.current.OnSummonUpdate += UpdateSummonText;

        GameplayController.current.OnTrapIconSetup += SetupTrapImage;
        GameplayController.current.OnTrapUpdate += UpdateTrapText;

        GameplayController.current.OnBerserkIconsSetup += SetupBerserkImages;
        GameplayController.current.OnBerserkDamageUpdate += UpdateBerserkDamageText;
        GameplayController.current.OnBerserkConcentrationUpdate += UpdateBerserkConcentrationText;

        GameplayController.current.OnEnemySelectedAction += UpdateSelectedActionText;

        pauseSprite = Resources.Load<Sprite>("pause_icon");
        playSprite = Resources.Load<Sprite>("play_icon");
        DisplayTimer(true);
    }

    private void AssignUI(GameObject gameObject, bool isPlayer)
    {
        var tempAmmoImage = gameObject.transform.Find("Ammo Icon").GetComponent<Image>();
        var tempSummonObject = gameObject.transform.Find("Summon Icon");
        var tempTrapObject = gameObject.transform.Find("Trap Icon");
        var tempBerserkObject = gameObject.transform.Find("Berserk Icons");
        var tempActionText = gameObject.transform.Find("Action").GetComponent<TextMeshProUGUI>();
        var tempImages = gameObject.transform.Find("HP Holder").GetComponentsInChildren<Image>();
        var energyPanel = gameObject.transform.Find("EnergyPanel");
        var tempEnergyText = energyPanel.transform.Find("EnergyText").GetComponent<TextMeshProUGUI>();
        var tempEnergySlider = energyPanel.transform.Find("EnergySlider").GetComponent<Slider>();

        if (isPlayer)
        {
            _playerAmmoImage = tempAmmoImage;
            _playerSummonObject = tempSummonObject?.gameObject;
            _playerSummonText = tempSummonObject?.GetComponentInChildren<TextMeshProUGUI>();
            _playerTrapUIObject = tempTrapObject?.gameObject;
            _playerTrapText = tempTrapObject?.GetComponentInChildren<TextMeshProUGUI>();
            _playerBerserkObject = tempBerserkObject?.gameObject;
            _playerBerserkDamageText = tempBerserkObject?.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            _playerBerserkConcentrationText = tempBerserkObject?.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            _playerAction = tempActionText;
            _playerHeartHPImages = tempImages;
            _playerEnergy = tempEnergyText;
            _playerEnergySlider = tempEnergySlider;
        }
        else
        {
            _enemyAmmoImage = tempAmmoImage;
            _enemySummonObject = tempSummonObject?.gameObject;
            _enemySummonText = tempSummonObject?.GetComponentInChildren<TextMeshProUGUI>();
            _enemyTrapUIObject = tempTrapObject?.gameObject;
            _enemyTrapText = tempTrapObject?.GetComponentInChildren<TextMeshProUGUI>();
            _enemyBerserkObject = tempBerserkObject?.gameObject;
            _enemyBerserkDamageText = tempBerserkObject?.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            _enemyBerserkConcentrationText = tempBerserkObject?.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            _enemyAction = tempActionText;
            _enemyHeartHPImages = tempImages;
            _enemyEnergy = tempEnergyText;
            _enemyEnergySlider = tempEnergySlider;
        }
    }

    #region In Game Functions
    public void ShowSettingsIcon(bool show)
    {
        settingsIcon.SetActive(show);
        if (!show)
            ShowActionDescriptionPanel(false);

        pauseIcon.sprite = show ?
            playSprite : pauseSprite;
    }
    public void ShowSettingsMenu()
    {
        showingSettings = !showingSettings;
        settingsScreen.SetActive(showingSettings);
    }

    public void DisplayTimer(bool enable)
    {
        timer.gameObject.SetActive(enable);
    }

    public void UpdateTimer(int time)
    {
        string prevTimerText = timer.text;
        if (time > 1)
        {
            timer.text = "" + time;
            if (timer.text != prevTimerText)
            {
                AudioController.current.PlayBeepSound();
            }
        }
        else
        {
            timer.text = LocalisationSystem.GetLocalisedValue("meow").ToUpper() + "!";
            if (timer.text != prevTimerText)
            {
                AudioController.current.PlayMeowSound();
            }
        }
    }

    // display consumed energy for each action button
    public void DisplayConsumedEnergy(Player player = null)
    {
        if (player != null)
        {
            for (int i = 0; i < player.Actions.Length; i++)
            {
                var action = player.Actions[i];
                if (action.EnergyConsumed != 0)
                {
                    var energyTextUI = actionButtons[i].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
                    energyTextUI.enabled = true;
                    energyTextUI.text = action.EnergyConsumed.ToString();
                }
            }
        }
    }

    public void EnableActionButtons(Player player = null)
    {
        if (player != null)
        {
            for (int i = 0; i < player.Actions.Length; i++)
            {
                var action = player.Actions[i];
                var canPerform = action.CanPerform();

                if (!canPerform && action.CurrentCooldown > 0)
                {
                    var textUI = actionButtons[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                    textUI.enabled = true;
                    textUI.text = action.CurrentCooldown.ToString();
                }

                actionButtons[i].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = canPerform && action.EnergyConsumed != 0;
                actionButtons[i].interactable = canPerform;
            }
        }
        else
        {
            for (int i = 0; i < actionButtons.Length; i++)
            {
                actionButtons[i].interactable = false;
                actionButtons[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
                actionButtons[i].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }
    }


    public void ShowActionsOnPause(CombatAction[] combatActions, bool? wasPaused = null)
    {
        _actions = combatActions;
        if (wasPaused != null)
        { 
            // if was - paused - create backup of actions statuses(interactable/not)
            if ((bool)wasPaused)
            {
                buttonStatuses = new bool[actionButtons.Length];
                for (int i = 0; i < actionButtons.Length; i++)
                {
                    buttonStatuses[i] = actionButtons[i].interactable;
                    actionButtons[i].interactable = true;
                    actionButtons[i].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }
            // else restore to previously saved statuses
            else
            {
                for (int i = 0; i < actionButtons.Length; i++)
                {
                    actionButtons[i].interactable = buttonStatuses[i];
                    actionButtons[i].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = buttonStatuses[i];
                }
            }
        }
       

        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = _actions[i].ToString();
            //update energy text
            actionButtons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _actions[i].EnergyConsumed.ToString();

            ColorBlock cb = actionButtons[i].colors;
            cb.normalColor = selectedActionColors[_actions[i].Classification];
            actionButtons[i].colors = cb;
        }
    }

    public void SetupPlayerActions(CombatAction[] combatActions)
    {
        _actions = combatActions;
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _actions[i].ToString();
            //setup energy
            actionButtons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _actions[i].EnergyConsumed.ToString();
            ColorBlock cb = actionButtons[i].colors;
            cb.normalColor = selectedActionColors[_actions[i].Classification];
            actionButtons[i].colors = cb;
        }
    }

    public void SelectedAction(int id)
    {
        CombatAction selectedAction = _actions[id];
        if (!GameplayController.current.isPaused)
        {
            GameplayController.current.player.SelectedAction = selectedAction;
            UpdateSelectedActionText(selectedAction.ToString());
        }
        else
        {
            ShowActionDescription(selectedAction.Type.ToString(), selectedAction.EnergyConsumed, selectedAction.Description);
            ShowActionDescriptionPanel(true);
        }
    }

    public void UpdateSelectedActionText(string actionText, bool isPlayer = true)
    {
        if (isPlayer)
        {
            _playerAction.text = actionText;
        }
        else
        {
            _enemyAction.text = actionText;
        }
    }

    public void UpdateEnergyText(int energy, int maxEnergy, bool isPlayer)
    {
        if (isPlayer)
        {
            _playerEnergy.text = energy + "/" + maxEnergy;
            _playerEnergySlider.value = (float)energy / (float)maxEnergy;

        }
        else
        {
            _enemyEnergy.text = energy + "/" + maxEnergy;
            _enemyEnergySlider.value = (float)energy / (float)maxEnergy;
        }
    }
    #region Setup/Update class icons methods

    //RangerUI
    public void SetupAmmoImage(bool enable, bool isPlayer = false)
    {
        Image img = isPlayer ? 
            _playerAmmoImage 
            : _enemyAmmoImage;

        if (img != null)
            img.enabled = enable;
    }

    public void UpdateAmmoImage(bool enable, bool isPlayer = false)
    {
        Image img = isPlayer ? 
            _playerAmmoImage 
            : _enemyAmmoImage;

        img.color = enable ?
            Color.red :
            Color.white;
    }

    private void SetupClassIconUI(GameObject playerObject, GameObject enemyObject, bool enable, bool isPlayer)
    {
        GameObject gObject = isPlayer ?
            playerObject
            : enemyObject;

        gObject?.SetActive(enable);
    }

    //Summoner UI
    public void SetupSummonImage(bool enable, bool isPlayer = false)
    {
        SetupClassIconUI(_playerSummonObject, _enemySummonObject, enable, isPlayer);

        UpdateSummonText(0, isPlayer);
    }

    public void UpdateSummonText(int summonCount, bool isPlayer = false)
    {
        TextMeshProUGUI text = isPlayer ?
            _playerSummonText
            : _enemySummonText;

        if (text != null)
            text.text = summonCount.ToString();
    }

    //TrapperUI
    public void SetupTrapImage(bool enable, bool isPlayer = false)
    {
        SetupClassIconUI(_playerTrapUIObject, _enemyTrapUIObject, enable, isPlayer);

        UpdateTrapText(0, isPlayer);
    }

    public void UpdateTrapText(int trapPointsCount, bool isPlayer = false)
    {
        TextMeshProUGUI text = isPlayer ?
            _playerTrapText
            : _enemyTrapText;

        if (text != null)
            text.text = trapPointsCount+"/10";
    }
    public void SetupBerserkImages(bool enable, bool isPlayer = false)
    {
        SetupClassIconUI(_playerBerserkObject, _enemyBerserkObject, enable, isPlayer);

        UpdateBerserkConcentrationText(0.5f, isPlayer);
        UpdateBerserkDamageText(1, isPlayer);
    }

    public void UpdateBerserkConcentrationText(float concentration, bool isPlayer = false)
    {
        TextMeshProUGUI text = isPlayer ?
            _playerBerserkConcentrationText
            : _enemyBerserkConcentrationText;

        if (text != null)
            text.text = String.Format("{0:0.##\\%}", concentration*100);
    }
    public void UpdateBerserkDamageText(int damage, bool isPlayer = false)
    {
        TextMeshProUGUI text = isPlayer ?
            _playerBerserkDamageText
            : _enemyBerserkDamageText;

        if (text != null)
            text.text = damage.ToString();
    }

    #endregion

    public void SetupHPImages(int hpCount, bool isPlayer)
    {
        if (hpCount > 10)
        {
            throw new Exception("More than 10 hp are not supported!");
        }
        UpdateHPs(hpCount, isPlayer ?
            _playerHeartHPImages : _enemyHeartHPImages);
    }

    private void UpdateHPs(int hpCount, Image[] hearthHPImages)
    {
        for (int i = 0; i < 5; i++)
        {
            var invisColor = Color.white;
            invisColor.a = 0f;
            hearthHPImages[i].color = invisColor;
        }
        int currentCount = 0;
        while (currentCount < hpCount && currentCount < 5)
        {
            hearthHPImages[currentCount].color = Color.red;
            currentCount++;
        }
        for (int i = 5; i < hpCount; i++)
        {
            hearthHPImages[i - 5].color = Color.yellow;
        }
    }

    public void UpdateDamagedHPs(int oldHPCount, int newHPCount, bool isPlayer)
    {
        var hearthHPImages = isPlayer ? _playerHeartHPImages : _enemyHeartHPImages;

        //Select removed hps
        for (int i = oldHPCount; i > newHPCount; i--)
        {
            if (i > 5)
            {
                var image = hearthHPImages[i - 1];
                StartCoroutine(CoroutineHelper.SmoothlyChangeColorAndFade(new ImageAdapter(image), image.color, Color.white, Color.red, 0.1f, 0.25f));
            }
            else if (i - 1 >= 0)
            {
                var image = hearthHPImages[i - 1];
                Color col = Color.white;
                col.a = 0f;
                StartCoroutine(CoroutineHelper.SmoothlyChangeColorAndFade(new ImageAdapter(image), image.color, Color.white, col, 0.1f, 0.25f));
            }
        }
    }

    #endregion

    #region
    public void ShowActionDescriptionPanel(bool show)
    {
        _actionDescriptionPaneel.SetActive(show);
    }

    public void ShowActionDescription(string actionName, int energyConsumed, string actionDescription)
    {
        _actionName.text = actionName;
        _energyConsumed.text = $"{LocalisationSystem.GetLocalisedValue("energy_consumed")}: {energyConsumed}";
        _actionDescription.text = actionDescription;
    }
    #endregion
}

