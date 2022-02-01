public class BerserkEnemy : Enemy
{
    public BerserkEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    { }

    private float HitChance
    {
        get
        {
            var c = (BerserkClass)SelectedCharacterClass;
            return c.HitChance;
        }
    }
    private int CurrentAttackDamage
    {
        get
        {
            var c = (BerserkClass)SelectedCharacterClass;
            return c.CurrentAttackDamage;
        }
    }

    public override void SelectAction()
    {
        CombatAction selectedAction;
        switch (_aiType)
        {
            case AIType.random:
                SelectedAction = SelectRandomAvailableAction();
                break;
            case AIType.aggressive:
                SelectedAction = CheckSeveralActionForEnergy(GetActionByType(ActionType.smash), GetActionByType(ActionType.enrage));
                break;
            case AIType.defensive:
                if (HitChance != 1)
                {
                    selectedAction = GetActionByType(ActionType.concentrate);
                }
                else if (CurrentAttackDamage < 5)
                {
                    selectedAction = GetActionByType(ActionType.enrage);
                }
                else
                {
                    selectedAction = CheckActionForEnergy(GetActionByType(ActionType.smash));
                }
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            default:
                break;
        }
    }
}
