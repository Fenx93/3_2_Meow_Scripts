public class RangedPlayer : Player
{
    private bool _playerHasAmmo;

    public RangedPlayer(CharacterClass characterClass, int hp) : base(characterClass, hp) 
    {
        HasAmmo = false;
    }

    public override bool HasAmmo
    {
        get { return _playerHasAmmo; }
        set
        {
            _playerHasAmmo = value;
            GetActionByType(ActionType.fire).Enabled = value;
            GetActionByType(ActionType.reload).Enabled = !value;
        }
    }
}
