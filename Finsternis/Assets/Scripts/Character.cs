using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(Inventory), typeof(Movement))]
[RequireComponent (typeof(AttributeTable))]
public class Character : MonoBehaviour
{
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
        attributes.AddAttribute<HeaderAttribute>("health");
    }
}
