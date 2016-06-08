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

    private RangedValueAttribute health;
    private RangedValueAttribute defense;
    private RangedValueAttribute magicResistance;

    private int _remainingInvincibility = 0;    

    private bool _dead;

    public bool Dead { get { return _dead; } }

    protected virtual void Awake()
    {
        health = CheckAttribute(health, "hp");
        defense = CheckAttribute(defense, "def");
        magicResistance = CheckAttribute(magicResistance, "mdef");
    }

    private RangedValueAttribute CheckAttribute(RangedValueAttribute attribute, string name)
    {
        if (!attribute)
            attribute = ((RangedValueAttribute)GetAttribute(name));
        if (!attribute)
        {
            attribute = gameObject.AddComponent<RangedValueAttribute>();
            attribute.AttributeName = name;
        }

        return attribute;
    }

    public RangedValueAttribute GetAttribute(string name)
    {
        RangedValueAttribute[] attributes = GetComponents<RangedValueAttribute>();
        foreach (RangedValueAttribute attribute in attributes)
            if (attribute.AttributeName.Equals(name))
                return attribute;
        return null;
    }

    protected virtual void Update()
    {
        if (!_dead)
        {
            if (health.Value == 0)
                Die();
            else if (_remainingInvincibility > 0)
                _remainingInvincibility--;
        }
    }

    public virtual void ReceiveDamage(DamageInfo info)
    {
        if (_remainingInvincibility == 0)
        {
            _remainingInvincibility = _invincibleFrames;
            health.Subtract(Mathf.Max(0, info.Amount - (info.Type == DamageInfo.DamageType.physical ? defense.Value : magicResistance.Value)));
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
