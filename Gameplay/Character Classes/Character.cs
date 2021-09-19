using System.Linq;

public abstract class Character
{
    private int _energy;
    public Character(CharacterClass characterClass, int hp, int maxEnergy)
    {
        CharacterClass = characterClass;
        HP = hp;
        MaxEnergy = maxEnergy;
        GameplayController.current.DamageReceived(HP, this is Player);
        GameplayController.current.AmmoIconSetup(CharacterClass.HasAmmo, this is Player);

        // copy actions, as cooldowns must belong to a particular character's action, not common action
        Actions = new CombatAction[CharacterClass.Actions.Length];
        for (int i = 0; i < CharacterClass.Actions.Length; i++)
        {
            Actions[i] = CharacterClass.Actions[i].Clone();
        }
        Energy = maxEnergy;
    }

    public virtual int HP { get; set; }
    public virtual int Energy { 
        get => _energy; 
        set
        {
            _energy = value;
            foreach (var action in Actions)
            {
                action.Enabled = action.EnergyConsumed < _energy;
            }
        }
    }
    public virtual int MaxEnergy { get; set; }
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
            bool won = !(this is Player);
            GameplayController.current.GameEnded(won);
        }
    }

    public void ConsumeEnergy(int energyConsumed)
    {
        Energy -= energyConsumed;
        // Visually update energy
        //GameplayController.current.DamageReceived(HP, this is Player);
    }

    public void RestoreEnergy()
    {
        Energy = MaxEnergy;
        // Visually update energy
        //GameplayController.current.DamageReceived(HP, this is Player);
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
