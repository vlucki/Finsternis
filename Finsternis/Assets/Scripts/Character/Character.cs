using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent (typeof(Inventory))]
public class Character : MonoBehaviour
{
    public delegate void OnDeath();
    public event OnDeath death;

    [SerializeField]
    public RangedValueAttribute health = new RangedValueAttribute("health", 0, 10, 10);

    [SerializeField]
    public RangedValueAttribute mana = new RangedValueAttribute("mana", 0, 5, 5);

    [SerializeField]
    public RangedValueAttribute damage = new RangedValueAttribute("damage", 0, 100, 1);

    [SerializeField]
    public RangedValueAttribute defense = new RangedValueAttribute("defense", 0, 100, 1);

    [SerializeField]
    private AttributeTable _attributeTable = new AttributeTable();

    //EVERY character has an inventory - even enemies
    private Inventory inventory;

    public AttributeTable Attributes
    {
        get { return _attributeTable; }
    }

    protected virtual void Start()
    {
        inventory = GetComponent<Inventory>();
        AddBaseAttributes();
    }

    private void AddBaseAttributes()
    {
        _attributeTable.AddAttribute(health);
        _attributeTable.AddAttribute(mana);
        _attributeTable.AddAttribute(damage);
        _attributeTable.AddAttribute(defense);
    }

    void Update()
    {
        if (health.Value == 0)
        {
            if (death != null)
            {
                death();
                death = null;
            }
        }
    }
}
