
public class TrapperPlayer : Player
{
    public TrapperPlayer(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    { }

    public override bool CanAttack()
    {
        return false;
    }
}
