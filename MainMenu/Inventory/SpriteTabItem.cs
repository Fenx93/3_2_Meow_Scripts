using UnityEngine;

[CreateAssetMenu(fileName = "SpriteTabItem", menuName = "Create sprite tab item")]
public class SpriteTabItem : TabItem
{
    public Sprite sprite;

    public override string GetID()
    {
        return sprite.name;
    }
}
