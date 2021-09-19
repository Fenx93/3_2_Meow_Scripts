using System;
using UnityEngine;
using UnityEngine.UI;
using static Tab;

public class CharacterCustomizer : MonoBehaviour
{

    [Tooltip("If using image, select true, if sprite renderer - false")]
    [SerializeField] bool useImages;

    [SerializeField] private Transform[] _characters;
    [SerializeField] private bool[] _flipXs;

    [HideInInspector]public CharacterAvatar[] avatars;

    public static CharacterCustomizer current;

    void Awake()
    {
        current = this;

        avatars = new CharacterAvatar[_characters.Length];
        for (int i = 0; i < _characters.Length; i++)
        {
            avatars[i] = new CharacterAvatar(_characters[i], !_flipXs[i], useImages);
        }
    }

}

public abstract class SpriteImageAdapter
{
    public abstract Color color { get; set; }
    public abstract Sprite sprite { get; set; }
}

public class ImageAdapter : SpriteImageAdapter
{
    Image adaptee;
    public ImageAdapter(Image adaptee) { this.adaptee = adaptee; }
    public override Color color { get { return adaptee.color; } set { adaptee.color = value; } }
    public override Sprite sprite { get { return adaptee.sprite; } set { adaptee.sprite = value; } }
}

public class SpriteRendererAdapter : SpriteImageAdapter
{
    SpriteRenderer adaptee;
    public SpriteRendererAdapter(SpriteRenderer adaptee) { this.adaptee = adaptee; }
    public override Color color { get { return adaptee.color; } set { adaptee.color = value; } }
    public override Sprite sprite { get { return adaptee.sprite; } set { adaptee.sprite = value; } }
}

public class CharacterAvatar 
{
    private bool _useImage;
    private Transform _gameObject;
    private Transform _head, _body, _weapon;
    private bool _flipX;

    public CharacterAvatar(Transform gameObject, bool flipX, bool useImage)
    {
        _gameObject = gameObject;
        _head = _gameObject.transform.Find("Head");
        _body = _gameObject.transform.Find("Body");
        _weapon = _gameObject.transform.Find("Weapon");
        _flipX = flipX;
        _useImage = useImage;
    }

    public void SetWeapon(Sprite weaponSprite)
    {
        var weapon = GetSpriteImageAdapter(_weapon);
        weapon.sprite = weaponSprite;

        if (weapon.GetType() == typeof(SpriteRendererAdapter))
        {
            SpriteRenderer sprite = _weapon.GetComponent<SpriteRenderer>();
            sprite.flipX = !_flipX;
        }
    }


    public void SetSprite(Sprite sprite, CharacterPart selectedPart)
    {
        if (sprite == null)
            return;
        Transform part = null;

        switch (selectedPart)
        {
            case CharacterPart.eyes:
                part = _head.transform.Find("Eyes");
                break;
            case CharacterPart.nose:
            case CharacterPart.mouth:
                part = _head.transform.Find("Nose&Mouth");
                break;
            default:
                throw new Exception("No Character parts provided!");
        }
        GetSpriteImageAdapter(part).sprite = sprite;
    }

    public void SetColor(Color color, CharacterPart selectedPart)
    {
        if (color == null || color.Equals(Color.clear))
            return;
        string part = null;

        switch (selectedPart)
        {
            case CharacterPart.mainColor:
                part = "Primary_Color";
                break;
            case CharacterPart.secondaryColor:
                part = "Secondary_Color";
                break;
            default:
                throw new Exception("No Character parts provided!");
        }
        GetSpriteImageAdapter(_head.transform.Find(part)).color = color;
        GetSpriteImageAdapter(_body.transform.Find(part)).color = color;
    }

    private SpriteImageAdapter GetSpriteImageAdapter(Transform transform)
    {
        if (_useImage)
        {
            return new ImageAdapter(transform.GetComponent<Image>());
        }
        return new SpriteRendererAdapter(transform.GetComponent<SpriteRenderer>());
    }
}

