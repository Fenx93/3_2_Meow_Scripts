using UnityEngine;

public class CombatAction
{
    public CombatAction(ActionType type, ActionClassification classification, int energyConsumed, Sprite visualisation, string description, AudioClip actionSound, int? cooldown = null, bool enabled = true)
    {
        Type = type;
        Cooldown = cooldown;
        Visualisation = visualisation;
        Enabled = enabled;
        EnergyConsumed = energyConsumed;
        Classification = classification;
        Description = description;
        ActionSound = actionSound;
    }
    public CombatAction(SerializableAction action)
    {
        Type = action.type;
        Visualisation = action.visualisation;
        Cooldown = action.cooldown;
        Enabled = action.enabled;
        EnergyConsumed = action.energyConsumed;
        Classification = action.classification;
        Description = action.description;
        ActionSound = action.actionSound;
    }

    public ActionType Type { get; set; }
    public ActionClassification Classification { get; set; }
    public Sprite Visualisation { get; set; }
    public int EnergyConsumed { get; set; }
    public int? Cooldown { get; set; }
    public int CurrentCooldown { get; set; }
    public int AvailableEnergy { get; set; }
    public string Description { get; set; }
    public AudioClip ActionSound { get; set; }
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
        return Type.ToString().ToUpper().Replace('_','-');
    }

    internal CombatAction Clone()
    {
        return new CombatAction(Type, Classification, EnergyConsumed, Visualisation, Description, ActionSound, Cooldown, Enabled);
    }
}