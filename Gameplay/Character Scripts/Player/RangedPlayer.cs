using System.Linq;

public class RangedPlayer : Player
{
    private bool _playerHasAmmo;

    public RangedPlayer(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    { }

    private bool HasAmmo
    {
        get
        {
            var c = (RangerClass)SelectedCharacterClass;
            return c.HasAmmo;
        }
    }

    public override bool CanAttack()
    {
        bool canAttack = false;
        var aggressiveActions = Actions.Where(x => x.Classification == ActionClassification.aggressive);
        foreach (var action in aggressiveActions)
        {
            if (action.EnergyConsumed <= Energy && HasAmmo)
                return true;
        }
        return canAttack;
    }
}
