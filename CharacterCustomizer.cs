using UnityEngine;

public class CharacterCustomizer : MonoBehaviour
{
    [SerializeField] private GameObject _player, _enemy;
    public CharacterAvatar playerCharacter, enemyCharacter;

    public static CharacterCustomizer current;

    void Awake()
    {
        current = this;
        playerCharacter = new CharacterAvatar(_player, true);
        enemyCharacter = new CharacterAvatar(_enemy, false);
    }
    // Start is called before the first frame update
    void Start()
    {
    }
}

public class CharacterAvatar 
{
    private GameObject _gameObject;
    private bool _isPlayer;

    public CharacterAvatar(GameObject gameObject, bool isPlayer)
    {
        _gameObject = gameObject;
        _isPlayer = isPlayer;
    }

    public void SetWeapon(Sprite weaponSprite)
    {
        var weapon = _gameObject.transform.Find("Weapon").GetComponent<SpriteRenderer>();
        weapon.sprite = weaponSprite;
        weapon.flipX = !_isPlayer;
    }

    public void SetMainColor(Color color)
    {
        _gameObject.transform.Find("Head").transform.Find("Primary_Color").GetComponent<SpriteRenderer>().color = color;
        _gameObject.transform.Find("Body").transform.Find("Primary_Color").GetComponent<SpriteRenderer>().color = color;
    }

    public void SetSecondaryColor(Color color)
    {
        _gameObject.transform.Find("Head").transform.Find("Secondary_Color").GetComponent<SpriteRenderer>().color = color;
        _gameObject.transform.Find("Body").transform.Find("Secondary_Color").GetComponent<SpriteRenderer>().color = color;
    }
}

