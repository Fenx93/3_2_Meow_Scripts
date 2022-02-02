public class WarriorClass : CharacterClass
{
    public WarriorClass(ScriptableCharacterClass characterClass) : base(characterClass)
    { }

    public override bool ActionWasCancelled(Character actionExecutor, Character actionDenier)
    {
        switch (actionExecutor.SelectedAction.Classification)
        {
            case ActionClassification.aggressive:
                if (actionDenier.SelectedAction.Type == ActionType.parry)
                    return true;
                break;
        }
        return false;
    }

    public override CombatResolution ExecuteAction(Character actor, Character receiver)
    {
        switch (actor.SelectedAction.Type)
        {
            case ActionType.block:
                return CombatResolution.neglected;

            case ActionType.parry:
                if (receiver.SelectedAction.Classification == ActionClassification.aggressive)
                {
                    GameplayController.current.delayedActions.Add(receiver.GetDamaged, receiver.Damage);
                    return CombatResolution.attack;
                }
                return CombatResolution.passive;

            case ActionType.slash:
                if (receiver.SelectedAction.Classification != ActionClassification.defensive)
                {
                    GameplayController.current.delayedActions.Add(receiver.GetDamaged, actor.Damage);
                    return CombatResolution.attack;
                }
                return CombatResolution.neglected;
            default:
                return base.ExecuteAction(actor, receiver);
        }
    }
}