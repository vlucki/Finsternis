using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    public UnityEvent onInteraction;

    public Entity lastDamageSource;

    public EntityAction lastInteraction;

    [SerializeField]
    protected bool interactable = true;

    [SerializeField]
    protected List<string> requiredAttributes;

    protected virtual void Awake()
    {
        //if(requiredAttributes != null)
        //    foreach(string attributeTag in requiredAttributes)
        //        CheckAttribute(health, attributeTag);

        //health = CheckAttribute(health, "hp");
        //defense = CheckAttribute(defense, "def");
        //magicResistance = CheckAttribute(magicResistance, "mdef");
    }

    protected RangedValueAttribute CheckAttribute(RangedValueAttribute attribute, string name)
    {
        if (!attribute)
            attribute = ((RangedValueAttribute)GetAttribute(name));
        if (!attribute)
        {
            attribute = gameObject.AddComponent<RangedValueAttribute>();
            attribute.AttributeName = name;
        }

        if (!attribute)
            throw new NullReferenceException("Could not load attribute " + name + "\nMaybe it was not set in the inspector?");

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

    public virtual void Interact(EntityAction action)
    {
        lastInteraction = action;
        if(onInteraction != null)
            onInteraction.Invoke();
    }

    public virtual void AtributeUpdated(EntityAttribute attribute)
    {
    }
}
