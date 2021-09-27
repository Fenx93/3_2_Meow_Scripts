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

    private bool _isPaused = false;

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

        var charCustomizer = CharacterCustomizer.current;

        if (CharacterStore.mainColor != null)
            charCustomizer.avatars[0].SetColor(CharacterStore.mainColor, CharacterPart.mainColor);
        if (CharacterStore.secondaryColor != null)
            charCustomizer.avatars[0].SetColor(CharacterStore.secondaryColor, CharacterPart.secondaryColor);

        if (CharacterStore.eyes != null)
            charCustomizer.avatars[0].SetSprite(CharacterStore.eyes, CharacterPart.eyes);
        if (CharacterStore.ears != null)
            charCustomizer.avatars[0].SetSprite(CharacterStore.ears, CharacterPart.mouth);
        if (CharacterStore.mouth != null)
            charCustomizer.avatars[0].SetSprite(CharacterStore.mouth, CharacterPart.mouth);


        UIController.current.DisplayConsumedEnergy(player);

        CharacterClass eCharClass = _characterClasses.Where(c => c.CharClass == CharClass.ranger).First();
        _enemy = new RangedEnemy(eCharClass, 5, 5);
        CharacterCustomizer.current.avatars[1].SetWeapon(eCharClass.WeaponSprite);


        ResetActions();
        StartCoroutine(nameof(Countdown));
    }

    private void Update()
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

    public void Pause()
    {
        _isPaused = !_isPaused;
        UIController.current.ShowPauseMenu(_isPaused);
    }

    private IEnumerator Countdown()
    {
        float duration = countdownDuration;

        while (duration > 1f)
        {
            if (!_isPaused)
            {
                duration -= (Time.deltaTime * countdownMultiplier);
                UIController.current.UpdateTimer((int)duration);
            }
            yield return null;
        }
        CountdownEnded();
    }

    private IEnumerator Waiting(float duration = 2f)
    {
        float normalizedTime = 0;
        while (normalizedTime <= duration)
        {
            if (!_isPaused)
            {
                normalizedTime += Time.deltaTime;
            }
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
            //TO-DO: attach initiative based on class
            var playerResult = ResolveAction(player, _enemy);
            var enemyResult = ResolveAction(_enemy, player);
            //animate selected actions
            ActionAnimator.current.UpdateSelectedAction(player.SelectedAction, playerResult);
            ActionAnimator.current.UpdateSelectedAction(_enemy.SelectedAction, enemyResult, false);

            if (!_continueGame)
                break;

            StartCoroutine(nameof(Waiting), 2f);
            break;
        }
    }

    private void ResetActions()
    {
        //deselect all actions
        player.SelectedAction = new CombatAction(ActionType.none, ActionClassification.none, 0, null);
        UIController.current.UpdateSelectedActionText("");
        _enemy.SelectedAction = new CombatAction(ActionType.none, ActionClassification.none, 0, null);
        // disable action buttons that are on cooldown
        UIController.current.EnableActionButtons(player);
        ActionAnimator.current.DisableActionVisualisations();
    }

    private CombatResolution ResolveAction(Character actor, Character receiver)
    {
        actor.SelectedAction.StartCooldown();
        actor.ConsumeEnergy(actor.SelectedAction.EnergyConsumed);
        switch (actor.SelectedAction.Type)
        {
            case ActionType.none:
                return CombatResolution.passive;

            case ActionType.rest:
                actor.RestoreEnergy();
                return CombatResolution.passive;

            //warrior actions
            case ActionType.block:
                return CombatResolution.neglected;

            case ActionType.parry:
                if (receiver.SelectedActionType == ActionType.fire || receiver.SelectedActionType == ActionType.slash)
                {
                    receiver.GetDamaged(receiver.CharacterClass.BaseDamage);
                    return CombatResolution.attack;
                }
                return CombatResolution.passive;

            case ActionType.slash:
                if (receiver.SelectedActionType != ActionType.dodge &&
                    receiver.SelectedActionType != ActionType.parry &&
                    receiver.SelectedActionType != ActionType.block)
                {
                    receiver.GetDamaged(actor.CharacterClass.BaseDamage);
                    return CombatResolution.attack;
                }
                return CombatResolution.neglected;

            //ranger actions
            case ActionType.dodge:
                return CombatResolution.neglected;

            case ActionType.reload:
                actor.HasAmmo = true;
                return CombatResolution.passive;

            case ActionType.fire:
                actor.HasAmmo = false;
                if (receiver.SelectedActionType != ActionType.dodge &&
                    receiver.SelectedActionType != ActionType.parry &&
                    receiver.SelectedActionType != ActionType.block)
                {
                    receiver.GetDamaged(actor.CharacterClass.BaseDamage);
                    return CombatResolution.attack;
                }
                return CombatResolution.neglected;
        }
        return CombatResolution.passive;
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
    public event Action<string, bool> OnEnemySelectedAction;
    public void EnemySelectedAction(string actionText)
    {
        OnEnemySelectedAction?.Invoke(actionText, false);
    }
    #endregion
}

public enum ActionType { none, fire, reload, dodge, slash, parry, block, summon, attack, sacrifice, rest };
public enum ActionClassification { none, aggressive, utility, defensive };
public enum CharClass { warrior, ranger, summoner };

public enum CombatResolution { passive, attack, neglected };
