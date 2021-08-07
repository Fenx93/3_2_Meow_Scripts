using System.Linq;

public class CharacterClass
{
    public CharacterClass(CharClass charClass, CombatAction[] actions, int baseDamage)
    {
        CharClass = charClass;
        Actions = actions;
        BaseDamage = baseDamage;
    }

    public CharClass CharClass { get; }
    public CombatAction[] Actions { get; }

    public int BaseDamage { get; }

}
