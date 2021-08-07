public abstract class Player : Character
{
    public Player(CharacterClass characterClass, int hp) : base(characterClass, hp) 
    {
        UIController.current.SetupPlayerActions(Actions);
    }
}
