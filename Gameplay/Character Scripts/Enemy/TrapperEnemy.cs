public class TrapperEnemy : Enemy
{
    public TrapperEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    { }
    public override void SelectAction()
    {
        switch (EnemyAIType)
        {
            case AIType.random:
            case AIType.aggressive:
            case AIType.defensive:
                SelectedAction = SelectRandomAvailableAction();
                break;
            default:
                base.SelectAction();
                break;
        }
    }
}
