public class RangedEnemy : Enemy
{
    private bool _enemyHasAmmo;

    public RangedEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    {
        HasAmmo = false;
    }

    //public override bool HasAmmo
    //{
    //    get { return _enemyHasAmmo; }
    //    set
    //    {
    //        _enemyHasAmmo = value;
    //        GameplayController.current.AmmoIconUpdate(_enemyHasAmmo, false);
    //        GetActionByType(ActionType.fire).Enabled = value;
    //        GetActionByType(ActionType.reload).Enabled = !value;
    //    }
    //}
    public override bool HasAmmo
    {
        get { return _enemyHasAmmo; }
        set
        {
            _enemyHasAmmo = value;
            GameplayController.current.AmmoIconUpdate(_enemyHasAmmo, false);
            GetActionByType(ActionType.fire).Enabled = value;
            GetActionByType(ActionType.reload).Enabled = !value;
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
                selectedAction = HasAmmo ?
                                            GetActionByType(ActionType.fire)
                                            : GetActionByType(ActionType.reload);
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            case AIType.defensive:
                var dodge = GetActionByType(ActionType.dodge);
                selectedAction = dodge.CanPerform() ?
                                            dodge 
                                            : SelectRandomAvailableAction();
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            default:
                break;
        }
    }
}
