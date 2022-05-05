using UnityEngine;

public class SummonerClass : CharacterClass
{
    private int _minionsNumber;
    public SummonerClass(ScriptableCharacterClass characterClass) : base(characterClass)
    {
    }

    public int MinionsNumber
    {
        get => _minionsNumber;
        set
        {
            _minionsNumber = value;
            GameplayController.current.SummonIconUpdate(_minionsNumber, IsPlayer);
        }
    }

    public override int Damage { get => BaseDamage * MinionsNumber * 2; }

    private void AddMinions()
    {
        if (MinionsNumber == 0)
        {
            MinionsNumber++;
        }
        else
        {
            MinionsNumber *= 2;
        }
    }
    private void RemoveMinions()
    {
        if (MinionsNumber > 0)
        {
            MinionsNumber--;
        }
    }

    public override CombatResolution ExecuteAction(Character actor, Character receiver)
    {   //summoner actions
        switch (actor.SelectedAction.Type)
        {
            case ActionType.summon:
                AddMinions();
                return CombatResolution.passive;
            case ActionType.sacrifice:
               RemoveMinions();
                actor.HP += 2;
                return CombatResolution.passive;
            case ActionType.attack:
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
}
