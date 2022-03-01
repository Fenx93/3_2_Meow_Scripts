
using UnityEngine;

public abstract class Enemy : Character
{
    private CombatAction _selectedAction;
    public virtual AIType EnemyAIType { get; set; }

    public Enemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    {
        characterClass.IsPlayer = false;
        //_aiType = AIType.passive;
        UpdateAI();
        Debug.Log("Enemy AI is: " + EnemyAIType.ToString());
    }

    public void UpdateAI(AIType? aIType = null)
    {
        EnemyAIType = aIType.HasValue? 
            aIType.Value
            : (AIType)Random.Range(0, System.Enum.GetNames(typeof(AIType)).Length - 1);
    }

    public override CombatAction SelectedAction
    {
        get => _selectedAction;
        set
        {
            _selectedAction = value;

            GameplayController.current.EnemySelectedAction(
                _selectedAction.Type != ActionType.none ?
                    _selectedAction.ToString()
                    : " "
            );
        }
    }

    protected CombatAction SelectRandomAction()
    {
        return Actions[Random.Range(0, Actions.Length)];
    }

    protected CombatAction SelectRandomAvailableAction()
    {
        while (true)
        {
            var act = SelectRandomAction();
            if (act.CanPerform() != false && act.EnergyConsumed <= Energy)
            {
                if (act.Type == ActionType.rest)
                {
                    if ((float)Energy < (float) MaxEnergy / 2)
                    {
                        return act;
                    }
                }
                else
                {
                    return act;
                }
            }
        }
    }

    public virtual void SelectAction()
    {
        switch (EnemyAIType)
        {
            case AIType.passive:
                SelectedAction = new CombatAction(GameplayController.current.doNothingAction);
                break;
        }
    }

    protected CombatAction CheckSeveralActionForEnergy(ActionType prioritizedActionType, ActionType secondaryActionType)
    {
        return CheckSeveralActionForEnergy(prioritizedActionType, secondaryActionType);
    }
    protected CombatAction CheckSeveralActionForEnergy(CombatAction prioritizedAction, CombatAction secondaryAction)
    {
        if (prioritizedAction.EnergyConsumed <= Energy)
        {
            return prioritizedAction;
        }
        else if(secondaryAction.EnergyConsumed <= Energy)
        {
            return secondaryAction;
        }
        return GetActionByType(ActionType.rest);
    }

    protected CombatAction CheckSeveralActions(CombatAction prioritizedAction, CombatAction secondaryAction, bool condition)
    {
        return condition ? prioritizedAction : secondaryAction;
    }

    protected CombatAction CheckSeveralActions(bool condition, ActionType prioritizedActionType, ActionType secondaryActionType)
    {
        return condition ? 
            GetActionByType(prioritizedActionType) 
            : GetActionByType(secondaryActionType);
    }

}
// Make sure AIType.passive is last!
public enum AIType { random, aggressive, defensive, passive }
