using UnityEngine;

[CreateAssetMenu(fileName = "CombatAction", menuName = "Create new combat action")]
public class SerializableAction : ScriptableObject
{
    public ActionType type;
    public ActionClassification classification;
    public int cooldown;
    public bool enabled = true;
}