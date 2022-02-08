using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterClass", menuName = "Create new character class")]
public class ScriptableCharacterClass : ScriptableObject
{
    [SerializeField] private CharClass charClass;
    [SerializeField] private SerializableAction[] actions;
    [SerializeField] private int baseDamage;
    [SerializeField] private Sprite weaponSprite;
    [SerializeField] private Sprite classIcon;
    [SerializeField] private bool hasAmmo = false;
    [SerializeField] private int unlocksAtLevel = 1;

    private CombatAction[] _actions;

    public CharClass CharClass { get => charClass; }
    public CombatAction[] Actions { 
        get
        {
            if (_actions == null)
            {
                _actions = new CombatAction[actions.Length];
                for (int i = 0; i < actions.Length; i++)
                {
                    _actions[i] = new CombatAction(actions[i]);
                }
            }
            return _actions;
        }
    }
    public string ClassName { get => LocalisationSystem.GetLocalisedValue(CharClass.ToString()); }
    /*public int HP { get => hp; }*/
    public int BaseDamage { get => baseDamage; }
    public bool HasAmmo { get => hasAmmo; }
    public Sprite WeaponSprite { get => weaponSprite; }
    public Sprite ClassIcon { get => classIcon; }
    public int UnlocksAtLevel { get => unlocksAtLevel; }
}
