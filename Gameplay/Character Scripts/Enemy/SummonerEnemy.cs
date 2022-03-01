public class SummonerEnemy : Enemy
{
    private int MinionsNumber
    {
        get
        {
            var c = (SummonerClass)SelectedCharacterClass;
            return c.MinionsNumber;
        }
    }

    public SummonerEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
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
                selectedAction = (MinionsNumber < 3) ?
                    GetActionByType(ActionType.summon)
                    : GetActionByType(ActionType.attack);

                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            case AIType.defensive:
                var playerCanAttack = GameplayController.current.player.CanAttack();
                if (playerCanAttack && HP < 2 && MinionsNumber > 0)
                {
                    selectedAction = GetActionByType(ActionType.sacrifice);
                }
                else if (MinionsNumber > 3)
                {
                    selectedAction = GetActionByType(ActionType.attack);
                }
                else
                {
                    selectedAction = GetActionByType(ActionType.summon);
                }

                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            default:
                base.SelectAction();
                break;
        }
    }
}
