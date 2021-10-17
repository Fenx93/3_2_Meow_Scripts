using UnityEngine;
using static InventorySettings;

[CreateAssetMenu(fileName ="Tab", menuName ="Create new tab")]
public class Tab : ScriptableObject
{
    public string tabName;
    public CharacterPart editedCharacterPart;

    /*public Color[] colors;
    public Sprite[] sprites;*/
    public TabItem[] items;
}

public enum ItemStatus { unlocked, locked }