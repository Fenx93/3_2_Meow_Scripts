using System.Linq;

public abstract class Character
{
    public Character(CharacterClass characterClass, int hp)
    {
        CharacterClass = characterClass;
        HP = hp;
        GameplayController.current.DamageReceived(HP, this is Player);

        // copy actions, as cooldowns must belong to a particular character's action, not common action
        Actions = new CombatAction[CharacterClass.Actions.Length];
        for (int i = 0; i < CharacterClass.Actions.Length; i++)
        {
            Actions[i] = CharacterClass.Actions[i].Clone();
        }
    }

    public virtual int HP { get; set; }
    public virtual bool HasAmmo { get; set; }
    public virtual CharacterClass CharacterClass { get; }
    public virtual CombatAction[] Actions { get; }
    public virtual CombatAction SelectedAction { get; set; }
    public virtual ActionType SelectedActionType { get { return SelectedAction.Type; } }

    public void GetDamaged(int damage)
    {
        HP -= damage;
        GameplayController.current.DamageReceived(HP, this is Player);

        if (HP <= 0)
        {
            GameplayController.current.GameEnded( this is Player ?
                "Defeat!"
                : "Victory!"
                );
        }
    }


    public CombatAction GetActionByType(ActionType type)
    {
        return Actions.Where(x => x.Type == type).First();
    }

    public void DecreaseActionCooldowns()
    {
        foreach (var action in Actions)
        {
            if (action.CurrentCooldown > 0)
                action.CurrentCooldown--;
        }
    }
}
