using UnityEngine;

[CreateAssetMenu(fileName ="Tab", menuName ="Create new tab")]
public class Tab : ScriptableObject
{
    public string tabName;
    public CharacterPart editedCharacterPart;

    public Color[] colors;
    public Sprite[] sprites;

    public enum CharacterPart { mainColor, secondaryColor, eyes, nose, mouth }
}
