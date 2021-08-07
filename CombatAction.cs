public class CombatAction
{
    public CombatAction(ActionType type, int? cooldown = null, bool enabled = true)
    {
        Type = type;
        Cooldown = cooldown;
        Enabled = enabled;
    }

    public ActionType Type { get; set; }
    public int? Cooldown { get; set; }
    public int CurrentCooldown { get; 
        set; }
    public bool Enabled { get; set; }


    public void StartCooldown()
    {
        if (Cooldown.HasValue)
        {
            CurrentCooldown = Cooldown.Value;
        }
    }

    public bool CanPerform()
    {
        return Enabled && CurrentCooldown == 0;
    }

    public override string ToString()
    {
        return Type.ToString().ToUpper();
    }

    internal CombatAction Clone()
    {
        return new CombatAction(Type, Cooldown, Enabled);
    }
}