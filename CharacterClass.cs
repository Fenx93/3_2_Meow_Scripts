using System.Linq;

public class CharacterClass
{
    public CharacterClass(CharClass charClass, CombatAction[] actions, int baseDamage, bool hasAmmo = false)
    {
        CharClass = charClass;
        Actions = actions;
        BaseDamage = baseDamage;
        HasAmmo = hasAmmo;
    }

    public CharClass CharClass { get; }
    public CombatAction[] Actions { get; }

    public int BaseDamage { get; }
    public bool HasAmmo { get; }

}
