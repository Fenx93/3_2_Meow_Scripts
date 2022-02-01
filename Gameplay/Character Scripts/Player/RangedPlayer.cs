public class RangedPlayer : Player
{
    private bool _playerHasAmmo;

    public RangedPlayer(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy)
    {
        //HasAmmo = false;
    }

    //public override bool HasAmmo
    //{
    //    get { return _playerHasAmmo; }
    //    set
    //    {
    //        _playerHasAmmo = value;
    //        GameplayController.current.AmmoIconUpdate(_playerHasAmmo, this is Player);
    //        GetActionByType(ActionType.fire).Enabled = value;
    //        GetActionByType(ActionType.reload).Enabled = !value;
    //    }
    //}
}
