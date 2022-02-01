public class BerserkClass : CharacterClass
{
    public float HitChance { get; private set; }
    public int CurrentAttackDamage { get; private set; }
    public override int Damage { get => CurrentAttackDamage; }

    public BerserkClass(ScriptableCharacterClass characterClass) : base(characterClass)
    {
        HitChance = 0.5f;
        CurrentAttackDamage = 1;
    }

    public override CombatResolution ExecuteAction(Character actor, Character receiver)
    {
        switch (actor.SelectedAction.Type)
        {
            case ActionType.smash:
                if (receiver.SelectedAction.Classification != ActionClassification.defensive)
                {
                    GameplayController.current.delayedActions.Add(receiver.GetDamaged, actor.Damage);
                    return CombatResolution.attack;
                }
                return CombatResolution.neglected;

            case ActionType.concentrate:
                if (HitChance < 1)
                {
                    HitChance += 0.25f;
                }
                return CombatResolution.passive;

            case ActionType.enrage:
                CurrentAttackDamage *= 2;
                return CombatResolution.passive;
            default:
                return base.ExecuteAction(actor, receiver);
        }
    }
}
