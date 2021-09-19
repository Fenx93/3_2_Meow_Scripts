using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UIController : MonoBehaviour
{
    [Header("In Game UI")]
    [SerializeField] private TextMeshProUGUI timer;

    [SerializeField] private GameObject playerUI, enemyUI;
    private TextMeshProUGUI _playerAction, _playerEnergy, _enemyAction, _enemyEnergy;

    private Image[] _playerHeartHPImages, _enemyHeartHPImages;
    private Image _playerAmmoImage, _enemyAmmoImage;

    [SerializeField] private Button[] actionButtons;

    [Serializable] public class MyDictionary1 : SerializableDictionary<ActionClassification, Color> { }

    [SerializeField]  private MyDictionary1 selectedActionColors;

    private CombatAction[] _actions;

    [Header("End Game UI")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject returnToMenuButton, gainMoreButton, coinIcon;
    [SerializeField] private TextMeshProUGUI matchResultText, gainedMoneyText, gainedEXPText;

    public static UIController current;

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
        var tempEnergyText = gameObject.transform.Find("Energy").GetComponent<TextMeshProUGUI>();

        if (isPlayer)
        {
            _playerAmmoImage = tempAmmoImage;
            _playerAction = tempActionText;
            _playerHeartHPImages = tempImages;
            _playerEnergy = tempEnergyText;
        }
        else
        {
            _enemyAmmoImage = tempAmmoImage;
            _enemyAction = tempActionText;
            _enemyHeartHPImages = tempImages;
            _enemyEnergy = tempEnergyText;
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
        GameplayController.current.OnDamageReceived += UpdateHPsImages;
        GameplayController.current.OnGameEnded += ShowGameEndMessage;
        GameplayController.current.OnEnemySelectedAction += UpdateSelectedEnemyAction;
    }

    #region In Game Functions
    public void UpdateTimer(int time)
    {
        timer.text = time > 1 ? 
            "" + time
            : "MEOW!";
    }

    private void ShowEndGamePanel(bool show)
    {
        endGamePanel.SetActive(show);
    }

    public void EnableActionButtons(Player player = null)
    {
        if (player != null)
        {
            for (int i = 0; i < player.Actions.Length; i++)
            {
                var action = player.Actions[i];
                var canPerform = action.CanPerform();
                // display cooldown timer on a button
                if (!canPerform && action.CurrentCooldown > 0)
                {
                    var textUI = actionButtons[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                    textUI.enabled = true;
                    textUI.text = action.CurrentCooldown.ToString();
                }
                actionButtons[i].interactable = canPerform;
            }
        }
        else
        {
            for (int i = 0; i < actionButtons.Length; i++)
            {
                actionButtons[i].interactable = false;
                actionButtons[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }
    }

    public void SetupPlayerActions(CombatAction[] combatActions)
    {
        _actions = combatActions;
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = _actions[i].ToString();
            ColorBlock cb = actionButtons[i].colors;
            cb.normalColor = selectedActionColors[_actions[i].Classification];
            actionButtons[i].colors = cb;
        }
    }

    private void SelectedAction(int id)
    {
        CombatAction selectedAction = _actions[id];
        GameplayController.current.player.SelectedAction = selectedAction;
        UpdateSelectedActionText(selectedAction.ToString());
    }

    public void UpdateSelectedActionText(string actionText)
    {
        _playerAction.text = actionText;
    }

    private void UpdateSelectedEnemyAction(string actionText)
    {
        _enemyAction.text = actionText;
    }
    
    public void UpdateEnergyText(int energyText, bool isPlayer)
    {
        if (isPlayer)
        {
            _playerEnergy.text = energyText.ToString();
        }
        else
        {
            _enemyEnergy.text = energyText.ToString();
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


    private void UpdateHPsImages(int hpCount, bool isPlayer)
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
    #endregion

    #region End Game Functions
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
}

