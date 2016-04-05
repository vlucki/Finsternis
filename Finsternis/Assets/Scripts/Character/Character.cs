using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent (typeof(Inventory))]
public class Character : MonoBehaviour
{

    [SerializeField]
    public RangedValueAttribute health = new RangedValueAttribute("health", 0, 10, 10);

    [SerializeField]
    public RangedValueAttribute mana = new RangedValueAttribute("mana", 0, 5, 5);

    [SerializeField]
    public RangedValueAttribute damage = new RangedValueAttribute("damage", 0, 100, 1);

    [SerializeField]
    public RangedValueAttribute defense = new RangedValueAttribute("defense", 0, 100, 1);

    [SerializeField]
    private AttributeTable _attributes = new AttributeTable();

    //EVERY character has an inventory - even enemies
    private Inventory inventory;

    public AttributeTable Attributes
    {
        get { return _attributes; }
    }

    protected virtual void Start()
    {
        inventory = GetComponent<Inventory>();
        AddBaseAttributes();
    }

    private void AddBaseAttributes()
    {
        _attributes.AddAttribute(health);
        _attributes.AddAttribute(mana);
        _attributes.AddAttribute(damage);
        _attributes.AddAttribute(defense);
    }
}
