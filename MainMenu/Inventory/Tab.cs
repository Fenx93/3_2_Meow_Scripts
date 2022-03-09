using System.Linq;
using UnityEngine;
using static InventorySettings;

[CreateAssetMenu(fileName ="Tab", menuName ="Create new tab")]
public class Tab : ScriptableObject
{
    public string tabName;
    public CharacterPart editedCharacterPart;

    public TabItem[] items;

    public StorableItem[] GetAllStorableItems()
        => items.Select(x => new StorableItem(editedCharacterPart, x.GetID(), x.status)).ToArray();
}

public enum ItemStatus { unlocked, locked }