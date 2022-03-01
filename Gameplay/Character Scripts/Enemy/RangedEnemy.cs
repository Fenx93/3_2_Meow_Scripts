public class RangedEnemy : Enemy
{
    private bool _enemyHasAmmo;

    public RangedEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    {
    }

    private bool HasAmmo
    {
        get
        {
            var c = (RangerClass)SelectedCharacterClass;
            return c.HasAmmo;
        }
    }
    public override void SelectAction()
    {
        CombatAction selectedAction;
        switch (EnemyAIType)
        {
            case AIType.random:
                SelectedAction = SelectRandomAvailableAction();
                break;
            case AIType.aggressive:
                selectedAction = HasAmmo ?
                                            GetActionByType(ActionType.fire)
                                            : GetActionByType(ActionType.reload);
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            case AIType.defensive:

                if (GameplayController.current.player.CanAttack())
                {
                    selectedAction = CheckActionForEnergy(GetActionByType(ActionType.dodge));
                }
                else
                {
                    selectedAction = CheckSeveralActions(HasAmmo, ActionType.fire, ActionType.reload);
                }
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            default:
                base.SelectAction();
                break;
        }
    }
}
