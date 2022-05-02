using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static InventorySettings;

public class GameplayController : MonoBehaviour
{
    public float countdownMultiplier = 1.5f, countdownDuration = 4f;

    private bool _continueGame = true;
    [SerializeField] private ScriptableCharacterClass[] _characterClasses;

    [HideInInspector] public Player player;
    [HideInInspector] public Enemy enemy;

    [HideInInspector] public bool isPaused = false;

    public static GameplayController current;

    public delegate void DelayedDelegate(int damage);
    public Dictionary<DelayedDelegate, int> delayedActions;

    private bool alreadyExecuting = false;

    public SerializableAction doNothingAction;

    [SerializeField] private bool enablePostProcessing;
    [SerializeField] private GameObject postProcessing;
    [SerializeField] private bool allEnemiesAvailable = false;

    public bool startByDefault, isTraining;
    [SerializeField] private GameObject trainingObject;
    [SerializeField] private AudioClip gameplayMusic;

    void Awake()
    {
        current = this;
    }

    void Start()
    {
        isTraining = Convert.ToBoolean(PlayerPrefs.GetInt("IsTraining"));
        if (enablePostProcessing)
        {
            postProcessing.SetActive(false);
        }
        SettingsMenu.Instance.Initiate();
        countdownDuration = SettingsMenu.Instance.GetSeconds() + 1f;

        FindObjectOfType<MapController>().SetupRandomMap();
        AudioController.current.PlayMusic(gameplayMusic);
        SetupGame();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Player damaged");
            player.GetDamaged(1);
        }
    }

    public void SetupGame()
    {
        delayedActions = new Dictionary<DelayedDelegate, int>();

        var selectedClass = (CharClass)System.Enum.Parse(typeof(CharClass), PlayerPrefs.GetString("SelectedClass"));
        if (isTraining)
        {
            selectedClass = CharClass.ranger;
            startByDefault = false;
        }

        CharacterClass charClass = CharacterClassHelper.GetCharacterClass(
            _characterClasses.Where(c => c.CharClass == selectedClass).First());
        switch (selectedClass)
        {
            case CharClass.warrior:
                player = new WarriorPlayer(charClass, 5, 5);
                break;
            case CharClass.ranger:
                player = new RangedPlayer(charClass, 5, 5);
                break;
            case CharClass.summoner:
                player = new SummonerPlayer(charClass, 5, 5);
                break;
            case CharClass.berserk:
                player = new BerserkPlayer(charClass, 5, 0);
                break;
            case CharClass.trapper:
                player = new TrapperPlayer(charClass, 5, 5);
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

        if (CharacterStore.hat != null)
            charCustomizer.avatars[0].SetSprite(CharacterStore.hat, CharacterPart.hat);
        if (CharacterStore.clothes != null)
            charCustomizer.avatars[0].SetSprite(CharacterStore.clothes, CharacterPart.clothes);

        UIController.current.DisplayConsumedEnergy(player);

        enemy = GetRandomEnemyClass(/*CharClass.warrior, AIType.defensive*/);

        trainingObject.SetActive(isTraining);

        ResetActions();
        if (startByDefault)
        {
            StartCoroutine(nameof(Countdown));
        }
    }

    private Enemy GetRandomEnemyClass(CharClass? passedClass = null, AIType? aIType = null)
    {
        Enemy enemy = null;
        var selectedClass = GetAvailableClass(passedClass);
        CharacterClass charClass = CharacterClassHelper.GetCharacterClass(
            _characterClasses.Where(c => c.CharClass == selectedClass).First());
        enemy = selectedClass switch
        {
            CharClass.warrior => new WarriorEnemy(charClass, 5, 5),
            CharClass.ranger => new RangedEnemy(charClass, 5, 5),
            CharClass.summoner => new SummonerEnemy(charClass, 5, 5),
            CharClass.berserk => new BerserkEnemy(charClass, 5, 0),
            CharClass.trapper => new TrapperEnemy(charClass, 5, 5),
            _ => throw new Exception("Encountered missing CharClass!"),
        };
        if (aIType.HasValue)
            enemy.UpdateAI(aIType.Value);

        CharacterCustomizer.current.avatars[1].SetWeapon(enemy.SelectedCharacterClass.WeaponSprite);
        SetupEnemyLooks();
        return enemy;
    }

    private CharClass GetAvailableClass(CharClass? passedClass = null)
    {
        if (isTraining)
            return CharClass.ranger;

        if (passedClass.HasValue)
            return passedClass.Value;

        CharClass[] availableClasses = _characterClasses.Select(x => x.CharClass).ToArray();

        if (!allEnemiesAvailable)
        {
            availableClasses = _characterClasses
                .Where(x => x.UnlocksForEnemyAtLevel <= PlayerStatsTracker.CurrentLvl)
                .Select(x => x.CharClass).ToArray();
        }

        return availableClasses[UnityEngine.Random.Range(0, availableClasses.Count())];
    }

    public ScriptableCharacterClass GetNewlyUnlockedClass()
        => _characterClasses
            .Where(x => x.UnlocksAtLevel == PlayerStatsTracker.CurrentLvl)
            .FirstOrDefault();

    public ScriptableCharacterClass GetNewlyUnlockedEnemyClass()
        => _characterClasses
            .Where(x => x.UnlocksForEnemyAtLevel == PlayerStatsTracker.CurrentLvl)
            .FirstOrDefault();

    private void SetupEnemyLooks()
    {
        var charCustomizer = CharacterCustomizer.current;
        if (EnemyPresetsHolder.mainColor != null)
            charCustomizer.avatars[1].SetColor(EnemyPresetsHolder.mainColor, CharacterPart.mainColor);
        if (EnemyPresetsHolder.secondaryColor != null)
            charCustomizer.avatars[1].SetColor(EnemyPresetsHolder.secondaryColor, CharacterPart.secondaryColor);

        //set enemy's appearance
        if (EnemyPresetsHolder.hat != null)
            charCustomizer.avatars[1].SetSprite(EnemyPresetsHolder.hat, CharacterPart.hat);
        if (EnemyPresetsHolder.clothes != null)
            charCustomizer.avatars[1].SetSprite(EnemyPresetsHolder.clothes, CharacterPart.clothes);

    }

    public void StartGame()
    {
        StartCoroutine(nameof(Countdown));
    }

    public void Pause()
    {
        isPaused = !isPaused;
        if (postProcessing != null)
        {
            postProcessing.SetActive(isPaused);
        }
        UIController.current.ShowSettingsIcon(isPaused);
        UIController.current.ShowActionsOnPause(player.Actions, isPaused);
    }

    private IEnumerator Countdown()
    {
        float duration = countdownDuration;

        while (duration > 1f)
        {
            if (!isPaused)
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
            if (!isPaused)
            {
                normalizedTime += Time.deltaTime;
            }
            yield return null;
        }
        ResetActions();
        if (_continueGame)
        {
            StartCoroutine(nameof(Countdown));
        }
    }

    private void CountdownEnded()
    {
        UIController.current.EnableActionButtons();
        enemy.SelectAction();
        // decrease action cooldowns
        player.DecreaseActionCooldowns();
        enemy.DecreaseActionCooldowns();

        while (_continueGame)
        {
            //TO-DO: attach initiative based on class
            var playerResult = ResolveAction(player, enemy);
            var enemyResult = ResolveAction(enemy, player);
            //animate selected actions
            ActionAnimator.current.UpdateSelectedAction(player.SelectedAction, playerResult);
            ActionAnimator.current.UpdateSelectedAction(enemy.SelectedAction, enemyResult, false);

            player.CheckForAdditionalVictoryCondition();
            enemy.CheckForAdditionalVictoryCondition();

            if (!_continueGame)
                break;

            StartCoroutine(nameof(Waiting), 2f);
            break;
        }
    }

    public void ResetActions()
    {
        //deselect all actions
        player.SelectedAction = new CombatAction(ActionType.none, ActionClassification.none, 0, null, null);
        UIController.current.UpdateSelectedActionText("");
        enemy.SelectedAction = new CombatAction(ActionType.none, ActionClassification.none, 0, null, null);
        // disable action buttons that are on cooldown
        UIController.current.EnableActionButtons(player);
        ActionAnimator.current.DisableActionVisualisations();
    }

    public CombatResolution ResolveAction(Character actor, Character receiver)
    {
        actor.SelectedAction.StartCooldown();
        actor.ConsumeEnergy(actor.SelectedAction.EnergyConsumed);
        AudioController.current.PlaySFX(actor.SelectedAction.ActionSound);

        bool executeAction = true;

        if (CharacterClassHelper.CharacterCanCancelActions(receiver))
        {
            if (receiver.SelectedCharacterClass.ActionWasCancelled(actor, receiver))
                executeAction = false;
        }

        actor.SelectedCharacterClass.ExecuteActionPrerequisition(actor);
        return executeAction ?
            actor.SelectedCharacterClass.ExecuteAction(actor, receiver)
            : CombatResolution.neglected;
    }

    public void ExecuteDelayedActions()
    {
        if (!alreadyExecuting)
        {
            alreadyExecuting = true;
            foreach (var action in delayedActions)
            {
                action.Key.Invoke(action.Value);
            }
            delayedActions.Clear();
            alreadyExecuting = false;
        }
    }


    public void GoBackToMainMenu()
    {
        SaveGameController.SaveData();
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }

    public void RefreshActionButtons(bool isPlayer)
    {
        if (isPaused)
        {
            var actions = isPlayer ? player.Actions :
                enemy.Actions;
            UIController.current.ShowActionsOnPause(actions);
        }
    }

    public void GameEnded(bool won)
    {
        string message;
        int exp, money;

        _continueGame = false;
        UIController.current.DisplayTimer(false);
        AudioController.current.StopMusic();

        if (won)
        {
            message = LocalisationSystem.GetLocalisedValue("match_result_victory");
            exp = 100;
            money = 50;
            VictoryAnimatorScript.current.SetValues(message, money, exp);
            VictoryAnimatorScript.current.StartAnimation(CharacterCustomizer.current.characters[0], CharacterCustomizer.current.characters[1], won);
        }
        else
        {
            message = LocalisationSystem.GetLocalisedValue("match_result_defeat");
            exp = 50;
            money = 25;
            VictoryAnimatorScript.current.SetValues(message, money, exp);
            VictoryAnimatorScript.current.StartAnimation(CharacterCustomizer.current.characters[1], CharacterCustomizer.current.characters[0], won);
        }
    }


    #region Events

    //Ranger
    public event Action<bool, bool> OnAmmoIconSetup;
    public void AmmoIconSetup(bool enabled, bool isPlayer)
        => OnAmmoIconSetup?.Invoke(enabled, isPlayer);

    public event Action<bool, bool> OnAmmoUpdate;
    public void AmmoIconUpdate(bool enabled, bool isPlayer)
        => OnAmmoUpdate?.Invoke(enabled, isPlayer);

    //Summoner
    public event Action<bool, bool> OnSummonIconSetup;
    public void SummonIconSetup(bool enabled, bool isPlayer)
        => OnSummonIconSetup?.Invoke(enabled, isPlayer);

    public event Action<int, bool> OnSummonUpdate;
    public void SummonIconUpdate(int summonCount, bool isPlayer)
        => OnSummonUpdate?.Invoke(summonCount, isPlayer);

    //Trapper
    public event Action<bool, bool> OnTrapIconSetup;
    public void TrapIconSetup(bool enabled, bool isPlayer) 
        => OnTrapIconSetup?.Invoke(enabled, isPlayer);

    public event Action<int, bool> OnTrapUpdate;
    public void TrapIconUpdate(int summonCount, bool isPlayer)
        => OnTrapUpdate?.Invoke(summonCount, isPlayer);

    //Berserk
    public event Action<bool, bool> OnBerserkIconsSetup;
    public void BerserkIconsSetup(bool enabled, bool isPlayer)
        => OnBerserkIconsSetup?.Invoke(enabled, isPlayer);

    public event Action<int, bool> OnBerserkDamageUpdate;
    public void BerserkDamageUpdate(int damage, bool isPlayer)
        => OnBerserkDamageUpdate?.Invoke(damage, isPlayer);

    public event Action<float, bool> OnBerserkConcentrationUpdate;
    public void BerserkConcentrationUpdate(float concentration, bool isPlayer)
        => OnBerserkConcentrationUpdate?.Invoke(concentration, isPlayer);


    public event Action<string, bool> OnEnemySelectedAction;
    public void EnemySelectedAction(string actionText)
        => OnEnemySelectedAction?.Invoke(actionText, false);
    #endregion
}

public enum ActionType { none, fire, reload, dodge,
    slash, parry, block,
    summon, attack, sacrifice,
    anti_attack, earn_points, anti_utility, 
    smash, concentrate, enrage,
    rest 
};

public enum ActionClassification { none, aggressive, utility, defensive };
public enum CharClass { warrior, ranger, summoner, trapper, berserk };
public enum CombatResolution { passive, attack, neglected };
