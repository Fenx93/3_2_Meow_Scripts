public class TrapperEnemy : Enemy
{

    public TrapperEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    { }
    public override void SelectAction()
    {
        CombatAction selectedAction;
        switch (EnemyAIType)
        {
            case AIType.random:
                SelectedAction = SelectRandomAvailableAction();
                break;
            case AIType.aggressive:
                selectedAction = GetActionByType(ActionType.anti_attack);
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;

            case AIType.defensive:
                selectedAction = GetActionByType(ActionType.dodge);
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            default:
                base.SelectAction();
                break;
        }
    }
}
