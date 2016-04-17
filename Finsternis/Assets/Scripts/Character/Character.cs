using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent (typeof(Inventory))]
public class Character : MonoBehaviour
{
    public UnityEvent onDeath;

    public UnityEvent onDamageTaken;

    public Character lastAttacker;

    [SerializeField]
    private RangedValueAttribute _health = new RangedValueAttribute("hp", 0, 10, 10);

    [SerializeField]
    private RangedValueAttribute _mana = new RangedValueAttribute("mp", 0, 5, 5);

    [SerializeField]
    private RangedValueAttribute _damage = new RangedValueAttribute("dmg", 0, 100, 1);

    [SerializeField]
    private RangedValueAttribute _defense = new RangedValueAttribute("def", 0, 100, 1);

    [SerializeField]
    private RangedValueAttribute _magicResistence = new RangedValueAttribute("mDef", 0, 100, 1);
    
    private AttributeTable _attributeTable;

    //EVERY character has an inventory - even enemies
    private Inventory _inventory;

    private bool _dead;

    public bool Dead { get { return _dead; } }

    public AttributeTable Attributes
    {
        get { return _attributeTable; }
    }

    protected virtual void Awake()
    {
        _inventory = GetComponent<Inventory>();
        CreateAttributesTable();
    }

    private void CreateAttributesTable()
    {
        _attributeTable = new AttributeTable(_health, _mana, _damage, _defense);
    }

    void Update()
    {
        if (!_dead && _health.Value == 0)
        {
            Die();
        }
    }

    public virtual void Attack(Character target)
    {
        target.Damage(new DamageInfo(DamageInfo.DamageType.physical, _damage.IntValue, this));
    }

    public virtual void Damage(DamageInfo info)
    {
        _health.Subtract(Mathf.Max(0, info.Amount - (info.Type == DamageInfo.DamageType.physical ? _defense.Value : _magicResistence.Value)));
        onDamageTaken.Invoke();
        lastAttacker = info.Source;
    }

    private void Die()
    {
        _dead = true;
        onDeath.Invoke();
        onDeath.RemoveAllListeners();
    }
}
