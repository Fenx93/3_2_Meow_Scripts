public class TrapperEnemy : Enemy
{

    public TrapperEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    { }
    /*public override void CheckForAdditionalVictoryCondition()
    {
        var trapper = (TrapperClass)SelectedCharacterClass;
        if (trapper.TrapPoints > 5)
        {
            GameplayController.current.GameEnded(false);
        }
    }*/

    /*private bool AntiAttackTurnedOn 
    { 
        get
        {
            var c = (TrapperClass)SelectedCharacterClass;
            return c.AntiAttackTurnedOn;
        }
    }
    private bool AntiDefenseTurnedOn
    {
        get
        {
            var c = (TrapperClass)SelectedCharacterClass;
            return c.AntiDefenseTurnedOn;
        }
    }
    private bool AntiUtilityTurnedOn
    {
        get
        {
            var c = (TrapperClass)SelectedCharacterClass;
            return c.AntiUtilityTurnedOn;
        }
    }
*/
    public override void SelectAction()
    {
        CombatAction selectedAction;
        switch (_aiType)
        {
            case AIType.random:
                SelectedAction = SelectRandomAvailableAction();
                break;
            case AIType.aggressive:
                selectedAction = GetActionByType(ActionType.anti_attack);
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;

            case AIType.defensive:
                selectedAction = GetActionByType(ActionType.dodge);
                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            default:
                base.SelectAction();
                break;
        }
    }
}
