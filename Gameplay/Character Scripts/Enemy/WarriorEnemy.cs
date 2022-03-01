public class WarriorEnemy : Enemy
{

    public WarriorEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    { }

    public override void SelectAction()
    {
        switch (EnemyAIType)
        {
            case AIType.random:
                SelectedAction = SelectRandomAvailableAction();
                break;
            case AIType.aggressive:
                SelectedAction = CheckActionForEnergy(GetActionByType(ActionType.slash));
                break;
            case AIType.defensive:
                CombatAction selectedAction;
                if (GameplayController.current.player.CanAttack())
                {
                    selectedAction = CheckSeveralActionForEnergy(ActionType.parry, ActionType.block);
                }
                else
                {
                    selectedAction = CheckActionForEnergy(GetActionByType(ActionType.slash));
                }

                SelectedAction = selectedAction;
                break;
            default:
                base.SelectAction();
                break;
        }
    }
}
