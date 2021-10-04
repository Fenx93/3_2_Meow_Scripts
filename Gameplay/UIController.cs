using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using Assets._3_2_Meow_Scripts;

public class UIController : MonoBehaviour
{
    [Header("In Game UI")]
    [SerializeField] private TextMeshProUGUI timer;

    [SerializeField] private GameObject playerUI, enemyUI, settingsScreen, settingsIcon;
    private TextMeshProUGUI _playerAction, _playerEnergy, _enemyAction, _enemyEnergy;
    private Slider _playerEnergySlider, _enemyEnergySlider;

    private Image[] _playerHeartHPImages, _enemyHeartHPImages;
    private Image _playerAmmoImage, _enemyAmmoImage;

    public Button[] actionButtons;
    private bool[] buttonStatuses;

    [Serializable] public class MyDictionary1 : SerializableDictionary<ActionClassification, Color> { }

    public MyDictionary1 selectedActionColors;

    private CombatAction[] _actions;

    [Header("End Game UI")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject returnToMenuButton, gainMoreButton, coinIcon;
    [SerializeField] private TextMeshProUGUI matchResultText, gainedMoneyText, gainedEXPText;

    [Header("Action Description Panel")]
    [SerializeField] private GameObject _actionDescriptionPaneel;
    [SerializeField] private TextMeshProUGUI _actionName, _energyConsumed, _actionDescription;

    public static UIController current;

    private bool showingSettings = false;

    void Awake()
    {
        current = this;
        AssignUI(playerUI, true);
        AssignUI(enemyUI, false);
    }

    private void AssignUI(GameObject gameObject, bool isPlayer)
    {
        var tempAmmoImage = gameObject.transform.Find("Ammo Icon").GetComponent<Image>();
        var tempActionText = gameObject.transform.Find("Action").GetComponent<TextMeshProUGUI>();
        var tempImages = gameObject.transform.Find("HP Holder").GetComponentsInChildren<Image>();
        var energyPanel = gameObject.transform.Find("EnergyPanel");
        var tempEnergyText = energyPanel.transform.Find("EnergyText").GetComponent<TextMeshProUGUI>();
        var tempEnergySlider = energyPanel.transform.Find("EnergySlider").GetComponent<Slider>();

        if (isPlayer)
        {
            _playerAmmoImage = tempAmmoImage;
            _playerAction = tempActionText;
            _playerHeartHPImages = tempImages;
            _playerEnergy = tempEnergyText;
            _playerEnergySlider = tempEnergySlider;
        }
        else
        {
            _enemyAmmoImage = tempAmmoImage;
            _enemyAction = tempActionText;
            _enemyHeartHPImages = tempImages;
            _enemyEnergy = tempEnergyText;
            _enemyEnergySlider = tempEnergySlider;
        }
    }

    void Start()
    {

        Button btn = actionButtons[0].GetComponent<Button>();
        btn.onClick.AddListener(delegate { SelectedAction(0); });

        Button btn1 = actionButtons[1].GetComponent<Button>();
        btn1.onClick.AddListener(delegate { SelectedAction(1); });

        Button btn2 = actionButtons[2].GetComponent<Button>();
        btn2.onClick.AddListener(delegate { SelectedAction(2); });

        Button btn3 = actionButtons[3].GetComponent<Button>();
        btn3.onClick.AddListener(delegate { SelectedAction(3); });

        ShowEndGamePanel(false);

        GameplayController.current.OnAmmoIconSetup += SetupAmmoImage;
        GameplayController.current.OnAmmoUpdate += UpdateAmmoImage;
        GameplayController.current.OnGameEnded += ShowGameEndMessage;
        GameplayController.current.OnEnemySelectedAction += UpdateSelectedActionText;
    }

    #region In Game Functions
    public void ShowSettingsIcon(bool show)
    {
        settingsIcon.SetActive(show);
        if (!show)
        {
            ShowActionDescriptionPanel(false);
        }
    }
    public void ShowSettingsMenu()
    {
        showingSettings = !showingSettings;
        settingsScreen.SetActive(showingSettings);
    }

    public void UpdateTimer(int time)
    {
        timer.text = time > 1 ? 
            "" + time
            : "MEOW!";
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
                // display cooldown timer on a button - no longer required
                /*if (!canPerform && action.CurrentCooldown > 0)
                {
                    var textUI = actionButtons[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                    textUI.enabled = true;
                    textUI.text = action.CurrentCooldown.ToString();
                }*/

                actionButtons[i].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = canPerform && action.EnergyConsumed != 0;
                actionButtons[i].interactable = canPerform;
            }
        }
        else
        {
            for (int i = 0; i < actionButtons.Length; i++)
            {
                actionButtons[i].interactable = false;
                //actionButtons[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
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

    private void SelectedAction(int id)
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

    public void SetupAmmoImage(bool enable, bool isPlayer = false)
    {
        Image img = isPlayer ? 
            _playerAmmoImage 
            : _enemyAmmoImage;

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


    public void SetupHPImages(int hpCount, bool isPlayer)
    {
        if (hpCount > 10)
        {
            throw new Exception("More than 10 hp are not supported!");
        }
        UpdateHPs(hpCount, isPlayer ? _playerHeartHPImages : _enemyHeartHPImages);
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
                StartCoroutine(MainHelper.SmoothlyChangeColorAndFade(new ImageAdapter(image), image.color, Color.white, Color.red, 0.1f, 0.25f));
            }
            else if (i - 1 >= 0)
            {
                var image = hearthHPImages[i - 1];
                Color col = Color.white;
                col.a = 0f;
                StartCoroutine(MainHelper.SmoothlyChangeColorAndFade(new ImageAdapter(image), image.color, Color.white, col, 0.1f, 0.25f));
            }
        }
    }

    #endregion

    #region End Game Functions

    private void ShowEndGamePanel(bool show)
    {
        endGamePanel.SetActive(show);
    }

    private void ShowGameEndMessage(string message, int money, int exp)
    {
        ShowEndGamePanel(true);
        matchResultText.text = message;

        gainedMoneyText.enabled = false;
        coinIcon.SetActive(false);
        gainedEXPText.enabled = false;
        gainMoreButton.SetActive(false);
        returnToMenuButton.SetActive(false);

        gainedMoneyText.text = "+" + money;
        gainedEXPText.text = "+" + exp + "EXP";

        //TO:DO - add after animation
        PlayerStatsTracker.AddMoney(money);
        PlayerStatsTracker.AddExperience(exp);

        //make gained stats appear one after the other
        StartCoroutine(nameof(Countdown), 0);

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
                gainMoreButton.SetActive(true);
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

    #region
    public void ShowActionDescriptionPanel(bool show)
    {
        _actionDescriptionPaneel.SetActive(show);
    }

    public void ShowActionDescription(string actionName, int energyConsumed, string actionDescription)
    {
        _actionName.text = actionName;
        _energyConsumed.text = "Energy consumed : " + energyConsumed;
        _actionDescription.text = actionDescription;
    }
    #endregion
}

