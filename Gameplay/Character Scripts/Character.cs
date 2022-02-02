using System.Linq;

public abstract class Character
{
    private int _energy;
    public Character(CharacterClass characterClass, int hp, int maxEnergy)
    {
        SelectedCharacterClass = characterClass;
        HP = hp;
        MaxEnergy = maxEnergy;
        UIController.current.SetupHPImages(HP, this is Player);
        GameplayController.current.AmmoIconSetup(characterClass.CharClass == CharClass.ranger, this is Player);
        GameplayController.current.SummonIconSetup(characterClass.CharClass == CharClass.summoner, this is Player);
        GameplayController.current.TrapIconSetup(characterClass.CharClass == CharClass.trapper, this is Player);
        GameplayController.current.BerserkIconsSetup(characterClass.CharClass == CharClass.berserk, this is Player);

        // copy actions, as cooldowns must belong to a particular character's action, not common action
        Actions = new CombatAction[SelectedCharacterClass.Actions.Length];
        for (int i = 0; i < SelectedCharacterClass.Actions.Length; i++)
        {
            Actions[i] = SelectedCharacterClass.Actions[i].Clone();
        }
        Energy = maxEnergy;
    }

    public virtual int Damage { get => SelectedCharacterClass.Damage; }
    public virtual int HP { get; set; }
    public virtual int Energy { 
        get => _energy; 
        set
        {
            _energy = value;
            //pass available energy to each action
            foreach (var action in Actions)
            {
                action.AvailableEnergy = _energy;
            }
            // Check if character uses energy at all (i.e. berserk)
            if (MaxEnergy != 0)
            {
                //disable rest action if full energy
                GetActionByType(ActionType.rest).Enabled = !(_energy == MaxEnergy);
                // Visually update energy
                UIController.current.UpdateEnergyText(Energy, MaxEnergy, this is Player);
            }
        }
    }
    public virtual int MaxEnergy { get; set; }
    //public virtual bool HasAmmo { get; set; }
    public virtual CharacterClass SelectedCharacterClass { get; set; }
    public virtual CombatAction[] Actions { get; }
    public virtual CombatAction SelectedAction { get; set; }

    public void GetDamaged(int damage)
    {
        var prevHP = HP;
        HP -= damage;
        // Play is damaged sound
        AudioController.current.PlayHitSound();
        //Play is damaged animation
        int isPlayer = this is Player ? 
            0 : 1;

        CharacterCustomizer.current.avatars[isPlayer].IsDamaged();

        UIController.current.UpdateDamagedHPs(prevHP, HP, this is Player);

        if (HP <= 0)
        {
            bool won = !(this is Player);
            GameplayController.current.GameEnded(won);
        }
    }

    public void ConsumeEnergy(int energyConsumed)
    {
        Energy -= energyConsumed;
    }

    public void RestoreEnergy()
    {
        Energy = MaxEnergy;
    }

    public void CheckForAdditionalVictoryCondition()
    {
        if (SelectedCharacterClass.HasAdditionalVictory)
        {
            SelectedCharacterClass.CheckForAdditionalVictoryCondition(this is Player);
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
