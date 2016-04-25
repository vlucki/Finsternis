using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Inventory))]
public class Character : Entity
{
    //EVERY character has an inventory - even enemies
    private Inventory _inventory;

    protected override void Awake()
    {
        base.Awake();
        _inventory = GetComponent<Inventory>();
    }
}
