using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttributeTable : MonoBehaviour
{
    private Dictionary<string, EntityAttribute> attributes;

    public AttributeTable()
    {
        attributes = new Dictionary<string, EntityAttribute>();
    }

    public void AddAttribute<T>(string name, T defaultValue, params AttributeConstraint[] constraints)
    {
        EntityAttribute<T> a = new EntityAttribute<T>(name);
        if (constraints != null)
            a.AddConstraints(constraints);
        if (!a.SetValue(defaultValue))
            throw new ArgumentException("Invalid default value (" + defaultValue + ") for attribute " + a);
        AddAttribute(a);
    }

    public void AddAttribute(EntityAttribute att)
    {
        if (!attributes.ContainsKey(att.Name))
        {
            attributes.Add(att.Name, att);
        }
    }

    public EntityAttribute<T> GetAttribute<T>()
    {
        foreach (EntityAttribute a in attributes.Values)
        {
            if (typeof(T).Equals(a.GetType().GetGenericArguments()[0]))
            {
                return a as EntityAttribute<T>;
            }
        }
        return null;
    }

    public List<EntityAttribute<T>> GetAttributes<T>()
    {
        List<EntityAttribute<T>> foundAttributes = new List<EntityAttribute<T>>();
        foreach (EntityAttribute a in attributes.Values)
        {
            if (typeof(T).Equals(a.GetType().GetGenericArguments()[0]))
            {
                foundAttributes.Add((EntityAttribute<T>)a);
            }
        }
        return foundAttributes;
    }

    public EntityAttribute GetAttribute(string name)
    {
        return attributes[name];
    }
}
