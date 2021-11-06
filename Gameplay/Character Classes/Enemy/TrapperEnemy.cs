public class TrapperEnemy : Enemy
{

    public TrapperEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    {
    }

    public bool AntiAttackTurnedOn { get; set; }
    public bool AntiDefenseTurnedOn { get; set; }
    public bool AntiUtilityTurnedOn { get; set; }

    public override void SelectAction()
    {
        CombatAction selectedAction;
        switch (_aiType)
        {
            case AIType.random:
                SelectedAction = SelectRandomAvailableAction();
                break;
            case AIType.aggressive:
                if (!AntiDefenseTurnedOn)
                {
                    selectedAction = GetActionByType(ActionType.anti_defense);
                }
                else if (!AntiUtilityTurnedOn)
                {
                    selectedAction = GetActionByType(ActionType.anti_utility);
                }
                else if (!AntiAttackTurnedOn)
                {
                    selectedAction = GetActionByType(ActionType.anti_attack);
                }
                else
                {
                    selectedAction = GetActionByType(ActionType.dodge);
                }
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;

            case AIType.defensive:
                if (!AntiAttackTurnedOn)
                {
                    selectedAction = GetActionByType(ActionType.anti_attack);
                }
                else if (!AntiUtilityTurnedOn)
                {
                    selectedAction = GetActionByType(ActionType.anti_utility);
                }
                else
                {
                    selectedAction = GetActionByType(ActionType.dodge);
                }
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            default:
                break;
        }
    }
}
