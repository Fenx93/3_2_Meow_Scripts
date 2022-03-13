public class TrapperEnemy : Enemy
{
    public TrapperEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    { }
    public override void SelectAction()
    {
        var player = GameplayController.current.player;
            switch (EnemyAIType)
        {
            case AIType.random:
                SelectedAction = SelectRandomAvailableAction();
                break;
            case AIType.aggressive:
            case AIType.defensive:
                CombatAction selectedAction;
                if (!player.CanPerformAnyAction())
                {
                    selectedAction = CheckActionForEnergy(GetActionByType(ActionType.anti_utility));
                }
                else if (player.CanAttack())
                {
                    selectedAction = CheckActionForEnergy(GetActionByType(ActionType.anti_attack));
                }
                else
                {
                    selectedAction = CheckActionForEnergy(GetActionByType(ActionType.earn_points));
                }
                SelectedAction = selectedAction;
                break;
            default:
                base.SelectAction();
                break;
        }
    }
}
