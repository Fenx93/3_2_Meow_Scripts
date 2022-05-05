public class RangerClass : CharacterClass
{
    public RangerClass(ScriptableCharacterClass characterClass) : base(characterClass)
    { }

    private bool _hasAmmo;

    public bool HasAmmo
    {
        get { return _hasAmmo; }
        set
        {
            _hasAmmo = value;
            GameplayController.current.AmmoIconUpdate(_hasAmmo, IsPlayer);
            if (IsPlayer)
            {
                GameplayController.current.player.GetActionByType(ActionType.fire).Enabled = value;
                GameplayController.current.player.GetActionByType(ActionType.reload).Enabled = !value;
            }
            else
            {
                GameplayController.current.enemy.GetActionByType(ActionType.fire).Enabled = value;
                GameplayController.current.enemy.GetActionByType(ActionType.reload).Enabled = !value;
            }
        }
    }

    public override CombatResolution ExecuteAction(Character actor, Character receiver)
    {
        switch (actor.SelectedAction.Type)
        {
            //ranger actions
            case ActionType.dodge:
                return CombatResolution.neglected;

            case ActionType.reload:
                HasAmmo = true;
                return CombatResolution.passive;

            case ActionType.fire:
                if (!receiver.SelectedAction.CanNeglectActions(ActionClassification.defensive))
                {
                    GameplayController.current.delayedActions.Add(new DelayedAction(receiver.GetDamaged, actor.Damage));
                    return CombatResolution.attack;
                }
                return CombatResolution.neglected;
            default:
                return base.ExecuteAction(actor, receiver);
        }
    }

    public override void ExecuteActionPrerequisition(Character actor)
    {
        if (actor.SelectedAction.Type == ActionType.fire)
        {
            HasAmmo = false;
        }
    }
}