using UnityEngine;
public class TrapperClass : CharacterClass
{
    public TrapperClass(ScriptableCharacterClass characterClass) : base(characterClass)
    {
        //TrapPoints = 0;
        HasAdditionalVictory = true;
    }
    // if TrapPoints exceed 10, win automatically
    private int _trapPoints;
    public int TrapPoints { get => _trapPoints;
        set
        {
            _trapPoints = value;
            GameplayController.current.TrapIconUpdate(_trapPoints, IsPlayer);
        }
    }
    public override void CheckForAdditionalVictoryCondition(bool isPlayer)
    {
        if (TrapPoints >= 10)
            GameplayController.current.GameEnded(isPlayer);
    }

    //Decided not to keep anti actions stored
    /*public bool AntiAttackTurnedOn { get; private set; }
    public bool AntiDefenseTurnedOn { get; private set; }
    public bool AntiUtilityTurnedOn { get; private set; }*/

    public override bool ActionWasCancelled(Character actionExecutor, Character actionDenier)
    {
        switch (actionExecutor.SelectedAction.Classification)
        {
            case ActionClassification.aggressive:
                if (actionDenier.SelectedAction.Type == ActionType.anti_attack)
                    return true;
                break;

            case ActionClassification.defensive:
                if (actionDenier.SelectedAction.Type == ActionType.anti_defense)
                    return true;
                break;

            case ActionClassification.utility:
                if (actionDenier.SelectedAction.Type == ActionType.anti_utility)
                    return true;
                break;
        }
        return false;
    }

    public override CombatResolution ExecuteAction(Character actor, Character receiver)
    {
        switch (actor.SelectedAction.Type)
        {
            case ActionType.anti_attack:
                if (receiver.SelectedAction.Classification == ActionClassification.aggressive)
                {
                    //TrapPoints++;
                    //GameplayController.current.delayedActions.Add(receiver.GetDamaged, receiver.Damage);
                    return CombatResolution.attack;
                }
                break;

            case ActionType.anti_defense:
                if (receiver.SelectedAction.Classification == ActionClassification.defensive)
                {
                    TrapPoints += 3;
                    return CombatResolution.attack;
                }
                break;

            case ActionType.anti_utility:
                if (receiver.SelectedAction.Classification == ActionClassification.utility)
                {
                    TrapPoints += 2;
                    return CombatResolution.attack;
                }
                break;
            default:
                return base.ExecuteAction(actor, receiver);
        }
        return CombatResolution.passive;
    }
}