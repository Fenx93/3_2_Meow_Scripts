using UnityEngine;

public abstract class CharacterClass
{
    public CharClass CharClass { get; protected set; }
    public CombatAction[] Actions { get; protected set; }
    //public string ClassName { get => CharClass.ToString(); }
    public int BaseDamage { get; protected set; }
    public virtual int Damage { get; protected set; }
    public Sprite WeaponSprite { get; protected set; }
    //public Sprite ClassIcon { get; protected set; }
    //public int UnlocksAtLevel { get; protected set; }
    public bool HasAmmo { get; protected set; }
    public bool HasAdditionalVictory { get; protected set; }
    public bool IsPlayer { get; set; }

    public CharacterClass(ScriptableCharacterClass characterClass)
    {
        CharClass = characterClass.CharClass;
        Actions = characterClass.Actions;
        BaseDamage = characterClass.BaseDamage;
        Damage = characterClass.BaseDamage;
        WeaponSprite = characterClass.WeaponSprite;
        //ClassIcon = characterClass.ClassIcon;
        //UnlocksAtLevel = characterClass.UnlocksAtLevel;
        HasAmmo = false;
        HasAdditionalVictory = false;
    }
    public virtual CombatResolution ExecuteAction(Character actor, Character receiver)
    {
        switch (actor.SelectedAction.Type)
        {
            case ActionType.none:
                return CombatResolution.passive;

            case ActionType.rest:
                actor.RestoreEnergy();
                return CombatResolution.passive;
        }
        return CombatResolution.passive;
    }

    public virtual void ExecuteActionPrerequisition(Character actor) {}

    public virtual void CheckForAdditionalVictoryCondition(bool isPlayer) {}
}

public static class CharacterClassHelper{
    public static CharacterClass GetCharacterClass(ScriptableCharacterClass characterClass)
    {
        CharacterClass charClass;
        switch (characterClass.CharClass)
        {
            case CharClass.warrior:
                charClass = new WarriorClass(characterClass);
                break;
            case CharClass.ranger:
                charClass = new RangerClass(characterClass);
                break;
            case CharClass.summoner:
                charClass = new SummonerClass(characterClass);
                break;
            case CharClass.trapper:
                charClass = new TrapperClass(characterClass);
                break;
            case CharClass.berserk:
                charClass = new BerserkClass(characterClass);
                break;
            default:
                throw new System.Exception("Missing character class type!");
        }
        return charClass;
    }

    public static bool CharacterIsTrapper(Character character, out TrapperClass trapperClass )
    {
        bool condition = character.SelectedCharacterClass.CharClass == CharClass.trapper;
        trapperClass = null;
        if (condition)
        {
            trapperClass = (TrapperClass)character.SelectedCharacterClass;
        }
        return condition;
    }
}
