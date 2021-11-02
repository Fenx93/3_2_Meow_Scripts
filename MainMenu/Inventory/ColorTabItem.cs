using UnityEngine;

[CreateAssetMenu(fileName = "ColorTabItem", menuName = "Create color tab item")]
public class ColorTabItem : TabItem
{
    public Color color;

    public override string GetID()
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }
}
