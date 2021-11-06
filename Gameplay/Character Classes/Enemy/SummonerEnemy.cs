public class SummonerEnemy : Enemy
{
    private int _minionsNumber;

    public SummonerEnemy(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    { }

    public void AddMinions()
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
    public void RemoveMinions()
    {
        if (MinionsNumber > 0)
        {
            MinionsNumber--;
        }
    }
    public int MinionsNumber { get => _minionsNumber; set => _minionsNumber = value; }

    public override int Damage { get => CharacterClass.BaseDamage * MinionsNumber * 2; }


    public override void SelectAction()
    {
        CombatAction selectedAction;
        switch (_aiType)
        {
            case AIType.random:
                SelectedAction = SelectRandomAvailableAction();
                break;
            case AIType.aggressive:
                selectedAction = (MinionsNumber < 3) ?
                    GetActionByType(ActionType.summon)
                    : GetActionByType(ActionType.attack);

                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            case AIType.defensive:

                if (HP < 2 && MinionsNumber > 0)
                {
                    selectedAction = GetActionByType(ActionType.sacrifice);
                }
                else if (MinionsNumber > 3)
                {
                    selectedAction = GetActionByType(ActionType.attack);
                }
                else
                {
                    selectedAction = GetActionByType(ActionType.summon);
                }

                SelectedAction = CheckActionForEnergy(selectedAction);
                break;
            default:
                break;
        }
    }
}
