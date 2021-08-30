using System;
using System.Collections;
using UnityEngine;
using static Tab;

public class GameplayController : MonoBehaviour
{
    [SerializeField] float countdownMultiplier = 1.5f;

    private bool _continueGame = true;
    [SerializeField] private Sprite _warriorWeapon, _rangerWeapon;

    [HideInInspector] public Player player;
    private Enemy _enemy;

    public static GameplayController current;

    void Awake()
    {
        current = this;
    }

    void Start()
    {
        var _warriorClass = new CharacterClass(CharClass.warrior, new CombatAction[] {
            new CombatAction(ActionType.slash, ActionClassification.aggressive, 2), 
            new CombatAction(ActionType.parry, ActionClassification.utility, 2), 
            new CombatAction(ActionType.block, ActionClassification.defensive, 2) },
            1);
        var _rangerClass = new CharacterClass(CharClass.ranger, new CombatAction[] { 
            new CombatAction(ActionType.fire, ActionClassification.aggressive, 0, false), 
            new CombatAction(ActionType.reload, ActionClassification.utility), 
            new CombatAction(ActionType.dodge, ActionClassification.defensive, 1) }, 
            3, true);
        var _summonerClass = new CharacterClass(CharClass.summoner, new CombatAction[] { 
            new CombatAction(ActionType.summon, ActionClassification.utility), 
            new CombatAction(ActionType.attack, ActionClassification.aggressive), 
            new CombatAction(ActionType.sacrifice, ActionClassification.defensive) }
        , 2);

        Sprite weaponSprite = null;
        switch ((CharClass)PlayerPrefs.GetInt("SelectedClass"))
        {
            case CharClass.warrior:
                player = new WarriorPlayer(_warriorClass, 5);
                weaponSprite = _warriorWeapon;
                break;
            case CharClass.ranger:
                player = new RangedPlayer(_rangerClass, 5);
                weaponSprite = _rangerWeapon;
                break;
            default:
                break;
        }
        CharacterCustomizer.current.avatars[0].SetWeapon(weaponSprite);
        CharacterCustomizer.current.avatars[0].SetColor(MainMenuController.current.mainColor, CharacterPart.mainColor);
        CharacterCustomizer.current.avatars[0].SetColor(MainMenuController.current.secondaryColor, CharacterPart.secondaryColor);

        CharacterCustomizer.current.avatars[0].SetSprite(MainMenuController.current.eyes, CharacterPart.eyes);
        CharacterCustomizer.current.avatars[0].SetSprite(MainMenuController.current.ears, CharacterPart.eyes);
        CharacterCustomizer.current.avatars[0].SetSprite(MainMenuController.current.mouth, CharacterPart.mouth);

        _enemy = new RangedEnemy(_rangerClass, 5);
        CharacterCustomizer.current.avatars[1].SetWeapon(_rangerWeapon);


        ResetActions();
        StartCoroutine(nameof(Countdown));
    }

    private IEnumerator Countdown()
    {
        float duration = 4f;

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
        player.SelectedAction = new CombatAction(ActionType.none, ActionClassification.none);
        UIController.current.UpdateSelectedActionText("");
        _enemy.SelectedAction = new CombatAction(ActionType.none, ActionClassification.none);
        // disable action buttons that are on cooldown
        UIController.current.EnableActionButtons(player);
    }

    private void ResolveAction(Character actor, Character receiver)
    {
        actor.SelectedAction.StartCooldown();
        switch (actor.SelectedAction.Type)
        {
            case ActionType.none:
            //warrior actions
            case ActionType.block:
                break;
            case ActionType.parry:
                if (receiver.SelectedActionType == ActionType.fire || receiver.SelectedActionType == ActionType.slash)
                    receiver.GetDamaged(receiver.CharacterClass.BaseDamage);
                break;
            case ActionType.slash:
                if (receiver.SelectedActionType != ActionType.dodge && receiver.SelectedActionType != ActionType.parry && receiver.SelectedActionType != ActionType.block)
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
                if (receiver.SelectedActionType != ActionType.dodge && receiver.SelectedActionType != ActionType.parry && receiver.SelectedActionType != ActionType.block)
                    receiver.GetDamaged(actor.CharacterClass.BaseDamage);
                break;
        }
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

    public event Action<string> OnGameEnded;
    public void GameEnded(string message)
    {
        _continueGame = false;
        OnGameEnded?.Invoke(message);
    }
    public event Action<string> OnEnemySelectedAction;
    public void EnemySelectedAction(string message)
    {
        OnEnemySelectedAction?.Invoke(message);
    }
    #endregion
}

public enum ActionType { none, fire, reload, dodge, slash, parry, block, summon, attack, sacrifice };
public enum ActionClassification { none, aggressive, utility, defensive };
public enum CharClass { warrior, ranger, summoner };
