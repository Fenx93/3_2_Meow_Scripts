public abstract class Player : Character
{
    public Player(CharacterClass characterClass, int hp, int maxEnergy) : base(characterClass, hp, maxEnergy) 
    {
        UIController.current.SetupPlayerActions(Actions);
        characterClass.IsPlayer = true;
    }
}
