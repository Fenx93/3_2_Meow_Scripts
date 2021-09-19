using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Tab;

public class GameplayController : MonoBehaviour
{
    [SerializeField] float countdownMultiplier = 1.5f, countdownDuration = 4f;

    private bool _continueGame = true;
    [SerializeField] private CharacterClass[] _characterClasses;

    [HideInInspector] public Player player;
    private Enemy _enemy;

    public static GameplayController current;

    void Awake()
    {
        current = this;
    }

    void Start()
    {
        var selectedClass = (CharClass)PlayerPrefs.GetInt("SelectedClass");
        CharacterClass charClass = _characterClasses.Where(c => c.CharClass == selectedClass).First();
        switch (selectedClass)
        {
            case CharClass.warrior:
                player = new WarriorPlayer(charClass, 5, 5);
                break;
            case CharClass.ranger:
                player = new RangedPlayer(charClass, 5, 5);
                break;
            default:
                break;
        }
        CharacterCustomizer.current.avatars[0].SetWeapon(charClass.WeaponSprite);

        if (MainMenuController.current != null)
        {
            var mainMenu = MainMenuController.current;
            var charCustomizer = CharacterCustomizer.current;

            if (mainMenu.mainColor != null)
                charCustomizer.avatars[0].SetColor(mainMenu.mainColor, CharacterPart.mainColor);
            if (mainMenu.secondaryColor != null)
                charCustomizer.avatars[0].SetColor(mainMenu.secondaryColor, CharacterPart.secondaryColor);

            if (mainMenu.eyes != null)
                charCustomizer.avatars[0].SetSprite(mainMenu.eyes, CharacterPart.eyes);
            if (mainMenu.ears != null)
                charCustomizer.avatars[0].SetSprite(mainMenu.ears, CharacterPart.eyes);
            if (mainMenu.mouth != null)
                charCustomizer.avatars[0].SetSprite(mainMenu.mouth, CharacterPart.mouth);

        }

        UIController.current.DisplayConsumedEnergy(player);

        CharacterClass eCharClass = _characterClasses.Where(c => c.CharClass == CharClass.ranger).First();
        _enemy = new RangedEnemy(eCharClass, 5, 5);
        CharacterCustomizer.current.avatars[1].SetWeapon(eCharClass.WeaponSprite);


        ResetActions();
        StartCoroutine(nameof(Countdown));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameEnded(true);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameEnded(false);
        }
    }

    private IEnumerator Countdown()
    {
        float duration = countdownDuration;

        while (duration > 1f)
        {
            duration -= (Time.deltaTime * countdownMultiplier);
            UIController.current.UpdateTimer((int)duration);
            yield return null;
        }
        CountdownEnded();
    }

    private IEnumerator Waiting(float duration = 2f)
    {
        float normalizedTime = 0;
        while (normalizedTime <= duration)
        {
            normalizedTime += Time.deltaTime;
            yield return null;
        }
        ResetActions();
        StartCoroutine(nameof(Countdown));
    }

    private void CountdownEnded()
    {
        UIController.current.EnableActionButtons();
        _enemy.SelectAction();
        // decrease action cooldowns
        player.DecreaseActionCooldowns();
        _enemy.DecreaseActionCooldowns();

        while (_continueGame)
        {
            ResolveActions();
            if (!_continueGame)
                break;

            StartCoroutine(nameof(Waiting), 2f);
            break;
        }
    }

    private void ResolveActions()
    {
        //TO-DO: attach initiative based on class
        ResolveAction(player, _enemy);
        ResolveAction(_enemy, player);
    }

    private void ResetActions()
    {
        //deselect all actions
        player.SelectedAction = new CombatAction(ActionType.none, ActionClassification.none, 0);
        UIController.current.UpdateSelectedActionText("");
        _enemy.SelectedAction = new CombatAction(ActionType.none, ActionClassification.none, 0);
        // disable action buttons that are on cooldown
        UIController.current.EnableActionButtons(player);
    }

    private void ResolveAction(Character actor, Character receiver)
    {
        actor.SelectedAction.StartCooldown();
        actor.ConsumeEnergy(actor.SelectedAction.EnergyConsumed);
        switch (actor.SelectedAction.Type)
        {
            case ActionType.none:
                break;
            case ActionType.rest:
                actor.RestoreEnergy();
                break;
            //warrior actions
            case ActionType.block:
                break;
            case ActionType.parry:
                if (receiver.SelectedActionType == ActionType.fire || receiver.SelectedActionType == ActionType.slash)
                    receiver.GetDamaged(receiver.CharacterClass.BaseDamage);
                break;
            case ActionType.slash:
                if (receiver.SelectedActionType != ActionType.dodge &&
                    receiver.SelectedActionType != ActionType.parry &&
                    receiver.SelectedActionType != ActionType.block)
                    receiver.GetDamaged(actor.CharacterClass.BaseDamage);
                break;
            //ranger actions
            case ActionType.dodge:
                break;
            case ActionType.reload:
                actor.HasAmmo = true;
                break;
            case ActionType.fire:
                actor.HasAmmo = false;
                if (receiver.SelectedActionType != ActionType.dodge &&
                    receiver.SelectedActionType != ActionType.parry &&
                    receiver.SelectedActionType != ActionType.block)
                    receiver.GetDamaged(actor.CharacterClass.BaseDamage);
                break;
        }
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }

    #region Events

    public event Action<int, bool> OnDamageReceived;
    public void DamageReceived(int hp, bool isPlayer)
    {
        OnDamageReceived?.Invoke(hp, isPlayer);
    }

    public event Action<bool, bool> OnAmmoIconSetup;
    public void AmmoIconSetup(bool enabled, bool isPlayer)
    {
        OnAmmoIconSetup?.Invoke(enabled, isPlayer);
    }

    public event Action<bool, bool> OnAmmoUpdate;
    public void AmmoIconUpdate(bool enabled, bool isPlayer)
    {
        OnAmmoUpdate?.Invoke(enabled, isPlayer);
    }

    public event Action<string, int, int> OnGameEnded;
    public void GameEnded(bool won)
    {
        string message;
        int exp, money;

        if (won)
        {
            message = "Victory!";
            exp = 100;
            money = 50;
        }
        else
        {
            message = "Defeat!";
            exp = 50;
            money = 25;
        }

        _continueGame = false;
        OnGameEnded?.Invoke(message, money, exp);
    }
    public event Action<string> OnEnemySelectedAction;
    public void EnemySelectedAction(string message)
    {
        OnEnemySelectedAction?.Invoke(message);
    }
    #endregion
}

public enum ActionType { none, fire, reload, dodge, slash, parry, block, summon, attack, sacrifice, rest };
public enum ActionClassification { none, aggressive, utility, defensive };
public enum CharClass { warrior, ranger, summoner };
