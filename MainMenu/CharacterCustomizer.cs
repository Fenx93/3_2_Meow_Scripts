using System;
using UnityEngine;
using UnityEngine.UI;
using static InventorySettings;

public class CharacterCustomizer : MonoBehaviour
{

    [Tooltip("If using image, select true, if sprite renderer - false")]
    [SerializeField] bool useImages;

    public Transform[] characters;
    [SerializeField] private bool[] _flipXs;

    [HideInInspector]public CharacterAvatar[] avatars;

    public static CharacterCustomizer current;

    void Awake()
    {
        current = this;

        avatars = new CharacterAvatar[characters.Length];
        for (int i = 0; i < characters.Length; i++)
        {
            avatars[i] = new CharacterAvatar(characters[i], !_flipXs[i], useImages);
        }
    }

}

#region SpriteImageAdapter

public abstract class SpriteImageAdapter
{
    public abstract Color Color { get; set; }
    public abstract Sprite Sprite { get; set; }
}

public class ImageAdapter : SpriteImageAdapter
{
    Image adaptee;
    public ImageAdapter(Image adaptee) { this.adaptee = adaptee; }
    public override Color Color { get { return adaptee.color; } set { adaptee.color = value; } }
    public override Sprite Sprite { get { return adaptee.sprite; } set { adaptee.sprite = value; } }
}

public class SpriteRendererAdapter : SpriteImageAdapter
{
    SpriteRenderer adaptee;
    public SpriteRendererAdapter(SpriteRenderer adaptee) { this.adaptee = adaptee; }
    public override Color Color { get { return adaptee.color; } 
        set { adaptee.color = value; } }
    public override Sprite Sprite { get { return adaptee.sprite; } set { adaptee.sprite = value; } }
}

#endregion

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

    public void IsDamaged()
    {
        DamagePulseAnimate(_head.transform.Find("Primary_Color").gameObject);
        DamagePulseAnimate(_head.transform.Find("Secondary_Color").gameObject);
        DamagePulseAnimate(_head.Find("Hat").gameObject);

        DamagePulseAnimate(_body.transform.Find("Primary_Color").gameObject);
        DamagePulseAnimate(_body.transform.Find("Secondary_Color").gameObject);
        DamagePulseAnimate(_body.Find("Clothes").gameObject);
    }

    private void DamagePulseAnimate(GameObject targetObject)
    {
        LeanTween.color(targetObject, Color.red, 0.5f).setLoopPingPong(3);
    }

    public void SetWeapon(Sprite weaponSprite)
    {
        var weapon = GetSpriteImageAdapter(_weapon);
        weapon.Sprite = weaponSprite;

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
            case CharacterPart.hat:
                part = _head.transform.Find("Hat");
                break;
            case CharacterPart.clothes:
                part = _body.transform.Find("Clothes");
                break;
            default:
                throw new Exception("No Character parts provided!");
        }
        GetSpriteImageAdapter(part).Sprite = sprite;
    }

    public void SetColor(Color newColor, CharacterPart selectedPart)
    {
        if (newColor == null || newColor.Equals(Color.clear))
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
        GetSpriteImageAdapter(_head.transform.Find(part)).Color = newColor;
        GetSpriteImageAdapter(_body.transform.Find(part)).Color = newColor;
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

