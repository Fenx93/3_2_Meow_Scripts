using UnityEngine;
using static InventorySettings;

public class TabItem : ScriptableObject
{
    public ItemStatus status;
    public ItemQuality quality;

    public virtual string GetID() => null;
}
