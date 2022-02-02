using UnityEngine;

public class BerserkClass : CharacterClass
{
    public BerserkClass(ScriptableCharacterClass characterClass) : base(characterClass)
    {
        _initialSetup = true;
        HitChance = 0.5f;
        CurrentAttackDamage = 1;
        _initialSetup = false;
    }

    private bool _initialSetup;
    private float _hitChance;
    private int _currentAttackDamage;
    public float HitChance
    {
        get => _hitChance;
        private set
        {
            _hitChance = value;
            if (!_initialSetup)
            {
                GameplayController.current.BerserkConcentrationUpdate(_hitChance, IsPlayer);
            }
        }
    }
    public int CurrentAttackDamage 
    { 
        get => _currentAttackDamage;
        private set 
        {
            _currentAttackDamage = value;
            if (!_initialSetup)
            {
                GameplayController.current.BerserkDamageUpdate(_currentAttackDamage, IsPlayer);
            }
        }
    }
    public override int Damage { get => CurrentAttackDamage; }


    public override CombatResolution ExecuteAction(Character actor, Character receiver)
    {
        switch (actor.SelectedAction.Type)
        {
            case ActionType.smash:
                if (receiver.SelectedAction.Classification != ActionClassification.defensive)
                {
                    var chance = Random.Range(0f, 1f);
                    var hitOpponent = HitChance >= chance;
                    if (hitOpponent)
                    {
                        GameplayController.current.delayedActions.Add(receiver.GetDamaged, actor.Damage);
                        return CombatResolution.attack;
                    }
                    else
                    {
                        GameplayController.current.delayedActions.Add(actor.GetDamaged, actor.Damage);
                        return CombatResolution.passive;
                    }
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
