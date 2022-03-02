using UnityEngine;

public class CombatAction
{
    public CombatAction(ActionType type, ActionClassification classification, int energyConsumed, 
        Sprite visualisation, AudioClip actionSound, int? cooldown = null, bool enabled = true, bool baseClassificationEnabled = false, bool ableToCancelActions = false)
    {
        Type = type;
        Cooldown = cooldown;
        Visualisation = visualisation;
        Enabled = enabled;
        EnergyConsumed = energyConsumed;
        Classification = classification;
        BaseClassificationEnabled = baseClassificationEnabled;
        AbleToCancelActions = ableToCancelActions;
        //Description = description;
        ActionSound = actionSound;
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
        BaseClassificationEnabled = action.baseClassificationEnabled;
        //Description = action.description;
        ActionSound = action.actionSound;
        AbleToCancelActions = action.ableToCancelActions;
    }

    public ActionType Type { get; set; }
    public ActionClassification Classification { get; set; }
    public bool BaseClassificationEnabled { get; private set; }
    public Sprite Visualisation { get; set; }
    public int EnergyConsumed { get; set; }
    public int? Cooldown { get; set; }
    public int CurrentCooldown { get; set; }
    public int AvailableEnergy { get; set; }
    public string Description { get => LocalisationSystem.GetLocalisedValue($"{Type.ToString().ToLower()}_description"); }
    public AudioClip ActionSound { get; set; }
    public bool Enabled { get; set; }
    public bool AbleToCancelActions { get; set; }

    public bool CanNeglectActions(ActionClassification classification)
    {
        return classification == Classification && BaseClassificationEnabled;
    }

    public void StartCooldown()
    {
        if (Cooldown.HasValue)
        {
            CurrentCooldown = Cooldown.Value;
        }
    }

    public bool CanPerform()
    {
        return Enabled && CurrentCooldown == 0 && EnergyConsumed <= AvailableEnergy;
    }

    public override string ToString()
    {
        try
        {
            return LocalisationSystem.GetLocalisedValue(Type.ToString()).ToUpper()/*.Replace('_','-')*/;
        }
        catch (System.Exception)
        {
            return "Localisation missing!";
        }
    }

    internal CombatAction Clone()
    {
        return new CombatAction(Type, Classification, EnergyConsumed, Visualisation, ActionSound, Cooldown, Enabled, BaseClassificationEnabled, AbleToCancelActions);
    }
}