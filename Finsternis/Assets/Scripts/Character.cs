using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(Inventory), typeof(AttributeTable), typeof(Movement))]
public class Character : MonoBehaviour
{
    [Header("BASIC ATTRIBUTES")]
    [Tooltip("min | current | max")]
    public Vector3 health;
    [Tooltip("min | current | max")]
    public Vector3 mana;
    public float damage;

    //EVERY character has an inventory - even enemies
    private Inventory inventory;
    private AttributeTable attributes;

    protected virtual void Start()
    {
        inventory = GetComponent<Inventory>();
        attributes = GetComponent<AttributeTable>();
        AddBaseAttributes();
    }

    private void AddBaseAttributes()
    {
        attributes.AddAttribute(new RangedValueAttribute("health", health.x, health.z, health.y));
        attributes.AddAttribute(new RangedValueAttribute("mana", mana.x, mana.z, mana.y));
        attributes.AddAttribute<float>("damage", damage);
    }
}
