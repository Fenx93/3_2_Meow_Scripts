
using UnityEngine;

public abstract class Enemy : Character
{
    private CombatAction _selectedAction;
    protected readonly AIType _aiType;

    public Enemy(CharacterClass characterClass, int hp) : base(characterClass, hp)
    {
        /*_aiType = AIType.random;*/
        _aiType = (AIType) Random.Range(0, System.Enum.GetNames(typeof(AIType)).Length);
        Debug.Log("Enemy AI is: " + _aiType.ToString());
    }

    public override CombatAction SelectedAction
    {
        get { return _selectedAction; }
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
            if (act.CanPerform() != false)
                return act;
        }
    }

    public abstract void SelectAction();

    protected enum AIType { random, aggressive, defensive }
}
