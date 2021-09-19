public class RangedEnemy : Enemy
{
    private bool _enemyHasAmmo;

    public RangedEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    {
        HasAmmo = false;
    }

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
        switch (_aiType)
        {
            case AIType.random:
                SelectedAction = SelectRandomAvailableAction();
                break;
            case AIType.aggressive:
                SelectedAction = HasAmmo ?
                                            GetActionByType(ActionType.fire)
                                            : GetActionByType(ActionType.reload);
                break;
            case AIType.defensive:
                var dodge = GetActionByType(ActionType.dodge);
                SelectedAction = dodge.CanPerform() ?
                                            dodge 
                                            : SelectRandomAvailableAction();
                break;
            default:
                break;
        }
    }
}
