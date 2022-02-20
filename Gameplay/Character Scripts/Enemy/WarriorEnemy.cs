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
                var parry = GetActionByType(ActionType.parry);
                var block = GetActionByType(ActionType.block);

                SelectedAction = CheckSeveralActionForEnergy(parry, block);
                break;
            default:
                base.SelectAction();
                break;
        }
    }
}
