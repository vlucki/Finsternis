using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    public UnityEvent onDeath;

    public UnityEvent onDamageTaken;

    public Entity lastDamageSource;

    [SerializeField]
    [Range(0, 20)]
    private int _invincibleFrames = 20;

    private int _remainingInvincibility = 0;

    private float _elapsedAttackTime = 0;

    [SerializeField]
    protected RangedValueAttribute health = new RangedValueAttribute("hp", 0, 10, 10);

    [SerializeField]
    protected RangedValueAttribute mana = new RangedValueAttribute("mp", 0, 5, 5);

    [SerializeField]
    protected RangedValueAttribute damage = new RangedValueAttribute("dmg", 0, 100, 1);

    [SerializeField]
    protected RangedValueAttribute speed = new RangedValueAttribute("spd", 0, 100, 1);

    [SerializeField]
    protected RangedValueAttribute magicResistence = new RangedValueAttribute("mDef", 0, 100, 1);

    [SerializeField]
    protected RangedValueAttribute defense = new RangedValueAttribute("def", 0, 100, 1);

    protected AttributeTable attributeTable;

    private bool _dead;

    public bool Dead { get { return _dead; } }

    public AttributeTable Attributes
    {
        get { return attributeTable; }
    }

    protected virtual void Awake()
    {
        CreateAttributesTable();
    }

    private void CreateAttributesTable()
    {
        attributeTable = new AttributeTable(health, mana, damage, defense, magicResistence, speed);
    }

    void Update()
    {
        if (!_dead)
        {
            if (health.Value == 0)
                Die();
            else if (_remainingInvincibility > 0)
                _remainingInvincibility--;
        }
    }

    public virtual void DoDamage(Entity target)
    {
        DoDamage(target, DamageInfo.DamageType.physical);
    }

    public virtual void DoDamage(Entity target, DamageInfo.DamageType damageType)
    {
        target.ReceiveDamage(new DamageInfo(damageType, damage.IntValue, this));
    }

    public virtual void ReceiveDamage(DamageInfo info)
    {
        if (_remainingInvincibility == 0)
        {
            _remainingInvincibility = _invincibleFrames;
            health.Subtract(Mathf.Max(0, info.Amount - (info.Type == DamageInfo.DamageType.physical ? defense.Value : magicResistence.Value)));
            onDamageTaken.Invoke();
            lastDamageSource = info.Source;
        }
    }

    private void Die()
    {
        _dead = true;
        onDeath.Invoke();
        onDeath.RemoveAllListeners();
    }
}
