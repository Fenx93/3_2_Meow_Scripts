public class CombatAction
{
    public CombatAction(ActionType type, ActionClassification classification, int energyConsumed, int? cooldown = null, bool enabled = true)
    {
        Type = type;
        Cooldown = cooldown;
        Enabled = enabled;
        EnergyConsumed = energyConsumed;
        Classification = classification;
    }
    public CombatAction(SerializableAction action)
    {
        Type = action.type;
        Cooldown = action.cooldown;
        Enabled = action.enabled;
        EnergyConsumed = action.energyConsumed;
        Classification = action.classification;
    }

    public ActionType Type { get; set; }
    public ActionClassification Classification { get; set; }
    public int EnergyConsumed { get; set; }
    public int? Cooldown { get; set; }
    public int CurrentCooldown { get; set; }
    public int AvailableEnergy { get; set; }
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
        return Enabled /*&& CurrentCooldown == 0*/&& EnergyConsumed <= AvailableEnergy;
    }

    public override string ToString()
    {
        return Type.ToString().ToUpper();
    }

    internal CombatAction Clone()
    {
        return new CombatAction(Type, Classification, EnergyConsumed, Cooldown, Enabled);
    }
}