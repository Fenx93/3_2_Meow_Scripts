
using UnityEngine;

public abstract class Enemy : Character
{
    private CombatAction _selectedAction;
    protected readonly AIType _aiType;

    public Enemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    {
        characterClass.IsPlayer = false;
        //_aiType = AIType.passive;
        _aiType = (AIType) Random.Range(0, System.Enum.GetNames(typeof(AIType)).Length-1);
        Debug.Log("Enemy AI is: " + _aiType.ToString());
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
        switch (_aiType)
        {
            case AIType.passive:
                SelectedAction = new CombatAction(GameplayController.current.doNothingAction);
                break;
        }
    }

    protected CombatAction CheckActionForEnergy(CombatAction combatAction)
    {
        return combatAction.EnergyConsumed > Energy?
            GetActionByType(ActionType.rest)
            : combatAction;
    }

    protected CombatAction CheckSeveralActionForEnergy(CombatAction prioritizedAction, CombatAction secondaryAction)
    {
        if (prioritizedAction.EnergyConsumed > Energy)
        {
            return prioritizedAction;
        }
        else if(secondaryAction.EnergyConsumed > Energy)
        {
            return secondaryAction;
        }
        return GetActionByType(ActionType.rest);
    }

    // Make sure AIType.passive is last!
    protected enum AIType { random, aggressive, defensive, passive }
}
