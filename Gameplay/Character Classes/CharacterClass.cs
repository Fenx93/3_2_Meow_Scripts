using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterClass", menuName = "Create new character class")]
public class CharacterClass : ScriptableObject
{
    [SerializeField] private CharClass charClass;
    [SerializeField] private SerializableAction[] actions;
    /*[SerializeField] private int hp;*/
    [SerializeField] private int baseDamage;
    [SerializeField] private Sprite weaponSprite;
    [SerializeField] private bool hasAmmo = false;

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
    /*public int HP { get => hp; }*/
    public int BaseDamage { get => baseDamage; }
    public bool HasAmmo { get => hasAmmo; }
    public Sprite WeaponSprite { get => weaponSprite; }

}
