using UnityEngine;

[CreateAssetMenu(fileName = "CombatAction", menuName = "Create new combat action")]
public class SerializableAction : ScriptableObject
{
    public ActionType type;
    public ActionClassification classification;
    public bool baseClassificationEnabled = true;
    public Sprite visualisation;
    public int cooldown;
    public int energyConsumed;
    //public string description;
    public bool enabled = true;
    public bool ableToCancelActions = false;
    public AudioClip actionSound;
}