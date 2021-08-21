using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;

    private TextMeshProUGUI playerAction, enemyAction;

    private Image[] _playerHeartHPImages, _enemyHeartHPImages;
    private Image _playerAmmoImage, _enemyAmmoImage;

    [SerializeField] private GameObject playerUI, enemyUI;
    [SerializeField] private Button[] actionButtons;
    //[SerializeField] private My<ActionClassification, Color> selectedActionColors;
    [Serializable] public class MyDictionary1 : SerializableDictionary<ActionClassification, Color> { }

    [SerializeField]  private MyDictionary1 selectedActionColors;

    private CombatAction[] _actions;

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

        if (isPlayer)
        {
            _playerAmmoImage = tempAmmoImage;
            playerAction = tempActionText;
            _playerHeartHPImages = tempImages;
        }
        else
        {
            _enemyAmmoImage = tempAmmoImage;
            enemyAction = tempActionText;
            _enemyHeartHPImages = tempImages;
        }
    }

    void Start()
    {
        Button btn = actionButtons[0].GetComponent<Button>();
        btn.onClick.AddListener(delegate{ SelectedAction(0); });

        Button btn1 = actionButtons[1].GetComponent<Button>();
        btn1.onClick.AddListener(delegate { SelectedAction(1); });

        Button btn2 = actionButtons[2].GetComponent<Button>();
        btn2.onClick.AddListener(delegate { SelectedAction(2); });

        GameplayController.current.OnAmmoIconSetup += SetupAmmoImage;
        GameplayController.current.OnAmmoUpdate += UpdateAmmoImage;
        GameplayController.current.OnDamageReceived += UpdateHPsImages;
        GameplayController.current.OnGameEnded += ShowGameEndMessage;
        GameplayController.current.OnEnemySelectedAction += UpdateSelectedEnemyAction;
    }

    public void UpdateTimer(int time)
    {
        timer.text = time > 1 ? 
            "" + time
            : "MEOW!";
    }

    private void ShowGameEndMessage(string message)
    {
        timer.text = message;
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
            for (int i = 0; i < 3; i++)
            {
                actionButtons[i].interactable = false;
                actionButtons[i].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }
    }

    public void SetupPlayerActions(CombatAction[] combatActions)
    {
        _actions = combatActions;
        for (int i = 0; i < 3; i++)
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
        playerAction.text = actionText;
    }

    private void UpdateSelectedEnemyAction(string actionText)
    {
        enemyAction.text = actionText;
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
            throw new System.Exception("More than 10 hp are not supported!");
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
}

